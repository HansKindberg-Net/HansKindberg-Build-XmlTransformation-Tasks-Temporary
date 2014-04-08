using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
		private readonly IXmlTransformationContext _xmlTransformationContext;
		private ITaskItem[] _xmlTransformationMaps;

		#endregion

		#region Constructors

		protected CollectXmlTransformation(IXmlTransformationContext xmlTransformationContext)
		{
			if(xmlTransformationContext == null)
				throw new ArgumentNullException("xmlTransformationContext");

			this._xmlTransformationContext = xmlTransformationContext;
		}

		#endregion

		#region Properties

		public virtual string AppConfigDestinationDirectory { get; set; }
		public virtual string DestinationDirectory { get; set; }

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

		public virtual bool SeparateSourceIsRequired { get; set; }

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

		protected internal abstract XmlTransformMode XmlTransformMode { get; }

		protected internal virtual IXmlTransformationContext XmlTransformationContext
		{
			get { return this._xmlTransformationContext; }
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
				if(!this.Validate())
					return false;

				if(!this.Files.Any())
					return true;

				this.XmlTransformationContext.XmlFileExtensions = this.XmlFileExtensions;
				this.XmlTransformationContext.XmlTransformationMaps = this.XmlTransformationMaps;

				var transformableXmlFiles = this.XmlTransformationContext.GetTransformableXmlFiles(this.Files, this.TransformName, this.XmlTransformMode, this.ValidationLog);

				transformableXmlFiles = transformableXmlFiles;

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
			}
			catch(Exception exception)
			{
				this.Log.LogError(exception.ToString());
				return false;
			}

			var thereAreValidationErrors = this.ValidationLog.ValidationMessages.All(validationMessage => !(validationMessage is ValidationError));

			return !(thereAreValidationErrors && this.ValidationModeInternal == Validation.ValidationMode.Error);
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Build.Utilities.TaskLoggingHelper.LogError(System.String,System.Object[])")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AppConfigDestinationDirectory")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "DestinationDirectory")]
		protected internal virtual bool Validate()
		{
			bool validate = true;

			if(!string.IsNullOrEmpty(this.AppConfigDestinationDirectory))
			{
				try
				{
					this.XmlTransformationContext.FileSystem.DirectoryInfo.FromDirectoryName(this.AppConfigDestinationDirectory);
				}
				catch(Exception exception)
				{
					this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"AppConfigDestinationDirectory\" value \"{0}\" is invalid. {1}", this.AppConfigDestinationDirectory, exception));
					validate = false;
				}
			}

			if(!string.IsNullOrEmpty(this.DestinationDirectory))
			{
				try
				{
					this.XmlTransformationContext.FileSystem.DirectoryInfo.FromDirectoryName(this.DestinationDirectory);
				}
				catch(Exception exception)
				{
					this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"DestinationDirectory\" value \"{0}\" is invalid. {1}", this.DestinationDirectory, exception));
					validate = false;
				}
			}

			return validate;
		}

		#endregion
	}
}