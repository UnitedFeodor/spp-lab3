using directory_scanner.directory_scanner;
using NUnit.Framework;

namespace directory_scanner.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var directoryScanner = new DirectoryScanner(25);

            directoryScanner.Start("D:/5 sem/kursach");
            //var tree = directoryScanner.Stop();

            var tree = directoryScanner.GetResult();

            
            Assert.Pass();
        }
    }
}