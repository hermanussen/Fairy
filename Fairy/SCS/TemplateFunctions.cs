using Fairy.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Fairy.SCS
{
    public static class TemplateFunctions
    {
        private static readonly char[] forbiddenCharacters = new[] { ' ', '-', ':', ';' };

        /// <summary>
        /// Resolve the parent of an item.
        /// </summary>
        /// <param name="item">Item for which to resolve</param>
        /// <returns></returns>
        public static ScsFile? ResolveParent(ScsFile item)
        {
            return Repository.Evaluate(file => file.Id == item.Id).FirstOrDefault();
        }

        /// <summary>
        /// Resolve the children of an item.
        /// </summary>
        /// <param name="item">Item for which to resolve</param>
        /// <returns></returns>
        public static IEnumerable<ScsFile> ResolveChildren(ScsFile item)
        {
            return Repository.Evaluate(file => file.ParentId == item.Id);
        }

        /// <summary>
        /// Resolve the fields for a template item.
        /// Returns empty if the item that is passed is not a template itself.
        /// </summary>
        /// <param name="item">Item for which to resolve</param>
        /// <returns></returns>
        public static IEnumerable<ScsFile> ResolveTemplateFields(ScsFile item)
        {
            if (!item.IsTemplate)
            {
                return Array.Empty<ScsFile>();
            }

            var sections = ResolveChildren(item);
            return sections.SelectMany(ResolveChildren).Where(f => f.IsTemplateField);
        }

        /// <summary>
        /// Resolve the fields for a template item and all of its base templates.
        /// Returns empty if the item that is passed is not a template itself.
        /// </summary>
        /// <param name="item">Item for which to resolve</param>
        /// <returns></returns>
        public static IEnumerable<ScsFile> ResolveTemplateFieldsDeep(ScsFile item)
        {
            if (!item.IsTemplate)
            {
                return Array.Empty<ScsFile>();
            }

            var templates = new[] { item }.Concat(ResolveTemplateBaseTemplatesDeep(item));
            return templates.SelectMany(ResolveTemplateFields);
        }

        /// <summary>
        /// Resolve the direct base templates for a template item.
        /// Returns empty if the item that is passed is not a template itself.
        /// </summary>
        /// <param name="item">Item for which to resolve</param>
        /// <returns></returns>
        public static IEnumerable<ScsFile> ResolveTemplateBaseTemplates(ScsFile item)
        {
            if(!item.IsTemplate
                || item.BaseTemplateIds == null
                || !item.BaseTemplateIds.Any())
            {
                return Array.Empty<ScsFile>();
            }

            return Repository.Evaluate(file => file.IsTemplate && item.BaseTemplateIds.Contains(file.Id));
        }

        /// <summary>
        /// Resolve the base templates and their base templates recursively for a template item.
        /// Returns empty if the item that is passed is not a template itself.
        /// </summary>
        /// <param name="item">Item for which to resolve</param>
        /// <returns></returns>
        public static IEnumerable<ScsFile> ResolveTemplateBaseTemplatesDeep(ScsFile item)
        {
            var baseTemplates = ResolveTemplateBaseTemplates(item).ToList();
            foreach(var baseTemplate in baseTemplates)
            {
                baseTemplates.AddRange(ResolveTemplateBaseTemplatesDeep(baseTemplate).Except(baseTemplates));
            }

            return baseTemplates;
        }

        /// <summary>
        /// Determine a friendly name, that should be valid in C#.
        /// </summary>
        /// <param name="name">Potentially unfriendly name</param>
        /// <returns></returns>
        public static string Friendly(string name)
        {
            List<char> newName = new List<char>();
            bool nextCharUpper = true;
            for (int i = 0; i < name.Length; i++)
            {
                // Strip spaces and other characters
                if (forbiddenCharacters.Contains(name[i]))
                {
                    nextCharUpper = true;
                    continue;
                }

                // Pascal casing
                if (nextCharUpper)
                {
                    nextCharUpper = false;
                    if (!char.IsUpper(name[i]))
                    {
                        newName.Add(char.ToUpper(name[i], CultureInfo.InvariantCulture));
                        continue;
                    }
                }

                newName.Add(name[i]);
            }

            return new string(newName.ToArray());
        }
    }
}
