using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public abstract class CollectXmlTransformation : Task
	{
		#region Fields

		private ITaskItem[] _files;
		private ITaskItem[] _filesToTransform;
		private string _transformName;
		private readonly IValidationLog _validationLog = new ValidationLog();
		private ValidationMode _validationMode = Validation.ValidationMode.Warning;
		private ITaskItem[] _xmlFileExtensions;
		private readonly IXmlTransformFactory _xmlTransformFactory;
		private readonly IXmlTransformationMapFactory _xmlTransformationMapFactory;
		private ITaskItem[] _xmlTransformationMaps;

		#endregion

		#region Constructors

		protected CollectXmlTransformation(IXmlTransformFactory xmlTransformFactory, IXmlTransformationMapFactory xmlTransformationMapFactory)
		{
			if(xmlTransformFactory == null)
				throw new ArgumentNullException("xmlTransformFactory");

			if(xmlTransformationMapFactory == null)
				throw new ArgumentNullException("xmlTransformationMapFactory");

			this._xmlTransformationMapFactory = xmlTransformationMapFactory;
			this._xmlTransformFactory = xmlTransformFactory;
		}

		#endregion

		#region Properties

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

		protected internal virtual IValidationLog ValidationLog
		{
			get { return this._validationLog; }
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

		protected internal virtual IXmlTransformFactory XmlTransformFactory
		{
			get { return this._xmlTransformFactory; }
		}

		protected internal abstract XmlTransformMode XmlTransformMode { get; }

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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool Execute()
		{
			try
			{
				var xmlTransformationMapList = new List<IXmlTransformationMap>();
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach(var xmlTransformationMap in this.XmlTransformationMaps)
				{
					xmlTransformationMapList.Add(this.XmlTransformationMapFactory.Create(xmlTransformationMap, this.ValidationLog));
				}
				// ReSharper restore LoopCanBeConvertedToQuery

				var xmlFiles = this.GetXmlFiles(this.Files).ToArray();

				var xmlFilesToTransform = new List<IXmlFileToTransform>();
				// ReSharper disable LoopCanBeConvertedToQuery
				foreach(var file in xmlFiles)
				{
					xmlFilesToTransform.Add(this.XmlTransformFactory.Create(file, this.TransformName, this.XmlTransformMode, xmlTransformationMapList.ToArray(), this.ValidationLog));
				}
				// ReSharper restore LoopCanBeConvertedToQuery

				var filesToTransform = new List<ITaskItem>();
				//foreach(var xmlFile in xmlFiles)
				//{
				//	var xmlFileToTransform = xmlFilesToTransform.FirstOrDefault(x => xmlFile.ItemSpec.Equals(x.XmlFile.OriginalPath, StringComparison.OrdinalIgnoreCase));

				//	if(xmlFileToTransform != null)
				//	{
				//		xmlFile.SetMetadata("IsAppConfig", xmlFileToTransform.IsAppConfig.ToString());
				//		xmlFile.SetMetadata("", xmlFileToTransform.);
				//	}
				//}

				this.FilesToTransform = filesToTransform.ToArray();

				this.TransferValidationLog();

				return this.ValidationLog.ValidationMessages.All(validationMessage => !(validationMessage is ValidationError));
			}
			catch(Exception exception)
			{
				this.Log.LogError(exception.ToString());

				return false;
			}
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

			return xmlFiles;
		}

		protected internal virtual void LogValidationMessage(ValidationMessage validationMessage)
		{
			if(validationMessage == null)
				throw new ArgumentNullException("validationMessage");

			if(this.ValidationModeInternal == Validation.ValidationMode.Message)
			{
				this.Log.LogMessage(validationMessage.Information);
				return;
			}

			if(this.ValidationModeInternal == Validation.ValidationMode.Warning)
			{
				this.Log.LogWarning(validationMessage.Information);
				return;
			}

			if(validationMessage is ValidationError)
			{
				this.Log.LogError(validationMessage.Information);
				return;
			}

			this.Log.LogWarning(validationMessage.Information);
		}

		protected internal virtual void TransferValidationLog()
		{
			foreach(var validationMessage in this.ValidationLog.ValidationMessages)
			{
				this.LogValidationMessage(validationMessage);
			}
		}

		#endregion
	}
}