using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IPotentialFileFactory
	{
		#region Methods

		IPotentialFile CreatePotentialFile(ITaskItem taskItem);

		#endregion
	}
}