using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.Framework.Extensions;
using HansKindberg.Build.XmlTransformation.Tasks.Extensions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationDecorator : IXmlTransformationDecorator, IXmlTransformationMapRepository
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

		public virtual void DecorateFile(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(!this.IsXmlFile(file))
				return;

			this.DecorateFileWithDefaultValues(file);

			file.IsAppConfig(file.ItemSpec.Equals("App.config", StringComparison.OrdinalIgnoreCase));

			var fileIsValid = this.ValidateFileForDecoration(file);

			this.DecorateFileWithDestination(file);
			file.DestinationIsValid(this.ValidateDestinationDecoration(file));

			var xmlTransformationMap = this.GetXmlTransformationMap(file);

			this.DecorateFileWithPreTransform(file, xmlTransformationMap);
			file.PreTransformIsValid(this.ValidatePreTransformDecoration(file));

			this.DecorateFileWithSource(file, xmlTransformationMap);
			file.SourceIsValid(this.ValidateSourceDecoration(file));

			this.DecorateFileWithTransform(file);
			file.TransformIsValid(this.ValidateTransformDecoration(file));

			file.IsValid(fileIsValid && file.DestinationIsValid() && file.SourceIsValid() && (file.PreTransformIsValid() || file.TransformIsValid()));
		}

		protected internal virtual void DecorateFileWithDefaultValues(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			file.Destination(string.Empty);
			file.IsAppConfig(false);
			file.IsValid(false);
			file.OriginalItemSpecification(file.ItemSpec);
			file.PreTransform(string.Empty);
			file.PreTransformIsValid(false);
			file.Source(string.Empty);
			file.SourceIsValid(false);
			file.Transform(string.Empty);
			file.TransformIsValid(false);
		}

		protected internal virtual void DecorateFileWithDestination(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			string destinationDirectory = (!file.IsAppConfig() ? this.XmlTransformationSettings.DestinationDirectory : this.XmlTransformationSettings.AppConfigDestinationDirectory) ?? string.Empty;

			var destinationRelativePath = file.DestinationRelativePath();

			if(string.IsNullOrEmpty(destinationRelativePath))
				destinationRelativePath = file.ToString();

			file.Destination(this.FileSystem.Path.Combine(destinationDirectory, destinationRelativePath));
		}

		protected internal virtual void DecorateFileWithPreTransform(ITaskItem file, IXmlTransformationMap xmlTransformationMap)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(xmlTransformationMap != null)
				file.PreTransform((this.XmlTransformationSettings.TransformMode == TransformMode.Build ? xmlTransformationMap.CommonBuildTransform : xmlTransformationMap.CommonPublishTransform) ?? string.Empty);
		}

		protected internal virtual void DecorateFileWithSource(ITaskItem file, IXmlTransformationMap xmlTransformationMap)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(xmlTransformationMap != null)
				file.Source(xmlTransformationMap.Source ?? string.Empty);

			if(string.IsNullOrEmpty(file.Source()))
				file.Source(file.ToString());
		}

		protected internal virtual void DecorateFileWithTransform(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			file.Transform(this.FileSystem.Path.GetFileNameWithoutExtension(file.ToString()) + "." + (this.XmlTransformationSettings.TransformName ?? string.Empty) + file.Extension());
		}

		public virtual IEnumerable<ITaskItem> GetDecoratedFiles(IEnumerable<ITaskItem> files)
		{
			if(files == null)
				throw new ArgumentNullException("files");

			foreach(var file in files)
			{
				this.DecorateFile(file);

				if(file.IsValid())
				{
					if(file.PreTransformIsValid())
					{
						var filePreTransform = new TaskItem(file);

						filePreTransform.ItemSpec = filePreTransform.Source();

						filePreTransform.Transform(filePreTransform.PreTransform());
						filePreTransform.TransformIsValid(filePreTransform.PreTransformIsValid());

						this.RemoveTemporaryDecorations(filePreTransform);

						yield return filePreTransform;
					}

					if(file.TransformIsValid())
					{
						file.ItemSpec = file.PreTransformIsValid() ? file.Destination() : file.Source();

						this.RemoveTemporaryDecorations(file);

						yield return file;
					}
				}
			}
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
					var newXmlTransformationMap = new XmlTransformationMap(xmlTransformationMapTaskItem);

					xmlTransformationMap = newXmlTransformationMap;
				}

				this.XmlTransformationMapCache.Add(xmlFile.ToString(), xmlTransformationMap);
			}

			return xmlTransformationMap;
		}

		protected internal virtual bool IsXmlFile(ITaskItem file)
		{
			if(file == null)
				return false;

			var extension = file.Extension();

			return extension != null && this.XmlTransformationSettings.XmlFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
		}

		protected internal virtual void LogXmlTransformationMapWarning(string information, ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			this.ValidationLog.LogWarning(information + string.Format(CultureInfo.InvariantCulture, " See the \"XmlTransformationMap\"-itemgroup with identity \"{0}\".", taskItem));
		}

		protected internal virtual void RemoveTemporaryDecorations(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			file.RemovePreTransform();
			file.RemovePreTransformIsValid();

			file.RemoveSource();
			file.RemoveSourceIsValid();
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		protected internal virtual bool ValidateDestinationDecoration(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(string.IsNullOrEmpty(file.Destination()))
			{
				this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The destination for file \"{0}\" is empty.", file));
				return false;
			}

			try
			{
				this.FileSystem.FileInfo.FromFileName(file.Destination());
			}
			catch
			{
				this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The destination \"{0}\" for file \"{1}\" is invalid.", file.Destination(), file));
				return false;
			}

			return true;
		}

		protected internal virtual bool ValidateFileForDecoration(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(this.FileSystem.Path.IsPathRooted(file.ToString()))
			{
				this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" will not be transformed because it has an absolute path.", file));
				return false;
			}

			return true;
		}

		protected internal virtual bool ValidatePreTransformDecoration(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(string.IsNullOrEmpty(file.PreTransform()))
				return false;

			if(!this.FileSystem.File.Exists(file.PreTransform()))
			{
				this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The pre-transform file \"{0}\" for file \"{1}\" does not exist.", file.PreTransform(), file), file);
				return false;
			}

			return true;
		}

		protected internal virtual bool ValidateSourceDecoration(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(!this.FileSystem.File.Exists(file.Source()))
			{
				this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" for file \"{1}\" does not exist.", file.Source(), file), file);
				return false;
			}

			if(this.XmlTransformationSettings.SeparateSourceIsRequired)
			{
				// If file and source have a path to the same file.
				if(file.ToString().Equals(file.Source(), StringComparison.OrdinalIgnoreCase) || file.FullPath().Equals(file.Source(), StringComparison.OrdinalIgnoreCase))
				{
					this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "A separate source is required. The file \"{0}\" with source \"{1}\" will not be transformed because the paths are the same.", file, file.Source()));
					return false;
				}
			}

			return true;
		}

		protected internal virtual bool ValidateTransformDecoration(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(string.IsNullOrEmpty(file.Transform()))
				return false;

			if(!this.FileSystem.File.Exists(file.Transform()))
				return false;

			return true;
		}

		protected internal virtual bool ValidateXmlTransformationMap(IXmlTransformationMap xmlTransformationMap)
		{
			if(xmlTransformationMap == null)
				throw new ArgumentNullException("xmlTransformationMap");

			bool validate = true;

			if(!string.IsNullOrEmpty(xmlTransformationMap.CommonBuildTransform) && !this.FileSystem.File.Exists(xmlTransformationMap.CommonBuildTransform))
			{
				validate = false;
				this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The \"{0}\" \"{1}\" does not exist.", "CommonBuildTransform", xmlTransformationMap.CommonBuildTransform), xmlTransformationMap);
			}

			if(!string.IsNullOrEmpty(xmlTransformationMap.CommonPublishTransform) && !this.FileSystem.File.Exists(xmlTransformationMap.CommonPublishTransform))
			{
				validate = false;
				this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The \"{0}\" \"{1}\" does not exist.", "CommonPublishTransform", xmlTransformationMap.CommonPublishTransform), xmlTransformationMap);
			}

			if(!string.IsNullOrEmpty(xmlTransformationMap.Source) && !this.FileSystem.File.Exists(xmlTransformationMap.Source))
			{
				validate = false;
				this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The \"{0}\" \"{1}\" does not exist.", "Source", xmlTransformationMap.Source), xmlTransformationMap);
			}

			return validate;
		}

		#endregion
	}
}