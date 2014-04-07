using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HansKindberg.Build.XmlTransformation.Tasks.UnitTests
{
	[TestClass]
	public class PotentialFileTest
	{
		#region Methods

		[TestMethod]
		public void OriginalPathWithoutExtension_IfTheOriginalPathContainsADot_ShouldReturnTheOriginalPathUpToTheLastDot()
		{
			var fileSystem = Mock.Of<IFileSystem>();

			Assert.AreEqual("Web", new PotentialFile("Web.config", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\Web", new PotentialFile(@"Directory\Web.config", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\Directory\Web", new PotentialFile(@"Directory\Directory\Web.config", fileSystem).OriginalPathWithoutExtension);

			Assert.AreEqual(@"Web.Extra", new PotentialFile(@"Web.Extra.config", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\Web.Extra", new PotentialFile(@"Directory\Web.Extra.config", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\Directory\Web.Extra", new PotentialFile(@"Directory\Directory\Web.Extra.config", fileSystem).OriginalPathWithoutExtension);
		}

		[TestMethod]
		public void OriginalPathWithoutExtension_IfTheOriginalPathDoesNotContainsADot_ShouldReturnTheOriginalPath()
		{
			var fileSystem = Mock.Of<IFileSystem>();

			Assert.AreEqual("Web", new PotentialFile("Web", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\Web", new PotentialFile(@"Directory\Web", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\Directory\Web", new PotentialFile(@"Directory\Directory\Web", fileSystem).OriginalPathWithoutExtension);

			Assert.AreEqual(@"WebExtra", new PotentialFile(@"WebExtra", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\WebExtra", new PotentialFile(@"Directory\WebExtra", fileSystem).OriginalPathWithoutExtension);
			Assert.AreEqual(@"Directory\Directory\WebExtra", new PotentialFile(@"Directory\Directory\WebExtra", fileSystem).OriginalPathWithoutExtension);
		}

		#endregion
	}
}