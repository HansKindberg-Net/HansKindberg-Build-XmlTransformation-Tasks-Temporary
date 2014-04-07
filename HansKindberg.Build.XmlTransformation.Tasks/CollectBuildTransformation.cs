using HansKindberg.Build.XmlTransformation.Tasks.IoC;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class CollectBuildTransformation : CollectXmlTransformation
	{
		#region Constructors

		public CollectBuildTransformation() : base(ServiceLocator.Instance.GetService<IXmlTransformFactory>(), ServiceLocator.Instance.GetService<IXmlTransformationMapFactory>()) {}

		#endregion

		#region Properties

		protected internal override XmlTransformMode XmlTransformMode
		{
			get { return Tasks.XmlTransformMode.Build; }
		}

		#endregion
	}
}