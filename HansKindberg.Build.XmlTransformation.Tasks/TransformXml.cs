using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.IoC;
using HansKindberg.Build.XmlTransformation.Tasks.Xdt;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	[CLSCompliant(false)]
	public class TransformXml : Task
	{
		#region Fields

		private readonly IFileSystem _fileSystem;
		private string _transformRootPath;
		private ITaskItem[] _transforms;
		private readonly IXmlTransformationFactory _xmlTransformationFactory;

		#endregion

		#region Constructors

		public TransformXml() : this(ServiceLocator.Instance.GetService<IFileSystem>(), ServiceLocator.Instance.GetService<IXmlTransformationFactory>()) {}

		public TransformXml(IFileSystem fileSystem, IXmlTransformationFactory xmlTransformationFactory)
		{
			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			if(xmlTransformationFactory == null)
				throw new ArgumentNullException("xmlTransformationFactory");

			this._fileSystem = fileSystem;
			this._xmlTransformationFactory = xmlTransformationFactory;
		}

		#endregion

		#region Properties

		[Required]
		public virtual ITaskItem Destination { get; set; }

		protected internal virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		[Required]
		public virtual ITaskItem Source { get; set; }

		public virtual string SourceRootPath { get; set; }
		public virtual bool StackTrace { get; set; }

		public virtual string TransformRootPath
		{
			get { return string.IsNullOrEmpty(this._transformRootPath) ? this.SourceRootPath : this._transformRootPath; }
			set { this._transformRootPath = value; }
		}

		[Required]
		public virtual ITaskItem[] Transforms
		{
			get { return this._transforms ?? (this._transforms = new ITaskItem[0]); }
			set { this._transforms = value; }
		}

		protected internal virtual IXmlTransformationFactory XmlTransformationFactory
		{
			get { return this._xmlTransformationFactory; }
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

				if(this.Destination == null || string.IsNullOrEmpty(this.Destination.ItemSpec))
					return true;

				if(this.Source == null || string.IsNullOrEmpty(this.Source.ItemSpec))
					return true;

				if(this.Transforms == null || !this.Transforms.Any())
					return true;

				for(int i = 0; i < this.Transforms.Count(); i++)
				{
					var source = i == 0 ? this.Source.ItemSpec : this.Destination.ItemSpec;
					var fullPathForSource = i == 0 ? this.GetFullPathForSource() : this.Destination.ItemSpec;

					using(var xmlTransformableDocument = this.XmlTransformationFactory.CreateXmlTransformableDocument(fullPathForSource))
					{
						var fullPathForTransform = this.GetFullPathForTransform(this.Transforms[i]);

						using(var xmlTransformation = this.XmlTransformationFactory.CreateXmlTransformation(fullPathForTransform))
						{
							xmlTransformation.Apply(xmlTransformableDocument);

							var destinationDirectory = this.FileSystem.FileInfo.FromFileName(this.Destination.ItemSpec).Directory;

							if(!destinationDirectory.Exists)
								destinationDirectory.Create();

							xmlTransformableDocument.Save(this.Destination.ItemSpec);

							this.Log.LogMessageFromText(string.Format(CultureInfo.InvariantCulture, "Transformed {0} using {1} into {2}.", source, fullPathForTransform, this.Destination), this.LogImportanceInternal);
						}
					}
				}
			}
			catch(Exception exception)
			{
				this.ValidationLog.LogError(exception.ToString());
			}

			return this.TransferValidationLog();
		}

		protected internal virtual string GetFullPathForFile(string root, ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(!this.FileSystem.Path.IsPathRooted(file.ItemSpec) && !string.IsNullOrEmpty(root))
				return this.FileSystem.Path.Combine(root, file.ItemSpec);

			return file.ItemSpec;
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		protected internal virtual string GetFullPathForSource()
		{
			return this.GetFullPathForFile(this.SourceRootPath, this.Source);
		}

		protected internal virtual string GetFullPathForTransform(ITaskItem transform)
		{
			return this.GetFullPathForFile(this.TransformRootPath, transform);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		[SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Build.Utilities.TaskLoggingHelper.LogError(System.String,System.Object[])")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TransformRootPath")]
		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SourceRootPath")]
		protected internal virtual bool Validate()
		{
			bool validate = true;

			if(this.Source != null && !string.IsNullOrEmpty(this.Source.ItemSpec))
			{
				try
				{
					this.FileSystem.FileInfo.FromFileName(this.Source.ItemSpec);
				}
				catch
				{
					this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"Source\" value \"{0}\" is invalid. A valid file-name is required.", this.Source));
					validate = false;
				}
			}

			if(!string.IsNullOrEmpty(this.SourceRootPath))
			{
				try
				{
					this.FileSystem.DirectoryInfo.FromDirectoryName(this.SourceRootPath);
				}
				catch
				{
					this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"SourceRootPath\" value \"{0}\" is invalid. A valid directory-name is required.", this.SourceRootPath));
					validate = false;
				}
			}

			if(this.Transforms != null && this.Transforms.Any())
			{
				foreach(var transform in this.Transforms)
				{
					try
					{
						this.FileSystem.FileInfo.FromFileName(transform.ItemSpec);
					}
					catch
					{
						this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"Transforms\" value is invalid. The item value \"{0}\" is invalid. A valid file-name is required.", transform));
						validate = false;
					}
				}
			}

			if(!string.IsNullOrEmpty(this.TransformRootPath))
			{
				try
				{
					this.FileSystem.DirectoryInfo.FromDirectoryName(this.TransformRootPath);
				}
				catch
				{
					this.Log.LogError(string.Format(CultureInfo.InvariantCulture, "The \"TransformRootPath\" value \"{0}\" is invalid. A valid directory-name is required.", this.TransformRootPath));
					validate = false;
				}
			}

			return validate;
		}

		#endregion
	}
}