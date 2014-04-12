using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationDecoratorFactory : IXmlTransformationDecoratorFactory
	{
		#region Fields

		private readonly IFileSystem _fileSystem;
		private readonly IEqualityComparer<ITaskItem> _taskItemComparer;

		#endregion

		#region Constructors

		public XmlTransformationDecoratorFactory(IFileSystem fileSystem, IEqualityComparer<ITaskItem> taskItemComparer)
		{
			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			if(taskItemComparer == null)
				throw new ArgumentNullException("taskItemComparer");

			this._fileSystem = fileSystem;
			this._taskItemComparer = taskItemComparer;
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

		#endregion

		#region Methods

		public virtual IXmlTransformationDecorator Create(IXmlTransformationSettings xmlTransformationSettings, IValidationLog validationLog)
		{
			return new XmlTransformationDecorator(this.FileSystem, xmlTransformationSettings, this.TaskItemComparer, validationLog);
		}

		#endregion
	}
}