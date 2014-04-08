using System.Collections.Generic;
using System.IO.Abstractions;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationContext : IPotentialFileFactory, IXmlTransformationMapRepository
	{
		#region Properties

		IFileSystem FileSystem { get; }
		IEnumerable<ITaskItem> XmlFileExtensions { get; set; }
		IEnumerable<ITaskItem> XmlTransformationMaps { get; set; }

		#endregion

		#region Methods

		IEnumerable<ITransformableXmlFile> GetTransformableXmlFiles(IEnumerable<ITaskItem> files, string transformName, XmlTransformMode transformMode, IValidationLog validationLog);

		#endregion
	}
}