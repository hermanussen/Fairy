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

        [Fact]
        public void ShouldGenerateWithOwnTemplate()
        {
            string source = @"using System;
using SomethingUnrelated;

namespace Example
{
    class Test
    {
        public static string RunTest()
        {
            return $""{UnrelatedType.GetMessage()}"";
        }
    }
}";
            var dictionary = GetEmbeddedContents("YmlFiles/Hero");
            dictionary.Add("FairyTemplate.sbntxt", "namespace SomethingUnrelated { public static class UnrelatedType { public static string GetMessage() { return \"from template that was defined in the project\"; } } }");
            var compilation = GetGeneratedOutput(source, dictionary);

            Assert.Equal("from template that was defined in the project", RunTest(compilation));
        }
    }
}
