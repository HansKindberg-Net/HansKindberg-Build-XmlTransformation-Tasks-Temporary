using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlFileFilter
	{
		#region Methods

		IEnumerable<ITaskItem> GetXmlFiles(IEnumerable<ITaskItem> files);

		#endregion
	}
}