using System;
using System.Diagnostics.CodeAnalysis;
using HansKindberg.Build.XmlTransformation.Tasks.Xdt.Extensions;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt
{
	[CLSCompliant(false)]
	[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "This is a wrapper.")]
	public class XmlTransformationWrapper : IXmlTransformation
	{
		#region Fields

		private readonly Microsoft.Web.XmlTransform.XmlTransformation _xmlTransformation;

		#endregion

		#region Constructors

		public XmlTransformationWrapper(Microsoft.Web.XmlTransform.XmlTransformation xmlTransformation)
		{
			if(xmlTransformation == null)
				throw new ArgumentNullException("xmlTransformation");

			this._xmlTransformation = xmlTransformation;
		}

		#endregion

		#region Properties

		protected internal virtual Microsoft.Web.XmlTransform.XmlTransformation XmlTransformation
		{
			get { return this._xmlTransformation; }
		}

		#endregion

		#region Methods

		public virtual bool Apply(IXmlTransformableDocument xmlTarget)
		{
			return this.XmlTransformation.Apply(this.GetXmlTransformableDocument(xmlTarget));
		}

		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "This is a wrapper.")]
		[SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "This is a wrapper.")]
		public virtual void Dispose()
		{
			this.XmlTransformation.Dispose();
		}

		#endregion
	}
}