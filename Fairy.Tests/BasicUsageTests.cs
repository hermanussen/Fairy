using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Fairy.Tests
{
    /// <summary>
    /// Unit tests basic usage scenarios.
    /// </summary>
    public class BasicUsageTests : TestsBase
    {
        public BasicUsageTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ShouldGenerate()
        {
            string source = @"using System;
using Templates;

namespace Example
{
    class Test
    {
        public static string RunTest()
        {
            var templateName = _Hero.TemplateName;
            var fieldName = _Hero.HeroTitleFieldName;
            
            return $""{templateName} {fieldName}"";
        }
    }
}";
            var compilation = GetGeneratedOutput(source, GetEmbeddedContents("YmlFiles/Hero"));

            Assert.Equal("_Hero Hero Title", RunTest(compilation));
        }

        private IDictionary<string, string> GetEmbeddedContents(string relativePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var baseName = assembly.GetName().Name;
            var resourceName = relativePath
                .TrimStart('.')
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');
            var name = $"{baseName}.{resourceName}.";

            return assembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(name)).ToDictionary(r => r, r =>
                    {
                        using var stream = assembly.GetManifestResourceStream(r);

                        if (stream == null)
                        {
                            throw new NotSupportedException("Unable to get embedded resource content, because the stream was null");
                        }

                        using var reader = new StreamReader(stream);
                        return reader.ReadToEnd();
                    });
        }
    }
}
