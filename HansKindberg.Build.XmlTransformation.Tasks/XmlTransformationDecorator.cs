using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.Framework.Extensions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationDecorator : IPotentialFileFactory, IXmlTransformationDecorator, IXmlTransformationMapRepository
	{
		#region Fields

		private readonly IFileSystem _fileSystem;
		private readonly IValidationLog _validationLog;
		private readonly IDictionary<string, IXmlTransformationMap> _xmlTransformationMapCache = new Dictionary<string, IXmlTransformationMap>(StringComparer.OrdinalIgnoreCase);
		private readonly IXmlTransformationSettings _xmlTransformationSettings;

		#endregion

		#region Constructors

		public XmlTransformationDecorator(IFileSystem fileSystem, IXmlTransformationSettings xmlTransformationSettings, IValidationLog validationLog)
		{
			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			if(xmlTransformationSettings == null)
				throw new ArgumentNullException("xmlTransformationSettings");

			var appConfigDestinationDirectory = xmlTransformationSettings.AppConfigDestinationDirectory;
			if(!string.IsNullOrEmpty(appConfigDestinationDirectory))
			{
				try
				{
					fileSystem.DirectoryInfo.FromDirectoryName(appConfigDestinationDirectory);
				}
				catch(Exception exception)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The \"AppConfigDestinationDirectory\" value \"{0}\" is invalid.", appConfigDestinationDirectory), "xmlTransformationSettings", exception);
				}
			}

			var destinationDirectory = xmlTransformationSettings.DestinationDirectory;
			if(!string.IsNullOrEmpty(destinationDirectory))
			{
				try
				{
					fileSystem.DirectoryInfo.FromDirectoryName(destinationDirectory);
				}
				catch(Exception exception)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The \"DestinationDirectory\" value \"{0}\" is invalid.", destinationDirectory), "xmlTransformationSettings", exception);
				}
			}

			var transformName = xmlTransformationSettings.TransformName;
			if(transformName == null)
				throw new ArgumentNullException("xmlTransformationSettings", "The \"TransformName\" value can not be null.");
			try
			{
				fileSystem.FileInfo.FromFileName(transformName);
			}
			catch(Exception exception)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The \"TransformName\" value \"{0}\" is invalid.", transformName), "xmlTransformationSettings", exception);
			}

			if(xmlTransformationSettings.XmlFileExtensions == null)
				throw new ArgumentNullException("xmlTransformationSettings", "The \"XmlFileExtensions\" value can not be null.");

			if(xmlTransformationSettings.XmlTransformationMaps == null)
				throw new ArgumentNullException("xmlTransformationSettings", "The \"XmlTransformationMaps\" value can not be null.");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			this._fileSystem = fileSystem;
			this._validationLog = validationLog;
			this._xmlTransformationSettings = xmlTransformationSettings;
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		protected internal virtual IValidationLog ValidationLog
		{
			get { return this._validationLog; }
		}

		protected internal virtual IDictionary<string, IXmlTransformationMap> XmlTransformationMapCache
		{
			get { return this._xmlTransformationMapCache; }
		}

		protected internal virtual IXmlTransformationSettings XmlTransformationSettings
		{
			get { return this._xmlTransformationSettings; }
		}

		#endregion

		#region Methods

		public virtual IPotentialFile CreatePotentialFile(ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return new PotentialFile(taskItem, this.FileSystem);
		}

		public virtual IEnumerable<ITransformableXmlFile> GetDecoratedXmlFiles(IEnumerable<ITaskItem> files)
		{
			if(files == null)
				throw new ArgumentNullException("files");

			return this.GetXmlFiles(files).Select(this.TryCreateTransformableXmlFile).Where(transformableXmlFile => transformableXmlFile != null).ToArray();
		}

		protected internal virtual IPotentialFile GetMetadataAsPotentialFile(ITaskItem taskItem, string metadataName, Action<string, ITaskItem> logWarning)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(string.IsNullOrEmpty(metadataName))
				throw new ArgumentException("The metadata-name can not be null or empty.", "metadataName");

			var metadataValue = taskItem.GetMetadata(metadataName);

			if(metadataValue != null)
			{
				var potentialFile = this.CreatePotentialFile(new TaskItem(metadataValue));

				if(!potentialFile.Exists && logWarning != null)
					logWarning(string.Format(CultureInfo.InvariantCulture, "The \"{0}\" \"{1}\" does not exist.", metadataName, potentialFile), taskItem);

				return potentialFile;
			}

			return null;
		}

		protected internal virtual IEnumerable<ITaskItem> GetXmlFiles(IEnumerable<ITaskItem> files)
		{
			if(files == null)
				throw new ArgumentNullException("files");

			var xmlFiles = new List<ITaskItem>();

			// ReSharper disable LoopCanBeConvertedToQuery
			foreach(var file in files)
			{
				var extension = file.GetMetadata("Extension");

				if(extension == null)
					continue;

				if(this.XmlTransformationSettings.XmlFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
					xmlFiles.Add(file);
			}
			// ReSharper restore LoopCanBeConvertedToQuery

			return xmlFiles.ToArray();
		}

		public virtual IXmlTransformationMap GetXmlTransformationMap(ITaskItem xmlFile)
		{
			if(xmlFile == null)
				throw new ArgumentNullException("xmlFile");

			IXmlTransformationMap xmlTransformationMap;

			if(!this.XmlTransformationMapCache.TryGetValue(xmlFile.ToString(), out xmlTransformationMap))
			{
				var xmlFileFullPath = xmlFile.GetMetadata("FullPath") ?? string.Empty;

				var xmlTransformationMapTaskItems = new List<ITaskItem>();

				xmlTransformationMapTaskItems.AddRange(this.XmlTransformationSettings.XmlTransformationMaps.Where(taskItem => xmlFile.ItemSpec.Equals(taskItem.ItemSpec, StringComparison.OrdinalIgnoreCase) || xmlFileFullPath.Equals(taskItem.ItemSpec, StringComparison.OrdinalIgnoreCase)));

				if(xmlTransformationMapTaskItems.Count > 1)
					this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The xml-file \"{0}\" has multiple XmlTransformationMaps. The first will be used.", xmlFile), xmlFile);

				var xmlTransformationMapTaskItem = xmlTransformationMapTaskItems.FirstOrDefault();

				if(xmlTransformationMapTaskItem != null)
				{
					var newXmlTransformationMap = new XmlTransformationMap
					{
						Identity = this.CreatePotentialFile(xmlTransformationMapTaskItem),
						CommonBuildTransform = this.GetMetadataAsPotentialFile(xmlTransformationMapTaskItem, "CommonBuildTransform", this.LogXmlTransformationMapWarning),
						CommonPublishTransform = this.GetMetadataAsPotentialFile(xmlTransformationMapTaskItem, "CommonPublishTransform", this.LogXmlTransformationMapWarning),
						Source = this.GetMetadataAsPotentialFile(xmlTransformationMapTaskItem, "Source", this.LogXmlTransformationMapWarning)
					};

					xmlTransformationMap = newXmlTransformationMap;
				}

				this.XmlTransformationMapCache.Add(xmlFile.ToString(), xmlTransformationMap);
			}

			return xmlTransformationMap;
		}

		protected internal virtual void LogXmlTransformationMapWarning(string information, ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			this.ValidationLog.LogWarning(information + string.Format(CultureInfo.InvariantCulture, " See the \"XmlTransformationMap\"-itemgroup with identity \"{0}\".", taskItem));
		}

		protected internal virtual ITransformableXmlFile TryCreateTransformableXmlFile(ITaskItem xmlFile)
		{
			if(xmlFile == null)
				throw new ArgumentNullException("xmlFile");

			if(this.FileSystem.Path.IsPathRooted(xmlFile.ToString()))
			{
				this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" can not be transformed because it has an absolute path.", xmlFile));
				return null;
			}

			var transformableXmlFile = new TransformableXmlFile(xmlFile);

			//if(!this.FileSystem.File.Exists(xmlFile.ToString()))
			//	this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The xml-file \"{0}\" does not exist.", xmlFile));

			string destinationDirectory = (!transformableXmlFile.IsAppConfig ? this.XmlTransformationSettings.DestinationDirectory : this.XmlTransformationSettings.AppConfigDestinationDirectory) ?? string.Empty;

			var destinationRelativePath = transformableXmlFile.DestinationRelativePath();

			if(string.IsNullOrEmpty(destinationRelativePath))
				destinationRelativePath = transformableXmlFile.ToString();

			transformableXmlFile.Destination = this.CreatePotentialFile(new TaskItem(this.FileSystem.Path.Combine(destinationDirectory, destinationRelativePath)));

			var xmlTransformationMap = this.GetXmlTransformationMap(transformableXmlFile);

			if(xmlTransformationMap != null)
			{
				transformableXmlFile.PreTransform = this.XmlTransformationSettings.TransformMode == TransformMode.Build ? xmlTransformationMap.CommonBuildTransform : xmlTransformationMap.CommonPublishTransform;
				transformableXmlFile.Source = xmlTransformationMap.Source;
			}

			if(transformableXmlFile.Source == null && !this.XmlTransformationSettings.SeparateSourceIsRequired)
				transformableXmlFile.Source = this.CreatePotentialFile(transformableXmlFile);

			transformableXmlFile.Transform = this.CreatePotentialFile(new TaskItem(this.FileSystem.Path.GetFileNameWithoutExtension(transformableXmlFile.ToString()) + "." + (this.XmlTransformationSettings.TransformName ?? string.Empty) + transformableXmlFile.Extension()));

			if(this.XmlTransformationSettings.SeparateSourceIsRequired && (transformableXmlFile.ToString().Equals(transformableXmlFile.Source.ToString(), StringComparison.OrdinalIgnoreCase) || transformableXmlFile.FullPath().Equals(transformableXmlFile.Source.FullName, StringComparison.OrdinalIgnoreCase)))
			{
				this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "A separate source is required. The xml-file \"{0}\" with source \"{1}\" will not be include because the paths are the same.", transformableXmlFile, transformableXmlFile.Source));
				return null;
			}

			if(!transformableXmlFile.Source.Exists && (transformableXmlFile.PreTransform.Exists || transformableXmlFile.Transform.Exists))
			{
				this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" for xml-file \"{1}\" does not exist.", transformableXmlFile.Source, transformableXmlFile));
				return null;
			}

			if((transformableXmlFile.PreTransform == null || !transformableXmlFile.PreTransform.Exists) && (transformableXmlFile.Transform == null || !transformableXmlFile.Transform.Exists))
				return null;

			return transformableXmlFile;
		}

		#endregion
	}
}