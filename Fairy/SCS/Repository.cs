namespace Fairy.SCS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public static class Repository
    {
        /// <summary>
        /// SCS file hashes that can be used to keep reference to caches for already parsed files.
        /// </summary>
        private static readonly Dictionary<string, int> _scsFileHashes = new Dictionary<string, int>();

        /// <summary>
        /// Cached versions of parsed .yml files.
        /// </summary>
        private static readonly Dictionary<string, ScsFile> _scsFiles = new Dictionary<string, ScsFile>();

        private static readonly object additionalFilesLock = new object();
        private static IEnumerable<AdditionalText>? _additionalFiles;
        public static IEnumerable<AdditionalText>? AdditionalFiles
        {
            get
            {
                return _additionalFiles;
            }
            internal set
            {
                lock(additionalFilesLock)
                {
                    _additionalFiles = value;
                }
            }
        }

        /// <summary>
        /// Finds derived templates recursively.
        /// </summary>
        /// <param name="allTemplateFiles">All template files.</param>
        /// <param name="templateFile">The template.</param>
        /// <param name="maxDepth">The maximum depth, to prevent stack overflow if there are circular references.</param>
        /// <returns></returns>
        private static IEnumerable<ScsFile> FindDerivedTemplates(List<ScsFile> allTemplateFiles, ScsFile templateFile, int maxDepth)
        {
            if (maxDepth > 0)
            {
                foreach (ScsFile derivedTemplate in allTemplateFiles.Where(t => t.BaseTemplateIds != null && t.BaseTemplateIds.Contains(templateFile.Id)))
                {
                    yield return derivedTemplate;

                    foreach (var derived in FindDerivedTemplates(allTemplateFiles, derivedTemplate, maxDepth - 1))
                    {
                        yield return derived;
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates which of the .yml files match the specified function.
        /// </summary>
        /// <param name="files">The .yml files.</param>
        /// <param name="evaluateFunction">The evaluation function.</param>
        /// <returns></returns>
        public static IEnumerable<ScsFile> Evaluate(Func<ScsFile, bool>? evaluateFunction = null)
        {
            lock (additionalFilesLock)
            {
                if (AdditionalFiles != null)
                {
                    foreach (AdditionalText text in AdditionalFiles)
                    {
                        ScsFile file;

                        // This code will probably not run in parallel, but ensure that the dictionaries remain in sync just in case
                        lock (_scsFileHashes)
                        {
                            if (_scsFileHashes.ContainsKey(text.Path))
                            {
                                // We have already parsed this file, so check if our cache is up to date
                                if (_scsFileHashes[text.Path] == text.GetHashCode())
                                {
                                    // The cache is up to date, so use the cached version
                                    file = _scsFiles[text.Path];
                                }
                                else
                                {
                                    // Update the cache
                                    _scsFileHashes[text.Path] = text.GetHashCode();
                                    file = ScsParserUtil.ParseScsFile(text);
                                    _scsFiles[text.Path] = file;
                                }
                            }
                            else
                            {
                                // Parse the file and add it to the cache
                                _scsFileHashes.Add(text.Path, text.GetHashCode());
                                file = ScsParserUtil.ParseScsFile(text);
                                _scsFiles.Add(text.Path, file);
                            }
                        }

                        // Only return a file if it matches the provided function
                        if (evaluateFunction == null || evaluateFunction(file))
                        {
                            yield return file;
                        }
                    }
                }
            }
        }
    }
}
