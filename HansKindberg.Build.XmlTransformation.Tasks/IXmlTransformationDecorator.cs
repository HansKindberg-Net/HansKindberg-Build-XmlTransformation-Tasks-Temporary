using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace HansKindberg.Build.XmlTransformation.Tasks
{
	public interface IXmlTransformationDecorator
	{
		#region Methods

		void DecorateFile(ITaskItem file);
		IEnumerable<ITaskItem> GetDecoratedFiles(IEnumerable<ITaskItem> files);

		#endregion
	}
}