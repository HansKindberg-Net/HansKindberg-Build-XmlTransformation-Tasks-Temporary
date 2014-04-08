using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationContext : IXmlTransformationContext
	{
		#region Fields

		private readonly IFileSystem _fileSystem;
		private IEnumerable<ITaskItem> _xmlFileExtensions;
		private readonly IDictionary<string, IXmlTransformationMap> _xmlTransformationMapCache = new Dictionary<string, IXmlTransformationMap>(StringComparer.OrdinalIgnoreCase);
		private IEnumerable<ITaskItem> _xmlTransformationMaps;

		#endregion

		#region Constructors

		public XmlTransformationContext(IFileSystem fileSystem)
		{
			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			this._fileSystem = fileSystem;
		}

		#endregion

		#region Properties

		public virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		public virtual IEnumerable<ITaskItem> XmlFileExtensions
		{
			get { return this._xmlFileExtensions ?? (this._xmlFileExtensions = new ITaskItem[0]); }
			set { this._xmlFileExtensions = value; }
		}

		protected internal virtual IDictionary<string, IXmlTransformationMap> XmlTransformationMapCache
		{
			get { return this._xmlTransformationMapCache; }
		}

		public virtual IEnumerable<ITaskItem> XmlTransformationMaps
		{
			get { return this._xmlTransformationMaps ?? (this._xmlTransformationMaps = new ITaskItem[0]); }
			set
			{
				this.XmlTransformationMapCache.Clear();
				this._xmlTransformationMaps = value;
			}
		}

		#endregion

		#region Methods

		public virtual IPotentialFile CreatePotentialFile(ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return new PotentialFile(taskItem, this.FileSystem);
		}

		protected internal virtual ITransformableXmlFile CreateTransformableXmlFile(ITaskItem xmlFile, string transformName, XmlTransformMode transformMode, IValidationLog validationLog)
		{
			return new TransformableXmlFile(xmlFile, transformName, transformMode, this.FileSystem, this, this, validationLog);
		}

		protected internal virtual IPotentialFile GetMetadataAsPotentialFile(ITaskItem taskItem, string metadataName, Action<string, ITaskItem, IValidationLog> logWarning, IValidationLog validationLog)
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
					logWarning(string.Format(CultureInfo.InvariantCulture, "The \"{0}\" \"{1}\" does not exist.", metadataName, potentialFile), taskItem, validationLog);

				return potentialFile;
			}

			return null;
		}

		public virtual IEnumerable<ITransformableXmlFile> GetTransformableXmlFiles(IEnumerable<ITaskItem> files, string transformName, XmlTransformMode transformMode, IValidationLog validationLog)
		{
			return this.GetXmlFiles(files).Select(xmlFile => this.CreateTransformableXmlFile(xmlFile, transformName, transformMode, validationLog)).ToArray();
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

				if(extension != null && this.XmlFileExtensions.Select(xmlFileExtension => xmlFileExtension.ItemSpec).Contains(extension, StringComparer.OrdinalIgnoreCase))
					xmlFiles.Add(file);
			}
			// ReSharper restore LoopCanBeConvertedToQuery

			return xmlFiles.ToArray();
		}

		public virtual IXmlTransformationMap GetXmlTransformationMap(ITaskItem xmlFile, IValidationLog validationLog)
		{
			if(xmlFile == null)
				throw new ArgumentNullException("xmlFile");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			IXmlTransformationMap xmlTransformationMap;

			if(!this.XmlTransformationMapCache.TryGetValue(xmlFile.ToString(), out xmlTransformationMap))
			{
				var xmlFileFullPath = xmlFile.GetMetadata("FullPath") ?? string.Empty;

				var xmlTransformationMapTaskItems = new List<ITaskItem>();

				xmlTransformationMapTaskItems.AddRange(this.XmlTransformationMaps.Where(taskItem => xmlFile.ItemSpec.Equals(taskItem.ItemSpec, StringComparison.OrdinalIgnoreCase) || xmlFileFullPath.Equals(taskItem.ItemSpec, StringComparison.OrdinalIgnoreCase)));

				if(xmlTransformationMapTaskItems.Count > 1)
					this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The xml-file \"{0}\" has multiple XmlTransformationMaps. The first will be used.", xmlFile), xmlFile, validationLog);

				var xmlTransformationMapTaskItem = xmlTransformationMapTaskItems.FirstOrDefault();

				if(xmlTransformationMapTaskItem != null)
				{
					var newXmlTransformationMap = new XmlTransformationMap
					{
						Identity = this.CreatePotentialFile(xmlTransformationMapTaskItem),
						CommonBuildTransform = this.GetMetadataAsPotentialFile(xmlTransformationMapTaskItem, "CommonBuildTransform", this.LogXmlTransformationMapWarning, validationLog),
						CommonPublishTransform = this.GetMetadataAsPotentialFile(xmlTransformationMapTaskItem, "CommonPublishTransform", this.LogXmlTransformationMapWarning, validationLog),
						Source = this.GetMetadataAsPotentialFile(xmlTransformationMapTaskItem, "Source", this.LogXmlTransformationMapWarning, validationLog)
					};

					xmlTransformationMap = newXmlTransformationMap;
				}

				this.XmlTransformationMapCache.Add(xmlFile.ToString(), xmlTransformationMap);
			}

			return xmlTransformationMap;
		}

		protected internal virtual void LogXmlTransformationMapWarning(string information, ITaskItem taskItem, IValidationLog validationLog)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			validationLog.LogWarning(information + string.Format(CultureInfo.InvariantCulture, " See the \"XmlTransformationMap\"-itemgroup with identity \"{0}\".", taskItem));
		}

		#endregion
	}
}