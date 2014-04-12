using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationSettings : IXmlTransformationSettings
	{
		#region Properties

		public virtual string AppConfigDestinationDirectory { get; set; }
		public virtual string DestinationDirectory { get; set; }
		public virtual bool ExcludeFilesDependentUpon { get; set; }
		public virtual bool ExcludeFilesDependentUponByFileName { get; set; }
		public virtual TransformMode TransformMode { get; set; }
		public virtual string TransformName { get; set; }
		public virtual IEnumerable<string> XmlFileExtensions { get; set; }
		public virtual IEnumerable<ITaskItem> XmlTransformationMaps { get; set; }

		#endregion
	}
}