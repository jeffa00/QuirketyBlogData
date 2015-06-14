using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuirketyBlogData
{
    public class LinkableItem
    {
        public static string TitleToUrl(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            title = title.ToLower();
            string patternToUnderscore = "[\\-\\/(){}\\[\\]\\~\\!@#$%\\^&\\*=\\-|\\?><\\.,'\"\\;\\:]";
            string patternMultipleUnderscores = "_+";

            title = title.Replace("+", "_plus");

            title = Regex.Replace(title, "\\s+", "_");
            title = Regex.Replace(title, patternToUnderscore, "_");
            title = Regex.Replace(title, patternMultipleUnderscores, "_");

            return title;
        }

    }
}
