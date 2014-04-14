using System;
using System.Globalization;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt.Extensions
{
	public class DefaultIXmlTransformableDocumentExtension : IXmlTransformableDocumentExtension
	{
		#region Methods

		public virtual XmlTransformableDocument GetXmlTransformableDocument(IXmlTransformableDocument xmlTransformableDocument)
		{
			if(xmlTransformableDocument == null)
				return null;

			IXmlTransformableDocumentInternal xmlTransformableDocumentInternal = xmlTransformableDocument as IXmlTransformableDocumentInternal;

			if(xmlTransformableDocumentInternal != null)
				return xmlTransformableDocumentInternal.XmlTransformableDocument;

			throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "The object of type \"{0}\" does not implement \"{1}\".", xmlTransformableDocument.GetType(), typeof(IXmlTransformableDocumentInternal)));
		}

		#endregion
	}
}