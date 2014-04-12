using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlFileFilter
	{
		IEnumerable<ITaskItem> GetXmlFiles(IEnumerable<ITaskItem> files);
	}
}
