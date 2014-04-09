using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationMap : ITaskItem
	{
		#region Properties

		string CommonBuildTransform { get; }
		string CommonPublishTransform { get; }
		string Source { get; }

		#endregion
	}
}