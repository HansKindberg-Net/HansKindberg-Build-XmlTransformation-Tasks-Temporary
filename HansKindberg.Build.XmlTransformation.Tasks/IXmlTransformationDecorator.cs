using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationDecorator : IXmlFileFilter
	{
		#region Methods

		IEnumerable<ITaskItem> GetDecoratedFiles(IEnumerable<ITaskItem> files);

		#endregion
	}
}