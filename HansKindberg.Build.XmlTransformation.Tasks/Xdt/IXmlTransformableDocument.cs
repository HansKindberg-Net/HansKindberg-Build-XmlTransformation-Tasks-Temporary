using System;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	public interface IXmlTransformableDocument : IDisposable
	{
		void Save(string fileName);
	}
}