using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationMap : ITaskItem
	{
		#region Properties

		ITaskItem GeneralBuildTransform { get; }
		ITaskItem GeneralPublishTransform { get; }
		ITaskItem Source { get; }

		#endregion
	}
}