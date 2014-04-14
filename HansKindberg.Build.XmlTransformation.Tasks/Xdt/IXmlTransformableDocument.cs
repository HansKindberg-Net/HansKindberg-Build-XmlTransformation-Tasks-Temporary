using System;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	public interface IXmlTransformableDocument : IDisposable
	{
		#region Methods

		void Save(string fileName);

		#endregion
	}
}