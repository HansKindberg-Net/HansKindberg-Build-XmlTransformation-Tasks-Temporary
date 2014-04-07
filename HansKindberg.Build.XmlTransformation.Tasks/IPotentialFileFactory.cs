using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IPotentialFileFactory
	{
		#region Methods

		IPotentialFile Create(string fileName);
		IPotentialFile Create(ITaskItem taskItem);

		#endregion
	}
}