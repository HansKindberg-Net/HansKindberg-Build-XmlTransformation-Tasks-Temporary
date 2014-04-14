using System;
using System.Diagnostics.CodeAnalysis;
using HansKindberg.Build.XmlTransformation.Tasks.IoC;
using Microsoft.Web.XmlTransform;

namespace HansKindberg.Build.XmlTransformation.Tasks.Xdt.Extensions
{
	public static class XmlTransformableDocumentExtension
	{
		#region Fields

		private static volatile IXmlTransformableDocumentExtension _instance;
		private static readonly object _lockObject = new object();

		#endregion

		#region Properties

		public static IXmlTransformableDocumentExtension Instance
		{
			get
			{
				if(_instance == null)
				{
					lock(_lockObject)
					{
						if(_instance == null)
						{
							_instance = ServiceLocator.Instance.GetService<IXmlTransformableDocumentExtension>();
						}
					}
				}

				return _instance;
			}
			set
			{
				if(Equals(_instance, value))
					return;

				lock(_lockObject)
				{
					_instance = value;
				}
			}
		}

		#endregion

		#region Methods

		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		public static XmlTransformableDocument GetXmlTransformableDocument(this object value, IXmlTransformableDocument xmlTransformableDocument)
		{
			if(value == null)
				throw new ArgumentNullException("value");

			return Instance.GetXmlTransformableDocument(xmlTransformableDocument);
		}

		#endregion
	}
}