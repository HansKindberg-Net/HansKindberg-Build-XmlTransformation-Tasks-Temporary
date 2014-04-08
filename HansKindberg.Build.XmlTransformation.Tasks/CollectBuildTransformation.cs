using HansKindberg.Build.XmlTransformation.Tasks.IoC;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class CollectBuildTransformation : CollectXmlTransformation
	{
		#region Constructors

		public CollectBuildTransformation() : this(ServiceLocator.Instance.GetService<IXmlTransformationContext>()) {}
		public CollectBuildTransformation(IXmlTransformationContext xmlTransformationContext) : base(xmlTransformationContext) {}

		#endregion

		#region Properties

		protected internal override XmlTransformMode XmlTransformMode
		{
			get { return Tasks.XmlTransformMode.Build; }
		}

		#endregion
	}
}