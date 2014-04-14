using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	[CLSCompliant(false)]
	public interface IXmlTransformableDocumentInternal
	{
		#region Properties

		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		XmlTransformableDocument XmlTransformableDocument { get; }

		#endregion
	}
}