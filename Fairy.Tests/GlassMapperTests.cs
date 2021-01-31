using Fairy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Fairy.Tests
{
    /// <summary>
    /// Check if generating Glass Mapper code works.
    /// </summary>
    public class GlassMapperTests : TestsBase
    {
        public GlassMapperTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ShouldGenerate()
        {
            string source = @"using System;
using System.Collections.Generic;
using TestImplementation.Templates;

namespace Example
{
    public class _Hero : I_Hero
    {
        public string HeroTitle { get; set; }
        public IEnumerable<Guid> HeroImages { get; set; }
    }

    class Test
    {
        public static string RunTest()
        {
            var hero = new _Hero()
                {
                    HeroTitle = ""testval""
                };
            
            return $""{hero.HeroTitle}"";
        }
    }
}";
            const string templatePath = "Templates/GlassMapper/FairyTemplate.sbntxt";
            var dictionary = GetEmbeddedContents("YmlFiles/Hero");
            dictionary.Add("FairyTemplate.sbntxt", EmbeddedResource.GetContent(templatePath, Assembly.GetExecutingAssembly()));
            var compilation = GetGeneratedOutput(source, dictionary);

            Assert.Equal("testval", RunTest(compilation));
        }
    }
}
