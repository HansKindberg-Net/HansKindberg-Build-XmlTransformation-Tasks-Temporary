using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface ITransformableXmlFile : ITaskItem
	{
		#region Properties

		IPotentialFile Destination { get; }
		bool IsAppConfig { get; }
		IPotentialFile PreTransform { get; }
		IPotentialFile Source { get; }
		IPotentialFile Transform { get; }

		#endregion
	}
}