using directory_scanner.directory_scanner;
using NUnit.Framework;
using System;
using System.Linq;

namespace directory_scanner.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InvalidThreadCount()
        {
            Assert.Catch<Exception>(() =>
            {
                var directoryScanner = new DirectoryScanner(0);
            });
        }

        [Test]
        public void InvalidDirectoryName()
        {
            Assert.Catch<Exception>(() =>
            {
                var directoryScanner = new DirectoryScanner(25);
                directoryScanner.Start("Z:/cringe");
            });
        }

        [Test]
        public void TestFinish()
        {
            var directoryScanner = new DirectoryScanner(25);

            directoryScanner.Start("D:\\5 sem\\spp\\lab3\\test1");

            var tree = directoryScanner.Finish();
            
            
            Assert.Multiple(() =>
            {
                Assert.That(tree.Name, Is.EqualTo("test1"));
                Assert.That(tree.Length, Is.AtLeast(200100200));
                Assert.That(tree.LengthPercentage, Is.EqualTo(100.0));
                Assert.That(tree.Children.Count, Is.EqualTo(4));
                
            });
            
        }

        [Test]
        public void TestCancel()
        {
            var directoryScanner = new DirectoryScanner(25);

            directoryScanner.Start("D:\\5 sem\\spp\\lab3\\test1");

            var tree = directoryScanner.Stop();


            Assert.Multiple(() =>
            {
                Assert.That(tree.Name, Is.EqualTo("test1"));
                Assert.That(tree.Length, Is.AtMost(199100200));
                Assert.That(tree.LengthPercentage, Is.EqualTo(100.0));
                Assert.That(tree.Children.Count, Is.AtMost(4));

            });

        }
    }
}