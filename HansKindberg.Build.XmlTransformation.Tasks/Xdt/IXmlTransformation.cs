using System;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	public interface IXmlTransformation : IDisposable
	{
		#region Methods

		bool Apply(IXmlTransformableDocument xmlTarget);

		#endregion
	}
}