using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt.Extensions
{
	public interface IXmlTransformableDocumentExtension
	{
		#region Methods

		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		XmlTransformableDocument GetXmlTransformableDocument(IXmlTransformableDocument xmlTransformableDocument);

		#endregion
	}
}