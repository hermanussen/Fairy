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
