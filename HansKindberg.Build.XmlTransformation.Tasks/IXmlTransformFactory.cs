using System.Collections.Generic;
using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformFactory
	{
		#region Properties

		IFileSystem FileSystem { get; }
		IPotentialFileFactory PotentialFileFactory { get; }

		#endregion

		#region Methods

		IXmlFileToTransform Create(ITaskItem file, string transformName, XmlTransformMode xmlTransformMode, IEnumerable<IXmlTransformationMap> xmlTransformationMaps, IValidationLog validationLog);

		#endregion
	}
}