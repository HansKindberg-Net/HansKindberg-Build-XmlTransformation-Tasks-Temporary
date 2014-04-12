using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using HansKindberg.Build.XmlTransformation.Tasks.Validation;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HansKindberg.Build.XmlTransformation.Tasks.UnitTests
{
	[TestClass]
	public class XmlTransformationDecoratorTest
	{
		#region Fields

		private const string _dependentUponMetadataName = "DependentUpon";

		#endregion

		#region Methods

		private static FileInfoBase CreateFileInfoBase()
		{
			var fileInfoBaseMock = new Mock<FileInfoBase>();

			return fileInfoBaseMock.Object;
		}

		private static IFileInfoFactory CreateFileInfoFactory()
		{
			var fileInfoFactoryMock = new Mock<IFileInfoFactory>();

			fileInfoFactoryMock.Setup(fileInfoFactory => fileInfoFactory.FromFileName(It.IsAny<string>())).Returns(CreateFileInfoBase());

			return fileInfoFactoryMock.Object;
		}

		private static IFileSystem CreateFileSystem()
		{
			var fileSystemMock = new Mock<IFileSystem>();

			fileSystemMock.Setup(fileSystem => fileSystem.FileInfo).Returns(CreateFileInfoFactory);
			fileSystemMock.Setup(fileSystem => fileSystem.Path).Returns(CreatePathBase);

			return fileSystemMock.Object;
		}

		private static PathBase CreatePathBase()
		{
			var pathBaseMock = new Mock<PathBase>();

			pathBaseMock.Setup(pathBase => pathBase.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns((string firstPath, string secondPath) => Path.Combine(firstPath, secondPath));
			pathBaseMock.Setup(pathBase => pathBase.GetDirectoryName(It.IsAny<string>())).Returns((string path) => Path.GetDirectoryName(path));
			pathBaseMock.Setup(pathBase => pathBase.GetFileNameWithoutExtension(It.IsAny<string>())).Returns((string path) => Path.GetFileNameWithoutExtension(path));

			return pathBaseMock.Object;
		}

		private static ITaskItem CreateTaskItem(string itemSpecification)
		{
			var taskItemMock = new Mock<ITaskItem>();

			taskItemMock.Setup(taskItem => taskItem.ItemSpec).Returns(itemSpecification);
			taskItemMock.Setup(taskItem => taskItem.ToString()).Returns(itemSpecification);
			taskItemMock.Setup(taskItem => taskItem.GetMetadata("Extension")).Returns(Path.GetExtension(itemSpecification));
			taskItemMock.Setup(taskItem => taskItem.GetMetadata("FullPath")).Returns(@"C:\" + itemSpecification);

			return taskItemMock.Object;
		}

		private static XmlTransformationDecorator CreateXmlTransformationDecorator()
		{
			return new XmlTransformationDecorator(CreateFileSystem(), CreateXmlTransformationSettings(), Mock.Of<IEqualityComparer<ITaskItem>>(), Mock.Of<IValidationLog>());
		}

		private static IXmlTransformationSettings CreateXmlTransformationSettings()
		{
			var xmlTransformationSettingsMock = new Mock<IXmlTransformationSettings>();
			xmlTransformationSettingsMock.Setup(xmlTransformationSettings => xmlTransformationSettings.TransformName).Returns("Test");

			return xmlTransformationSettingsMock.Object;
		}

		private static void IsDependentUponByFileNameTest(string fileName, IEnumerable<string> fileNames, bool assertIsTrue)
		{
			var file = CreateTaskItem(fileName);
			var files = fileNames.Select(CreateTaskItem).ToList();

			if(assertIsTrue)
				Assert.IsTrue(CreateXmlTransformationDecorator().IsDependentUponByFileName(file, files));
			else
				Assert.IsFalse(CreateXmlTransformationDecorator().IsDependentUponByFileName(file, files));
		}

		[TestMethod]
		public void IsDependentUponByFileName_IfFilesHaveDifferentDirectories_ShouldReturnFalse()
		{
			IsDependentUponByFileNameTest(@"Directory\Directory\File.Interfix.extension", new[] {@"Directory\File.extension", @"Directory\File.Interfix.extension"}, false);
		}

		[TestMethod]
		public void IsDependentUponByFileName_IfFilesStartWithTheFileNameWithoutExtension_ShouldReturnTrue()
		{
			IsDependentUponByFileNameTest(@"Directory\File.Interfix.extension", new[] {@"Directory\File.extension", @"Directory\File.Interfix.extension"}, true);
		}

		[TestMethod]
		public void IsDependentUponByFileName_IfNoFilesStartWithTheFileNameWithoutExtension_ShouldReturnFalse()
		{
			IsDependentUponByFileNameTest(@"Directory\File.extension", new[] {@"Directory\File.extension", @"Directory\File.Interfix.extension"}, false);
		}

		[TestMethod]
		public void IsDependentUponByFileName_IfTheTaskItemEnumerableParameterValueIsNull_ShouldReturnFalse()
		{
			Assert.IsFalse(CreateXmlTransformationDecorator().IsDependentUponByFileName(Mock.Of<ITaskItem>(), null));
		}

		[TestMethod]
		public void IsDependentUponByFileName_IfTheTaskItemParameterValueIsNull_ShouldReturnFalse()
		{
			Assert.IsFalse(CreateXmlTransformationDecorator().IsDependentUponByFileName(null, new ITaskItem[0]));
		}

		[TestMethod]
		public void IsDependentUponByFileName_ShouldBeCaseInsensitive()
		{
			IsDependentUponByFileNameTest(@"DIRECTORY\FILE.INTERFIX.EXTENSION", new[] {@"Directory\File.extension", @"Directory\File.Interfix.extension"}, true);
		}

		[TestMethod]
		public void IsDependentUpon_IfTheTaskItemHasDependentUponMetadataThatIsEmpty_ShouldReturnFalse()
		{
			var taskItemMock = new Mock<ITaskItem>();
			taskItemMock.Setup(taskItem => taskItem.GetMetadata(_dependentUponMetadataName)).Returns(string.Empty);

			Assert.IsFalse(CreateXmlTransformationDecorator().IsDependentUpon(taskItemMock.Object));
		}

		[TestMethod]
		public void IsDependentUpon_IfTheTaskItemHasDependentUponMetadataThatIsNotNullOrEmpty_ShouldReturnTrue()
		{
			var taskItemMock = new Mock<ITaskItem>();
			taskItemMock.Setup(taskItem => taskItem.GetMetadata(_dependentUponMetadataName)).Returns("Test");

			Assert.IsTrue(CreateXmlTransformationDecorator().IsDependentUpon(taskItemMock.Object));
		}

		[TestMethod]
		public void IsDependentUpon_IfTheTaskItemHasDependentUponMetadataThatIsNull_ShouldReturnFalse()
		{
			var taskItemMock = new Mock<ITaskItem>();
			taskItemMock.Setup(taskItem => taskItem.GetMetadata(_dependentUponMetadataName)).Returns((string) null);

			Assert.IsFalse(CreateXmlTransformationDecorator().IsDependentUpon(taskItemMock.Object));
		}

		[TestMethod]
		public void IsDependentUpon_IfTheTaskItemParameterValueIsNull_ShouldReturnFalse()
		{
			Assert.IsFalse(CreateXmlTransformationDecorator().IsDependentUpon(null));
		}

		#endregion
	}
}