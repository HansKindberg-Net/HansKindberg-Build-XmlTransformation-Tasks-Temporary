using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.Framework.Extensions;
using HansKindberg.Build.XmlTransformation.Tasks.Extensions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationDecorator : IXmlTransformationDecorator, IXmlTransformationMapRepository
	{
		#region Fields

		private readonly IFileSystem _fileSystem;
		private readonly IEqualityComparer<ITaskItem> _taskItemComparer;
		private readonly IValidationLog _validationLog;
		private readonly IDictionary<string, IXmlTransformationMap> _xmlTransformationMapCache = new Dictionary<string, IXmlTransformationMap>(StringComparer.OrdinalIgnoreCase);
		private readonly IXmlTransformationSettings _xmlTransformationSettings;

		#endregion

		#region Constructors

		public XmlTransformationDecorator(IFileSystem fileSystem, IXmlTransformationSettings xmlTransformationSettings, IEqualityComparer<ITaskItem> taskItemComparer, IValidationLog validationLog)
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

			if(taskItemComparer == null)
				throw new ArgumentNullException("taskItemComparer");

			if(validationLog == null)
				throw new ArgumentNullException("validationLog");

			this._fileSystem = fileSystem;
			this._taskItemComparer = taskItemComparer;
			this._validationLog = validationLog;
			this._xmlTransformationSettings = xmlTransformationSettings;
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem
		{
			get { return this._fileSystem; }
		}

		protected internal virtual IEqualityComparer<ITaskItem> TaskItemComparer
		{
			get { return this._taskItemComparer; }
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

		protected internal virtual IEnumerable<ITaskItem> GetDecoratedFiles(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(file.Objective() == null)
				file.Objective(file.ItemSpec);

			var validationResult = new ValidationResult();
			var transformValidationResult = new ValidationResult();
			var decoratedFiles = new List<ITaskItem>();

			var validatedObjective = this.GetValidatedObjective(file);

			if(!validatedObjective.IsValid)
			{
				validationResult.Exceptions.Add(validatedObjective.Exception);
			}
			else
			{
				var validatedGeneralTransform = this.GetValidatedGeneralTransform(file);

				if(!validatedGeneralTransform.IsValid)
					transformValidationResult.Exceptions.Add(validatedGeneralTransform.Exception);

				var validatedTransform = this.GetValidatedTransform(file);

				if((validatedGeneralTransform.IsValid && !string.IsNullOrEmpty(validatedGeneralTransform.Value)) || validatedTransform.IsValid)
				{
					var validatedDestination = this.GetValidatedDestination(file);

					if(!validatedDestination.IsValid)
						validationResult.Exceptions.Add(validatedDestination.Exception);

					var validatedSource = this.GetValidatedSource(file);

					if(!validatedSource.IsValid)
						validationResult.Exceptions.Add(validatedSource.Exception);

					if(validatedDestination.IsValid && validatedSource.IsValid)
					{
						var source = validatedSource.Value;

						if(string.IsNullOrEmpty(source))
							source = file.ItemSpec;

						var destinationValidation = this.ValidateDestination(file, validatedDestination.Value, source);

						if(!destinationValidation.IsValid)
						{
							validationResult.AddExceptions(destinationValidation.Exceptions);
						}
						else
						{
							if(!source.Equals(file.ItemSpec))
								file.ItemSpec = source;

							file.Destination(validatedDestination.Value);
							file.FirstTransform(string.Empty);
							file.GeneralTransform(string.Empty);
							file.LastTransform(string.Empty);
							file.Transform(string.Empty);

							if(validatedGeneralTransform.IsValid && !string.IsNullOrEmpty(validatedGeneralTransform.Value))
								file.GeneralTransform(validatedGeneralTransform.Value);

							if(validatedTransform.IsValid)
								file.Transform(validatedTransform.Value);

							if(!string.IsNullOrEmpty(file.GeneralTransform()) && !string.IsNullOrEmpty(file.Transform()))
							{
								file.FirstTransform(file.GeneralTransform());
								file.LastTransform(file.Transform());
							}
							else
							{
								file.FirstTransform(!string.IsNullOrEmpty(file.GeneralTransform()) ? file.GeneralTransform() : file.Transform());
							}

							decoratedFiles.Add(file);
						}
					}
				}
			}

			this.Log(transformValidationResult);
			this.Log(file, validationResult);

			return decoratedFiles.ToArray();
		}

		public virtual IEnumerable<ITaskItem> GetDecoratedFiles(IEnumerable<ITaskItem> files)
		{
			if(files == null)
				throw new ArgumentNullException("files");

			// ReSharper disable LoopCanBeConvertedToQuery
			foreach(var file in this.GetDistinctFilesForDecoration(files))
			{
				foreach(var decoratedFile in this.GetDecoratedFiles(file))
				{
					yield return decoratedFile;
				}
			}
			// ReSharper restore LoopCanBeConvertedToQuery
		}

		protected internal virtual IEnumerable<ITaskItem> GetDistinctFilesForDecoration(IEnumerable<ITaskItem> files)
		{
			if(files == null)
				throw new ArgumentNullException("files");

			var filesCopy = files.ToArray();
			var allFiles = filesCopy.ToArray();

			var includedXmlFiles = new List<ITaskItem>();

			// ReSharper disable LoopCanBeConvertedToQuery
			foreach(var file in filesCopy)
			{
				if(!this.IsXmlFile(file))
					continue;

				if(this.XmlTransformationSettings.ExcludeFilesDependentUpon && this.IsDependentUpon(file))
					continue;

				if(this.XmlTransformationSettings.ExcludeFilesDependentUponByFileName && this.IsDependentUponByFileName(file, allFiles))
					continue;

				includedXmlFiles.Add(file);
			}
			// ReSharper restore LoopCanBeConvertedToQuery

			var includedObjectives = includedXmlFiles.Where(file => file.Objective() != null).Select(file => file.Objective()).Distinct(this.TaskItemComparer);

			var distinctFilesForDecoration = includedXmlFiles.Where(file => !includedObjectives.Contains(file, this.TaskItemComparer)).Distinct(this.TaskItemComparer);

			return distinctFilesForDecoration.ToArray();
		}

		protected internal virtual string GetFullPathWithoutExtension(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			return this.FileSystem.Path.Combine(this.FileSystem.Path.GetDirectoryName(file.FullPath()), this.FileSystem.Path.GetFileNameWithoutExtension(file.FullPath()));
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		protected internal virtual IValidatable<string> GetValidatedDestination(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			var validatedDestination = new Validatable<string>();

			bool isAppConfig = file.IsAppConfig();

			string destinationDirectory = (!isAppConfig ? this.XmlTransformationSettings.DestinationDirectory : this.XmlTransformationSettings.AppConfigDestinationDirectory) ?? string.Empty;

			var destinationRelativePath = file.DestinationRelativePath();

			if(string.IsNullOrEmpty(destinationRelativePath))
				destinationRelativePath = file.Objective().ItemSpec;

			var destination = this.FileSystem.Path.Combine(destinationDirectory, destinationRelativePath);
			validatedDestination.Value = destination;

			try
			{
				this.FileSystem.FileInfo.FromFileName(destination);
			}
			catch
			{
				validatedDestination.Exception = new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "The destination \"{0}\" for file \"{1}\" is invalid.", destination, file));
			}

			return validatedDestination;
		}

		protected internal virtual IValidatable<string> GetValidatedGeneralTransform(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			var validatedGeneralTransform = new Validatable<string>();

			var objective = file.Objective();

			var xmlTransformationMap = this.GetXmlTransformationMap(objective);

			if(xmlTransformationMap == null)
				return validatedGeneralTransform;

			var generalTransform = this.XmlTransformationSettings.TransformMode == TransformMode.Build ? xmlTransformationMap.GeneralBuildTransform : xmlTransformationMap.GeneralPublishTransform;

			if(generalTransform == null)
				return validatedGeneralTransform;

			if(string.IsNullOrEmpty(generalTransform.ItemSpec))
				return validatedGeneralTransform;

			validatedGeneralTransform.Value = generalTransform.ItemSpec;

			if(!this.FileSystem.File.Exists(generalTransform.ItemSpec))
				validatedGeneralTransform.Exception = new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "The general transform file \"{0}\" for file \"{1}\" does not exist. {2}", generalTransform, file, this.GetXmlTransformationMapInformation(objective)), generalTransform.ItemSpec);

			return validatedGeneralTransform;
		}

		protected internal virtual IValidatable<ITaskItem> GetValidatedObjective(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			const string exceptionMessageSuffix = "This is not supposed to happen. There is a bug somewhere.";

			var objective = file.Objective();

			var validatedObjective = new Validatable<ITaskItem>
			{
				Value = objective
			};

			if(objective == null)
				validatedObjective.Exception = new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The objective is null. {0}", exceptionMessageSuffix));
			else if(string.IsNullOrEmpty(objective.ItemSpec))
				validatedObjective.Exception = new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The objective is empty. {0}", exceptionMessageSuffix));

			return validatedObjective;
		}

		protected internal virtual IValidatable<string> GetValidatedSource(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			var validatedSource = new Validatable<string>();

			var xmlTransformationMap = this.GetXmlTransformationMap(file);

			if(xmlTransformationMap == null)
				return validatedSource;

			var source = xmlTransformationMap.Source;

			if(source == null)
				return validatedSource;

			if(string.IsNullOrEmpty(source.ItemSpec))
				return validatedSource;

			validatedSource.Value = source.ItemSpec;

			if(!this.FileSystem.File.Exists(source.ItemSpec))
				validatedSource.Exception = new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" for file \"{1}\" does not exist. {2}", source, file, this.GetXmlTransformationMapInformation(file)), source.ItemSpec);

			return validatedSource;
		}

		protected internal virtual IValidatable<string> GetValidatedTransform(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			var validatedTransform = new Validatable<string>();

			var objective = file.Objective();

			var transform = this.FileSystem.Path.GetFileNameWithoutExtension(objective.ItemSpec) + "." + (this.XmlTransformationSettings.TransformName ?? string.Empty) + objective.Extension();
			validatedTransform.Value = transform;

			if(!this.FileSystem.File.Exists(transform))
				validatedTransform.Exception = new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "The transform file \"{0}\" for file \"{1}\" does not exist.", transform, file), transform);

			return validatedTransform;
		}

		public virtual IEnumerable<ITaskItem> GetXmlFiles(IEnumerable<ITaskItem> files)
		{
			if(files == null)
				throw new ArgumentNullException("files");

			return files.Where(this.IsXmlFile);
		}

		public virtual IXmlTransformationMap GetXmlTransformationMap(ITaskItem file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			IXmlTransformationMap xmlTransformationMap;

			if(!this.XmlTransformationMapCache.TryGetValue(file.ItemSpec, out xmlTransformationMap))
			{
				var objectiveFullPath = file.FullPath();

				var xmlTransformationMapTaskItems = new List<ITaskItem>();

				xmlTransformationMapTaskItems.AddRange(this.XmlTransformationSettings.XmlTransformationMaps.Where(taskItem => file.ItemSpec.Equals(taskItem.ItemSpec, StringComparison.OrdinalIgnoreCase) || objectiveFullPath.Equals(taskItem.ItemSpec, StringComparison.OrdinalIgnoreCase)));

				if(xmlTransformationMapTaskItems.Count > 1)
					this.LogXmlTransformationMapWarning(string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" has multiple XmlTransformationMaps. The first will be used.", file), file);

				var xmlTransformationMapTaskItem = xmlTransformationMapTaskItems.FirstOrDefault();

				if(xmlTransformationMapTaskItem != null)
				{
					var newXmlTransformationMap = new XmlTransformationMap(xmlTransformationMapTaskItem);

					xmlTransformationMap = newXmlTransformationMap;
				}

				this.XmlTransformationMapCache.Add(file.ItemSpec, xmlTransformationMap);
			}

			return xmlTransformationMap;
		}

		protected internal virtual string GetXmlTransformationMapInformation(ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return string.Format(CultureInfo.InvariantCulture, "See the \"XmlTransformationMap\"-itemgroup with identity \"{0}\".", taskItem);
		}

		protected internal virtual bool IsDependentUpon(ITaskItem file)
		{
			if(file == null)
				return false;

			return !string.IsNullOrEmpty(file.DependentUpon());
		}

		protected internal virtual bool IsDependentUponByFileName(ITaskItem file, IEnumerable<ITaskItem> files)
		{
			if(file == null)
				return false;

			if(files == null)
				return false;

			var fileFullPathWithoutExtension = this.GetFullPathWithoutExtension(file);

			// ReSharper disable LoopCanBeConvertedToQuery
			foreach(var fileItem in files)
			{
				if(!file.Extension().Equals(fileItem.Extension(), StringComparison.OrdinalIgnoreCase))
					continue;

				var fileItemFullPathWithoutExtension = this.GetFullPathWithoutExtension(fileItem);

				if(fileFullPathWithoutExtension.StartsWith(fileItemFullPathWithoutExtension, StringComparison.OrdinalIgnoreCase) && fileFullPathWithoutExtension.Length > fileItemFullPathWithoutExtension.Length)
				{
					var reminder = fileFullPathWithoutExtension.Substring(fileItemFullPathWithoutExtension.Length);

					if(reminder.StartsWith(".", StringComparison.OrdinalIgnoreCase))
						return true;
				}
			}
			// ReSharper restore LoopCanBeConvertedToQuery

			return false;
		}

		protected internal virtual bool IsXmlFile(ITaskItem file)
		{
			if(file == null)
				return false;

			var extension = file.Extension();

			return extension != null && this.XmlTransformationSettings.XmlFileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
		}

		protected internal virtual void Log(IValidationResult validationResult)
		{
			if(validationResult == null)
				throw new ArgumentNullException("validationResult");

			foreach(var exception in validationResult.Exceptions)
			{
				this.ValidationLog.LogWarning(exception.Message);
			}
		}

		protected internal virtual void Log(ITaskItem file, IValidationResult validationResult)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if(validationResult == null)
				throw new ArgumentNullException("validationResult");

			if(validationResult.IsValid)
				return;

			var log = new List<string>
			{
				string.Format(CultureInfo.InvariantCulture, "The file \"{0}\" will not be transformed:", file)
			};

			for(int i = 0; i < validationResult.Exceptions.Count(); i++)
			{
				log.Add(string.Format(CultureInfo.InvariantCulture, "{0}. {1}", i + 1, validationResult.Exceptions.ElementAt(i).Message));
			}

			this.ValidationLog.LogWarning(string.Join(" ", log.ToArray()));
		}

		protected internal virtual void LogXmlTransformationMapWarning(string information, ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			this.ValidationLog.LogWarning(string.Format(CultureInfo.InvariantCulture, "{0} {1}", information, this.GetXmlTransformationMapInformation(taskItem)));
		}

		protected internal virtual IValidationResult ValidateDestination(ITaskItem file, string destination, string source)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			var validationResult = new ValidationResult();

			var destinationFullPath = this.FileSystem.Path.GetFullPath(destination);
			var sourceFullPath = this.FileSystem.Path.GetFullPath(source);

			if(destinationFullPath.Equals(sourceFullPath, StringComparison.OrdinalIgnoreCase))
				validationResult.Exceptions.Add(new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The destination for file \"{0}\" can not be the same as the source \"{1}\".", file, source)));

			return validationResult;
		}

		#endregion
	}
}