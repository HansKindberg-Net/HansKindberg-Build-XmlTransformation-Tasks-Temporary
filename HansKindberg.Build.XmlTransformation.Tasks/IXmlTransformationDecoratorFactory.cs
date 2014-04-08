using HansKindberg.Build.XmlTransformation.Tasks.Validation;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationDecoratorFactory
	{
		#region Methods

		IXmlTransformationDecorator Create(IXmlTransformationSettings xmlTransformationSettings, IValidationLog validationLog);

		#endregion
	}
}