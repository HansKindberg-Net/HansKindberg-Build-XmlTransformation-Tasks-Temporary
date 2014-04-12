using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationMapRepository
	{
		#region Methods

		IXmlTransformationMap GetXmlTransformationMap(ITaskItem file);

		#endregion
	}
}