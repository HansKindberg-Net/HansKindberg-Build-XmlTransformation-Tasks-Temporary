using System;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	[CLSCompliant(false)]
	public interface IXmlTransformationFactory
	{
		#region Methods

		IXmlTransformableDocument CreateXmlTransformableDocument(string file);
		IXmlTransformation CreateXmlTransformation(string file);
		IXmlTransformation CreateXmlTransformation(string file, IXmlTransformationLogger xmlTransformationLogger);

		#endregion
	}
}