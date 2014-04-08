using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationDecorator
	{
		#region Methods

		IEnumerable<ITransformableXmlFile> GetDecoratedXmlFiles(IEnumerable<ITaskItem> files);

		#endregion
	}
}