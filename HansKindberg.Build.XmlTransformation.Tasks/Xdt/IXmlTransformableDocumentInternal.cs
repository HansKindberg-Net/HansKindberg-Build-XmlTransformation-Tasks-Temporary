using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	public interface IXmlTransformableDocumentInternal
	{
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		XmlTransformableDocument XmlTransformableDocument { get; }
	}
}