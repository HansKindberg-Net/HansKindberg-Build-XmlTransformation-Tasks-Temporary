using System;
using System.IO.Abstractions;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class PotentialFileFactory : IPotentialFileFactory
	{
		#region Fields

		private readonly IFileSystem _fileSystem;

		#endregion

		#region Constructors

		public PotentialFileFactory(IFileSystem fileSystem)
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

		#endregion

		#region Methods

		public virtual IPotentialFile Create(string fileName)
		{
			if(fileName == null)
				throw new ArgumentNullException("fileName");

			return new PotentialFile(fileName, this.FileSystem);
		}

		public virtual IPotentialFile Create(ITaskItem taskItem)
		{
			if(taskItem == null)
				throw new ArgumentNullException("taskItem");

			return new PotentialFile(taskItem, this.FileSystem);
		}

		#endregion
	}
}