﻿using System.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wyam.Core.Modules;
using Wyam.Abstractions;

namespace Wyam.Core.Tests.Modules
{
    [TestFixture]
    public class ReadFilesFixture
    {        
        [Test]
        public void NoInputPathReadsNoFiles()
        {
            // Given
            Engine engine = new Engine();
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles("*.txt");

            // When
            IEnumerable<IDocument> documents = readFiles.Execute(inputs, context);
            int count = documents.Count();

            // Then
            Assert.AreEqual(0, count);
        }

        [Test]
        public void ThrowsOnNullPathFunction()
        {
            // Given

            // When

            // Then
            Assert.Throws<ArgumentNullException>(() => new ReadFiles((Func<dynamic, string>)null));
        }

        [Test]
        public void ThrowsOnNullExtension()
        {
            // Given

            // When

            // Then
            Assert.Throws<ArgumentNullException>(() => new ReadFiles((string)null));
        }

        [TestCase("*.foo", SearchOption.TopDirectoryOnly, 0)]
        [TestCase("*.foo", SearchOption.AllDirectories, 0)]
        [TestCase("*.txt", SearchOption.TopDirectoryOnly, 2)]
        [TestCase("*.txt", SearchOption.AllDirectories, 3)]
        [TestCase("*.md", SearchOption.TopDirectoryOnly, 1)]
        [TestCase("*.md", SearchOption.AllDirectories, 2)]
        [TestCase("*.*", SearchOption.TopDirectoryOnly, 3)]
        [TestCase("*.*", SearchOption.AllDirectories, 5)]
        public void SearchPatternFindsCorrectFiles(string searchPattern, SearchOption searchOption, int expectedCount)
        {
            // Given
            Engine engine = new Engine();
            engine.Metadata["InputPath"] = @"TestFiles\Input\";
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles(searchPattern, searchOption);

            // When
            IEnumerable<IDocument> documents = readFiles.Execute(inputs, context);
            int count = documents.Count();
            
            // Then
            Assert.AreEqual(expectedCount, count);
        }

        [Test]
        public void SearchPatternWorksWithoutInputPathTrailingSlash()
        {
            // Given
            Engine engine = new Engine();
            engine.Metadata["InputPath"] = @"TestFiles\Input";
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles("*.txt");

            // When
            IEnumerable<IDocument> documents = readFiles.Execute(inputs, context);
            int count = documents.Count();
            
            // Then
            Assert.AreEqual(3, count);
        }

        [Test]
        public void SearchPatternWorksWithInputPathTrailingSlash()
        {
            // Given
            Engine engine = new Engine();
            engine.Metadata["InputPath"] = @"TestFiles\Input";
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles("*.txt");

            // When
            IEnumerable<IDocument> documents = readFiles.Execute(inputs, context);
            int count = documents.Count();
            
            // Then
            Assert.AreEqual(3, count);
        }

        [Test]
        public void SearchPatternWorksWithSubpath()
        {
            // Given
            Engine engine = new Engine();
            engine.Metadata["InputPath"] = @"TestFiles\Input";
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles(@"Subfolder\*.txt");

            // When
            IEnumerable<IDocument> documents = readFiles.Execute(inputs, context);
            int count = documents.Count();
            
            // Then
            Assert.AreEqual(1, count);
        }

        [Test]
        public void SearchPatternWorksWithSingleFile()
        {
            // Given
            Engine engine = new Engine();
            engine.Metadata["InputPath"] = @"TestFiles\Input";
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles(@"test-a.txt");

            // When
            IEnumerable<IDocument> documents = readFiles.Execute(inputs, context);
            int count = documents.Count();
            
            // Then
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ExecuteGetsCorrectContent()
        {
            // Given
            Engine engine = new Engine();
            engine.Metadata["InputPath"] = @"TestFiles\Input";
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles(@"test-a.txt");

            // When
            IDocument document = readFiles.Execute(inputs, context).First();
            
            // Then
            Assert.AreEqual("aaa", document.Content);
        }

        [TestCase("FileRoot", @"TestFiles\Input")]
        [TestCase("FileBase", @"test-c")]
        [TestCase("FileExt", @".txt")]
        [TestCase("FileName", @"test-c.txt")]
        [TestCase("FileDir", @"TestFiles\Input\Subfolder")]
        [TestCase("FilePath", @"TestFiles\Input\Subfolder\test-c.txt")]
        public void PrepareSetsMetadata(string key, string expectedEnding)
        {
            // Given
            Engine engine = new Engine();
            engine.Metadata["InputPath"] = @"TestFiles\Input";
            Metadata metadata = new Metadata(engine);
            IDocument[] inputs = { new Document(metadata) };
            Pipeline pipeline = new Pipeline("Pipeline", engine, null);
            IExecutionContext context = new ExecutionContext(engine, pipeline);
            ReadFiles readFiles = new ReadFiles(@"test-c.txt");

            // When
            IDocument document = readFiles.Execute(inputs, context).First();
            
            // Then
            Assert.That(document.Metadata[key], Is.StringEnding(expectedEnding));
        }
    }
}
