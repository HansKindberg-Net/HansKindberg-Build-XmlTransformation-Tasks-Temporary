using HansKindberg.Build.XmlTransformation.Tasks.IoC;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public class CollectPublishTransformation : CollectXmlTransformation
	{
		#region Constructors

		public CollectPublishTransformation() : this(ServiceLocator.Instance.GetService<IXmlTransformationContext>()) {}
		public CollectPublishTransformation(IXmlTransformationContext xmlTransformationContext) : base(xmlTransformationContext) {}

		#endregion

		#region Properties

		protected internal override XmlTransformMode XmlTransformMode
		{
			get { return Tasks.XmlTransformMode.Publish; }
		}

		#endregion
	}
}