using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationMapRepository
	{
		#region Methods

		IXmlTransformationMap GetXmlTransformationMap(ITaskItem xmlFile, IValidationLog validationLog);

		#endregion
	}
}