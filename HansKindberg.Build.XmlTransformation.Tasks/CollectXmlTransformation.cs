using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.Extensions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public abstract class CollectXmlTransformation : Task
	{
		#region Fields

		private readonly IFileSystem _fileSystem;
		private ITaskItem[] _files;
		private ITaskItem[] _filesToTransform;
		private MessageImportance _logImportance = MessageImportance.Normal;
		private string _transformName;
		private readonly IValidationLog _validationLog = new ValidationLog();
		private ValidationMode _validationMode = Validation.ValidationMode.Warning;
		private ITaskItem[] _xmlFileExtensions;
		private readonly IXmlTransformationDecoratorFactory _xmlTransformationDecoratorFactory;
		private ITaskItem[] _xmlTransformationMaps;

		#endregion

		#region Constructors

		protected CollectXmlTransformation(IFileSystem fileSystem, IXmlTransformationDecoratorFactory xmlTransformationDecoratorFactory)
		{
			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			if(xmlTransformationDecoratorFactory == null)
				throw new ArgumentNullException("xmlTransformationDecoratorFactory");

			this._fileSystem = fileSystem;
			this._xmlTransformationDecoratorFactory = xmlTransformationDecoratorFactory;
		}

		#endregion

		#region Properties

		public virtual string AppConfigDestinationDirectory { get; set; }
		public virtual string DestinationDirectory { get; set; }
		public virtual bool ExcludeFilesDependentUpon { get; set; }
		public virtual bool ExcludeFilesDependentUponByFileName { get; set; }

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

		public virtual string LogImportance
		{
			get { return this.LogImportanceInternal.ToString(); }
			set { this.LogImportanceInternal = (MessageImportance) Enum.Parse(typeof(MessageImportance), value, true); }
		}

		protected internal virtual MessageImportance LogImportanceInternal
		{
			get { return this._logImportance; }
			set { this._logImportance = value; }
		}

		protected internal abstract TransformMode TransformMode { get; }

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

		protected internal virtual IXmlTransformationDecoratorFactory XmlTransformationDecoratorFactory
		{
			get { return this._xmlTransformationDecoratorFactory; }
		}

		public virtual ITaskItem[] XmlTransformationMaps
		{
			get { return this._xmlTransformationMaps ?? (this._xmlTransformationMaps = new ITaskItem[0]); }
			set { this._xmlTransformationMaps = value; }
		}

		#endregion

		#region Methods

		protected internal virtual IXmlTransformationSettings CreateXmlTransformationSettings()
		{
			return new XmlTransformationSettings
			{
				AppConfigDestinationDirectory = this.AppConfigDestinationDirectory,
				DestinationDirectory = this.DestinationDirectory,
				ExcludeFilesDependentUpon = this.ExcludeFilesDependentUpon,
				ExcludeFilesDependentUponByFileName = this.ExcludeFilesDependentUponByFileName,
				TransformMode = this.TransformMode,
				TransformName = this.TransformName,
				XmlFileExtensions = this.XmlFileExtensions.Select(xmlFileExtension => xmlFileExtension.ItemSpec).ToArray(),
				XmlTransformationMaps = this.XmlTransformationMaps
			};
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool Execute()
		{
			try
			{
				if(!this.Validate())
					return false;

				if(!this.Files.Any())
					return true;

				var xmlTransformationSettings = this.CreateXmlTransformationSettings();

				var xmlTransformationDecorator = this.XmlTransformationDecoratorFactory.Create(xmlTransformationSettings, this.ValidationLog);

				this.FilesToTransform = xmlTransformationDecorator.GetDecoratedFiles(this.Files).ToArray();

				this.TransferValidationLog();

				this.LogInformation(xmlTransformationSettings, xmlTransformationDecorator);
			}
			catch(Exception exception)
			{
				this.Log.LogError(exception.ToString());
				return false;
			}

			var thereAreValidationErrors = this.ValidationLog.ValidationMessages.All(validationMessage => !(validationMessage is ValidationError));

			return !(thereAreValidationErrors && this.ValidationModeInternal == Validation.ValidationMode.Error);
		}

		protected internal virtual string GetUnderliningForText(char underliningCharacter, string text)
		{
			var underlining = string.Empty;

			if(string.IsNullOrEmpty(text))
				return underlining;

			for(int i = 0; i < text.Length; i++)
			{
				underlining += underliningCharacter.ToString(CultureInfo.InvariantCulture);
			}

			return underlining;
		}

		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "HansKindberg.Build.XmlTransformation.Tasks.CollectXmlTransformation.GetUnderliningForText(System.Char,System.String)")]
		protected internal virtual void LogInformation(IXmlTransformationSettings xmlTransformationSettings, IXmlFileFilter xmlFileFilter)
		{
			if(xmlFileFilter == null)
				throw new ArgumentNullException("xmlFileFilter");

			var log = new List<string>
			{
				string.Empty
			};

			var heading = string.Format(CultureInfo.InvariantCulture, "Information from \"{0}\"", this.GetType().FullName);
			log.Add(heading);
			log.Add(this.GetUnderliningForText('=', heading));
			log.Add(string.Empty);
			const string settings = "Settings";
			log.Add(settings);
			log.Add(this.GetUnderliningForText('-', settings));
			if(xmlTransformationSettings != null)
			{
				log.Add(string.Format(CultureInfo.InvariantCulture, " - AppConfigDestinationDirectory = '{0}'", xmlTransformationSettings.AppConfigDestinationDirectory));
				log.Add(string.Format(CultureInfo.InvariantCulture, " - DestinationDirectory = '{0}'", xmlTransformationSettings.DestinationDirectory));
				log.Add(string.Format(CultureInfo.InvariantCulture, " - ExcludeFilesDependentUpon = '{0}'", xmlTransformationSettings.ExcludeFilesDependentUpon));
				log.Add(string.Format(CultureInfo.InvariantCulture, " - ExcludeFilesDependentUponByFileName = '{0}'", xmlTransformationSettings.ExcludeFilesDependentUponByFileName));
				log.Add(string.Format(CultureInfo.InvariantCulture, " - TransformMode = '{0}'", xmlTransformationSettings.TransformMode));
				log.Add(string.Format(CultureInfo.InvariantCulture, " - TransformName = '{0}'", xmlTransformationSettings.TransformName));
				log.Add(string.Format(CultureInfo.InvariantCulture, " - XmlFileExtensions = '{0}'", string.Join(", ", xmlTransformationSettings.XmlFileExtensions.ToArray())));
				log.Add(" - XmlTransformationMaps:");
				if(xmlTransformationSettings.XmlTransformationMaps.Any())
				{
					foreach(var xmlTransformationMap in xmlTransformationSettings.XmlTransformationMaps)
					{
						log.Add(string.Format(CultureInfo.InvariantCulture, "   - XmlTransformationMap = '{0}'", xmlTransformationMap));
						log.Add(string.Format(CultureInfo.InvariantCulture, "     - {0} = '{1}'", XmlTransformationMap.GeneralBuildTransformMetadataName, xmlTransformationMap.GetMetadata(XmlTransformationMap.GeneralBuildTransformMetadataName)));
						log.Add(string.Format(CultureInfo.InvariantCulture, "     - {0} = '{1}'", XmlTransformationMap.GeneralPublishTransformMetadataName, xmlTransformationMap.GetMetadata(XmlTransformationMap.GeneralPublishTransformMetadataName)));
						log.Add(string.Format(CultureInfo.InvariantCulture, "     - {0} = '{1}'", XmlTransformationMap.SourceMetadataName, xmlTransformationMap.GetMetadata(XmlTransformationMap.SourceMetadataName)));
					}
				}
				else
				{
					log.Add("   - NONE");
				}
			}
			log.Add(string.Empty);
			const string files = "Incoming xml-files";
			log.Add(files);
			log.Add(this.GetUnderliningForText('-', files));
			var xmlFiles = xmlFileFilter.GetXmlFiles(this.Files).ToArray();
			if(xmlFiles.Any())
				log.AddRange(xmlFiles.Select(xmlFile => string.Format(CultureInfo.InvariantCulture, " - '{0}'", xmlFile)));
			else
				log.Add(" - NONE");

			log.Add(string.Empty);
			const string filesToTransform = "Files to transform";
			log.Add(filesToTransform);
			log.Add(this.GetUnderliningForText('-', filesToTransform));
			if(this.FilesToTransform.Any())
			{
				foreach(var fileToTransform in this.FilesToTransform)
				{
					log.Add(string.Format(CultureInfo.InvariantCulture, " - '{0}'", fileToTransform));
					log.Add(string.Format(CultureInfo.InvariantCulture, "   - {0} = '{1}'", XmlTransformationDecoratedTaskItemExtension.DestinationMetadataName, fileToTransform.GetMetadata(XmlTransformationDecoratedTaskItemExtension.DestinationMetadataName)));
					log.Add(string.Format(CultureInfo.InvariantCulture, "   - {0} = '{1}'", XmlTransformationDecoratedTaskItemExtension.IsAppConfigMetadataName, fileToTransform.GetMetadata(XmlTransformationDecoratedTaskItemExtension.IsAppConfigMetadataName)));
					log.Add(string.Format(CultureInfo.InvariantCulture, "   - {0} = '{1}'", XmlTransformationDecoratedTaskItemExtension.ObjectiveMetadataName, fileToTransform.GetMetadata(XmlTransformationDecoratedTaskItemExtension.ObjectiveMetadataName)));
					log.Add(string.Format(CultureInfo.InvariantCulture, "   - {0} = '{1}'", XmlTransformationDecoratedTaskItemExtension.TransformMetadataName, fileToTransform.GetMetadata(XmlTransformationDecoratedTaskItemExtension.TransformMetadataName)));
				}
			}
			else
			{
				log.Add(" - NONE");
			}
			log.Add(string.Empty);

			this.Log.LogMessageFromText(string.Join(Environment.NewLine, log.ToArray()), this.LogImportanceInternal);
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
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TransformName")]
		protected internal virtual bool Validate()
		{
			bool validate = true;

			if(!string.IsNullOrEmpty(this.AppConfigDestinationDirectory))
			{
				try
				{
					this.FileSystem.DirectoryInfo.FromDirectoryName(this.AppConfigDestinationDirectory);
				}
				catch
				{
					this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"AppConfigDestinationDirectory\" value \"{0}\" is invalid. A valid directory-name is required.", this.AppConfigDestinationDirectory));
					validate = false;
				}
			}

			if(!string.IsNullOrEmpty(this.DestinationDirectory))
			{
				try
				{
					this.FileSystem.DirectoryInfo.FromDirectoryName(this.DestinationDirectory);
				}
				catch
				{
					this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"DestinationDirectory\" value \"{0}\" is invalid. A valid directory-name is required.", this.DestinationDirectory));
					validate = false;
				}
			}

			try
			{
				this.FileSystem.FileInfo.FromFileName(this.TransformName);
			}
			catch
			{
				this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"TransformName\" value \"{0}\" is invalid. A valid file-name is required.", this.TransformName));
				validate = false;
			}

			return validate;
		}

		#endregion
	}
}