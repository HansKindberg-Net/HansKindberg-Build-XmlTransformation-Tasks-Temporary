﻿using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace HansKindberg.Build.XmlTransformation.Tasks.IntegrationTests.Helpers
{
	public static class XmlFileExtensionHelper
	{
		#region Fields

		private static readonly IEnumerable<string> _defaultXmlFileExtensions = new[] {".config", ".resx", ".xml"};

		#endregion

		#region Methods

		public static IEnumerable<ITaskItem> CreateDefaultXmlFileExtensions()
		{
			return CreateXmlFileExtensions(_defaultXmlFileExtensions);
		}

		public static IEnumerable<ITaskItem> CreateXmlFileExtensions(IEnumerable<string> extensions)
		{
			var xmlFileExtensions = new List<ITaskItem>();

			if(extensions == null)
				return xmlFileExtensions.ToArray();

			foreach(var extension in extensions)
			{
				if(extension == null)
					throw new ArgumentException("The extensions parameter can not contain null values.", "extensions");

				xmlFileExtensions.Add(new TaskItem(extension));
			}

			return xmlFileExtensions.ToArray();
		}

		#endregion
	}
}