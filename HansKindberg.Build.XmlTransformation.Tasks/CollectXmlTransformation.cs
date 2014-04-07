using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public abstract class CollectXmlTransformation : Task
	{
		#region Fields

		private readonly IList<ValidationInformation> _executionResult = new List<ValidationInformation>();
		private readonly IFileSystem _fileSystem;
		private ITaskItem[] _files;
		private ITaskItem[] _filesToTransform;
		private string _transformName;
		private ValidationMode _validationMode = Tasks.ValidationMode.Warning;
		private ITaskItem[] _xmlFileExtensions;
		private XmlTransformMode _xmlTransformMode = Tasks.XmlTransformMode.Build;
		private readonly IXmlTransformationMapFactory _xmlTransformationMapFactory;
		private ITaskItem[] _xmlTransformationMaps;

		#endregion

		#region Constructors

		protected CollectXmlTransformation(IFileSystem fileSystem, IXmlTransformationMapFactory xmlTransformationMapFactory)
		{
			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			if(xmlTransformationMapFactory == null)
				throw new ArgumentNullException("xmlTransformationMapFactory");

			this._fileSystem = fileSystem;
			this._xmlTransformationMapFactory = xmlTransformationMapFactory;
		}

		#endregion

		#region Properties

		protected internal virtual IList<ValidationInformation> ExecutionResult
		{
			get { return this._executionResult; }
		}

		protected internal virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		[Required]
		public virtual ITaskItem[] Files
		{
			get { return this._files ?? (this._files = new ITaskItem[0]); }
			set { this._files = value; }
		}

		[Output]
		public virtual ITaskItem[] FilesToTransform
		{
			get { return this._filesToTransform ?? (this._filesToTransform = new ITaskItem[0]); }
			protected internal set { this._filesToTransform = value; }
		}

		[Required]
		public virtual string TransformName
		{
			get { return this._transformName ?? (this._transformName = string.Empty); }
			set { this._transformName = value; }
		}

		public virtual string ValidationMode
		{
			get { return this.ValidationModeInternal.ToString(); }
			set { this.ValidationModeInternal = (ValidationMode) Enum.Parse(typeof(ValidationMode), value, true); }
		}

		protected internal virtual ValidationMode ValidationModeInternal
		{
			get { return this._validationMode; }
			set { this._validationMode = value; }
		}

		[Required]
		public virtual ITaskItem[] XmlFileExtensions
		{
			get { return this._xmlFileExtensions ?? (this._xmlFileExtensions = new ITaskItem[0]); }
			set { this._xmlFileExtensions = value; }
		}

		public virtual string XmlTransformMode
		{
			get { return this.XmlTransformModeInternal.ToString(); }
			set { this.XmlTransformModeInternal = (XmlTransformMode) Enum.Parse(typeof(XmlTransformMode), value, true); }
		}

		protected internal virtual XmlTransformMode XmlTransformModeInternal
		{
			get { return this._xmlTransformMode; }
			set { this._xmlTransformMode = value; }
		}

		protected internal virtual IXmlTransformationMapFactory XmlTransformationMapFactory
		{
			get { return this._xmlTransformationMapFactory; }
		}

		public virtual ITaskItem[] XmlTransformationMaps
		{
			get { return this._xmlTransformationMaps ?? (this._xmlTransformationMaps = new ITaskItem[0]); }
			set { this._xmlTransformationMaps = value; }
		}

		#endregion

		#region Methods

		protected internal virtual void AddError(string information)
		{
			this.ExecutionResult.Add(new ValidationInformation
			{
				ValidationMode = this.ValidationModeInternal,
				Information = information
			});
		}

		protected internal virtual void AddWarning(string information)
		{
			var validationMode = this.ValidationModeInternal == Tasks.ValidationMode.Message ? Tasks.ValidationMode.Message : Tasks.ValidationMode.Warning;

			this.ExecutionResult.Add(new ValidationInformation
			{
				ValidationMode = validationMode,
				Information = information
			});
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool Execute()
		{
			try
			{
				//this.FilesToTransform = this.GetFilesToTransform(this.Files).ToArray();

				foreach(var validationInformation in this.ExecutionResult)
				{
					switch(validationInformation.ValidationMode)
					{
						case Tasks.ValidationMode.Error:
						{
							this.Log.LogError(validationInformation.Information);
							break;
						}
						case Tasks.ValidationMode.Warning:
						{
							this.Log.LogWarning(validationInformation.Information);
							break;
						}
						default:
						{
							this.Log.LogMessage(validationInformation.Information);
							break;
						}
					}
				}

				return this.ExecutionResult.All(validationInformation => validationInformation.ValidationMode != Tasks.ValidationMode.Error);
			}
			catch(Exception exception)
			{
				this.Log.LogError(exception.ToString());

				return false;
			}
		}

		//protected internal virtual IEnumerable<ITaskItem> GetFilesToTransform(IEnumerable<ITaskItem> files)
		//{
		//	var filesToTransform = new List<ITaskItem>();
		//	var xmlFiles = this.GetXmlFiles(files);
		//	return null;
		//}
		protected internal virtual IDictionary<ITaskItem, FileInfoBase> GetXmlFiles(IEnumerable<ITaskItem> files)
		{
			if(files == null)
				throw new ArgumentNullException("files");

			var xmlFiles = new Dictionary<ITaskItem, FileInfoBase>();

			foreach(var file in files)
			{
				var fileInfo = this.FileSystem.FileInfo.FromFileName(file.ItemSpec);

				if(!fileInfo.Exists)
				{
					this.AddWarning(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" does not exist.", file.ItemSpec));
					continue;
				}

				if(this.XmlFileExtensions.Select(xmlFileExtension => xmlFileExtension.ItemSpec).Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase))
					xmlFiles.Add(file, fileInfo);
			}

			return xmlFiles;
		}

		#endregion
	}
}