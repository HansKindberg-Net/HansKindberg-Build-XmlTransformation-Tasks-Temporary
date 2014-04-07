using System.IO.Abstractions;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IPotentialFileFactory
	{
		#region Properties

		IFileSystem FileSystem { get; }

		#endregion

		#region Methods

		IPotentialFile Create(string fileName);
		IPotentialFile Create(ITaskItem taskItem);

		#endregion
	}
}