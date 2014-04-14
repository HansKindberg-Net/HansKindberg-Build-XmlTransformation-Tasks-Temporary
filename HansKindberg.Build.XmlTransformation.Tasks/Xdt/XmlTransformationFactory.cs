using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	[CLSCompliant(false)]
	public class XmlTransformationFactory : IXmlTransformationFactory
	{
		#region Methods

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The caller is responsible for disposing the object.")]
		public virtual IXmlTransformableDocument CreateXmlTransformableDocument(string file)
		{
			XmlTransformableDocument xmlTransformableDocument = new XmlTransformableDocument
			{
				PreserveWhitespace = true
			};

			xmlTransformableDocument.Load(file);

			return new XmlTransformableDocumentWrapper(xmlTransformableDocument);
		}

		public virtual IXmlTransformation CreateXmlTransformation(string file)
		{
			return this.CreateXmlTransformation(file, null);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The caller is responsible for disposing the object.")]
		public virtual IXmlTransformation CreateXmlTransformation(string file, IXmlTransformationLogger xmlTransformationLogger)
		{
			return new XmlTransformationWrapper(new Microsoft.Web.XmlTransform.XmlTransformation(file, xmlTransformationLogger));
		}

		#endregion
	}
}