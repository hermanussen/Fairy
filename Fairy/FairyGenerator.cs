using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Fairy.Utils;
using Scriban;
using Fairy.SCS;
using Fairy.Constants;
using Scriban.Runtime;

namespace Fairy
{
    /// <summary>
    /// Source generator that generates C# code based on Sitecore Content Serialization (SCS) .yml files.
    /// </summary>
    [Generator]
    public class FairyGenerator : ISourceGenerator
    {
        public static readonly object _scribanLock = new object();

        /// <summary>
        /// Executes the generator logic during compilation
        /// </summary>
        /// <param name="context">Generator context that contains info about the compilation</param>
        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                const string templatePath = "FairyTemplate.sbntxt";
                var templateContent = EmbeddedResource.GetContent(templatePath);
                var template = Template.Parse(templateContent, templatePath);

                var scriptObject = new ScriptObject();
                scriptObject.Import(typeof(TemplateFunctions));

                var templateContext = new TemplateContext();
                templateContext.PushGlobal(scriptObject);

                lock (_scribanLock)
                {
                    Repository.AdditionalFiles = context.AdditionalFiles;
                    templateContext.PushGlobal(ScriptObject.From(
                        new
                        {
                            Items = Repository.Evaluate()
                        }));

                    string generatedCode = template.Render(templateContext);

                    // Add the generated code to the compilation
                    context.AddSource("fairy.gen.cs",
                        SourceText.From(generatedCode, Encoding.UTF8));
                }
            }
            catch (Exception ex)
            {
                // Report a diagnostic if an exception occurs while generating code; allows consumers to know what is going on
                string message = $"Exception: {ex.Message} - {ex.StackTrace}";
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "SI0001",
                        message,
                        message,
                        "FairyGenerator",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    Location.None));
            }
        }

        /// <summary>
        /// Initialization of the generator; allows to setup visitors for syntax.
        /// </summary>
        /// <param name="context">Code generator context</param>
        public void Initialize(GeneratorInitializationContext context)
        {
            // No implementation needed here; the generator is entirely driven by use of AdditionalFiles
        }
    }
}