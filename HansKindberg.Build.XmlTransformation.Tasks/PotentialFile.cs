using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class PotentialFile : IPotentialFile
	{
		#region Fields

		private readonly FileInfoBase _fileInfo;
		private readonly ITaskItem _taskItem;

		#endregion

		#region Constructors

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public PotentialFile(ITaskItem taskItem, IFileSystem fileSystem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			this._taskItem = taskItem;

			try
			{
				this._fileInfo = fileSystem.FileInfo.FromFileName(taskItem.ToString());
			}
			catch
			{
				this._fileInfo = null;
			}
		}

		#endregion

		#region Properties

		public virtual bool Exists
		{
			get { return this.FileInfo != null && this.FileInfo.Exists; }
		}

		public virtual string Extension
		{
			get { return this.FileInfo == null ? string.Empty : this.FileInfo.Extension; }
		}

		protected internal virtual FileInfoBase FileInfo
		{
			get { return this._fileInfo; }
		}

		public virtual string FullName
		{
			get { return this.FileInfo == null ? string.Empty : this.FileInfo.FullName; }
		}

		public virtual bool IsFile
		{
			get { return this.FileInfo != null; }
		}

		public virtual string Name
		{
			get { return this.FileInfo == null ? string.Empty : this.FileInfo.Name; }
		}

		public virtual string OriginalPath
		{
			get { return this.TaskItem.ToString(); }
		}

		public virtual ITaskItem TaskItem
		{
			get { return this._taskItem; }
		}

		#endregion

		#region Methods

		public override string ToString()
		{
			return this.TaskItem.ToString();
		}

		#endregion
	}
}