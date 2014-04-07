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
		private readonly string _originalPath;

		#endregion

		#region Constructors

		public PotentialFile(string fileName, IFileSystem fileSystem) : this(fileName, null, true, fileSystem) {}
		public PotentialFile(ITaskItem taskItem, IFileSystem fileSystem) : this(null, taskItem, false, fileSystem) {}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private PotentialFile(string fileName, ITaskItem taskItem, bool useFileName, IFileSystem fileSystem)
		{
			if(useFileName && fileName == null)
				throw new ArgumentNullException("fileName");

			if(!useFileName && taskItem == null)
				throw new ArgumentNullException("taskItem");

			if(!useFileName && taskItem.ItemSpec == null)
				throw new ArgumentException("The item-spec can not be null.", "taskItem");

			if(fileSystem == null)
				throw new ArgumentNullException("fileSystem");

			this._originalPath = useFileName ? fileName : taskItem.ItemSpec;

			try
			{
				this._fileInfo = fileSystem.FileInfo.FromFileName(this._originalPath);
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
			get { return this._originalPath; }
		}

		#endregion
	}
}