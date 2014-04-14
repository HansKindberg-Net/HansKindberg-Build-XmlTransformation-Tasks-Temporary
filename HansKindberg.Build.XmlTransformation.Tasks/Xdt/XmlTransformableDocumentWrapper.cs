using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "This is a wrapper.")]
	public class XmlTransformableDocumentWrapper : IXmlTransformableDocument, IXmlTransformableDocumentInternal
	{
		#region Fields

		private readonly XmlTransformableDocument _xmlTransformableDocument;

		#endregion

		#region Constructors

		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		public XmlTransformableDocumentWrapper(XmlTransformableDocument xmlTransformableDocument)
		{
			if(xmlTransformableDocument == null)
				throw new ArgumentNullException("xmlTransformableDocument");

			this._xmlTransformableDocument = xmlTransformableDocument;
		}

		#endregion

		#region Properties

		public virtual XmlTransformableDocument XmlTransformableDocument
		{
			get { return this._xmlTransformableDocument; }
		}

		#endregion

		#region Methods

		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "This is a wrapper.")]
		[SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "This is a wrapper.")]
		public virtual void Dispose()
		{
			this.XmlTransformableDocument.Dispose();
		}

		public virtual void Save(string fileName)
		{
			this.XmlTransformableDocument.Save(fileName);
		}

		#endregion
	}
}