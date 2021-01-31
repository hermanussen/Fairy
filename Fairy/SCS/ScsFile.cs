using Fairy.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fairy.SCS
{
    public class ScsFile
    {
        /// <summary>
        /// Gets or sets the Sitecore ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Sitecore path.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the Sitecore template ID.
        /// </summary>
        public Guid TemplateId { get; set; }

        public List<Guid>? BaseTemplateIds { get; set; }

        public string? TemplateFieldType { get; set; }

        /// <summary>
        /// Gets or sets the parent item ID.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Gets the name of the item as determined from the path.
        /// </summary>
        public string? ItemName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Path))
                {
                    return null;
                }

                if (!(this.Path?.TrimEnd('/').Contains("/") ?? false))
                {
                    return this.Path;
                }

                return this.Path.Substring(this.Path.TrimEnd('/').LastIndexOf('/') + 1);
            }
        }

        /// <summary>
        /// Returns true if the current item is a template itself.
        /// </summary>
        public bool IsTemplate
        {
            get
            {
                return TemplateId == SitecoreConstants.SitecoreTemplateTemplateId;
            }
        }

        /// <summary>
        /// Returns true if the current item is a template field.
        /// </summary>
        public bool IsTemplateField
        {
            get
            {
                return TemplateId == SitecoreConstants.SitecoreTemplateFieldId;
            }
        }
    }
}
