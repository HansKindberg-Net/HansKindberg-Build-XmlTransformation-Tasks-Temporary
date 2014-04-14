using System;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	public interface IXmlTransformation : IDisposable {
		bool Apply(IXmlTransformableDocument xmlTarget);
	}
}