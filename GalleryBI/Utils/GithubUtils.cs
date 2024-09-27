using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalleryBI.Utils
{
    internal class GithubUtils
    {
        public static string GetTemplateOwner(string templateUrl)
        {
            return AppContext.TemplateDefaultOwner;
        }

        public static string GetIssueCreateDate(string issueUrl)
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
