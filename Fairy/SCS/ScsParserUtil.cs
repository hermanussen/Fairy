﻿using Fairy.Constants;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fairy.SCS
{
    public static class ScsParserUtil
    {
        /// <summary>
        /// Parses the SCS (.yml) file.
        /// </summary>
        /// <param name="text">The text of the .yml file.</param>
        /// <param name="sourceRepository"></param>
        /// <returns></returns>
        public static ScsFile ParseScsFile(AdditionalText text)
        {
            const string idParseKey = "ID: ";
            const string pathParseKey = "Path: ";
            const string templateParseKey = "Template: ";
            const string parentParseKey = "Parent: ";
            const string baseTemplateIntroLine = "- ID: \"12c33f3f-86c5-43a5-aeb4-5598cec45116\"";
            const string baseTemplateIntroLineNoQuotes = "- ID: 12c33f3f-86c5-43a5-aeb4-5598cec45116";
            const string baseTemplateParseKey = "Value: ";
            const string fieldTypeIntroLine = "- ID: \"ab162cc0-dc80-4abf-8871-998ee5d7ba32\"";
            const string fieldTypeIntroLineNoQuotes = "- ID: ab162cc0-dc80-4abf-8871-998ee5d7ba32";
            const string fieldTypeParseKey = "Value: ";

            var textLines = text.GetText()?.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (textLines.Any())
            {
                var result = new ScsFile();

                string textLineId = textLines.FirstOrDefault(l => l.StartsWith(idParseKey));
                Guid idGuid;
                if (textLineId != null && Guid.TryParse(textLineId.Substring(idParseKey.Length).Trim(' ', '"'), out idGuid))
                {
                    result.Id = idGuid;
                }

                var textLineParent = textLines.FirstOrDefault(l => l.StartsWith(parentParseKey));
                Guid parentIdGuid;
                if (textLineParent != null && Guid.TryParse(textLineParent.Substring(parentParseKey.Length).Trim(' ', '"'), out parentIdGuid))
                {
                    result.ParentId = parentIdGuid;
                }

                var textLinePath = textLines.FirstOrDefault(l => l.StartsWith(pathParseKey));
                if (textLinePath != null)
                {
                    result.Path = textLinePath.Substring(pathParseKey.Length).Trim().Trim('\"');
                }

                string textLineTemplate = textLines.FirstOrDefault(l => l.StartsWith(templateParseKey));
                Guid templateGuid;
                if (textLineTemplate != null && Guid.TryParse(textLineTemplate.Substring(templateParseKey.Length).Trim(' ', '"'), out templateGuid))
                {
                    result.TemplateId = templateGuid;

                    // Parse base template(s) if the item is a template
                    if (Guid.Equals(result.TemplateId, SitecoreConstants.SitecoreTemplateTemplateId))
                    {
                        result.BaseTemplateIds = new List<Guid>();

                        ////int? lineIndex = textLines
                        ////    .Select((l, i) => new { Index = i, Line = l })
                        ////    .FirstOrDefault(l => string.Equals(baseTemplateIntroLine, l.Line.Trim()) || string.Equals(baseTemplateIntroLineNoQuotes, l.Line.Trim()))?.Index;

                        var baseTemplateIdLines = textLines
                            .SkipWhile(l => !(string.Equals(baseTemplateIntroLine, l.Trim()) || string.Equals(baseTemplateIntroLineNoQuotes, l.Trim())))
                            .SkipWhile(l => !l.TrimStart().StartsWith(baseTemplateParseKey))
                            .TakeWhile(l => l.StartsWith("  "));
                        foreach (string baseTemplateIdLine in baseTemplateIdLines)
                        {
                            string line = baseTemplateIdLine.Trim();
                            if (line.StartsWith(baseTemplateParseKey))
                            {
                                line = line.Substring(baseTemplateParseKey.Length);
                            }

                            line = line.Trim(' ', '|', '"');
                            Guid baseTemplateId;
                            if (Guid.TryParse(line, out baseTemplateId))
                            {
                                result.BaseTemplateIds.Add(baseTemplateId);
                            }
                        }
                    }
                    else if(Guid.Equals(result.TemplateId, SitecoreConstants.SitecoreTemplateFieldId))
                    {
                        var fieldTemplateTypeLines = textLines
                            .SkipWhile(l => !(string.Equals(fieldTypeIntroLine, l.Trim()) || string.Equals(fieldTypeIntroLineNoQuotes, l.Trim())))
                            .SkipWhile(l => !l.TrimStart().StartsWith(fieldTypeParseKey))
                            .TakeWhile(l => l.StartsWith("  "));
                        foreach (string fieldTemplateTypeLine in fieldTemplateTypeLines)
                        {
                            string line = fieldTemplateTypeLine.Trim();
                            if (line.StartsWith(fieldTypeParseKey))
                            {
                                line = line.Substring(fieldTypeParseKey.Length);
                            }

                            line = line.Trim(' ', '|', '"');
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                result.TemplateFieldType = line;
                            }
                        }
                    }
                }

                return result;
            }

            // Not a valid result, but return an empty object anyway so it won't have to be parsed more often
            return new ScsFile();
        }
    }
}
