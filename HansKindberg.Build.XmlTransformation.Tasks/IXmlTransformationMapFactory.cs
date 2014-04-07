using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationMapFactory
	{
		#region Methods

		IXmlTransformationMap Create(ITaskItem xmlTransformationMapTaskItem, IValidationLog validationLog);

		#endregion
	}
}