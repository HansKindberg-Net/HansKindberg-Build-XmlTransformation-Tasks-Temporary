using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Helpers
{
	public static class XmlTransformationMapHelper
	{
		#region Methods

		public static ITaskItem CreateXmlTransformationMap(string id, string generalBuildTransform, string generalPublishTransform, string source)
		{
			var xmlTransformationMap = new TaskItem(id);

			if(generalBuildTransform != null)
				xmlTransformationMap.SetMetadata("GeneralBuildTransform", generalBuildTransform);

			if(generalPublishTransform != null)
				xmlTransformationMap.SetMetadata("GeneralPublishTransform", generalPublishTransform);

			if(source != null)
				xmlTransformationMap.SetMetadata("Source", source);

			return xmlTransformationMap;
		}

		#endregion
	}
}