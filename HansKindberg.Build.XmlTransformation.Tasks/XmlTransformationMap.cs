namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class XmlTransformationMap : IXmlTransformationMap
	{
		#region Properties

		public virtual IPotentialFile CommonBuildTransform { get; set; }
		public virtual IPotentialFile CommonPublishTransform { get; set; }
		public virtual IPotentialFile Source { get; set; }

		#endregion
	}
}