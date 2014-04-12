using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationSettings
	{
		#region Properties

		string AppConfigDestinationDirectory { get; }
		string DestinationDirectory { get; }
		bool ExcludeFilesDependentUpon { get; }
		bool ExcludeFilesDependentUponByFileName { get; }
		TransformMode TransformMode { get; }
		string TransformName { get; }
		IEnumerable<string> XmlFileExtensions { get; }
		IEnumerable<ITaskItem> XmlTransformationMaps { get; }

		#endregion
	}
}