using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	public interface IXmlTransformationFactory {
		IXmlTransformableDocument CreateXmlTransformableDocument(string file);
		IXmlTransformation CreateXmlTransformation(string file);
		IXmlTransformation CreateXmlTransformation(string file, IXmlTransformationLogger xmlTransformationLogger);
	}
}