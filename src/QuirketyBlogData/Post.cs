using CommonMark;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuirketyBlogData
{
    public class Post : LinkableItem, IComparable<Post>
    {
        private PostHeader _metaData = new PostHeader();

        public string FileName { get; set; }
        public string RawContent { get; set; }
        public PostHeader MetaData
        {
            get { return _metaData; }
            set { _metaData = value; }
        }

        public string PlainContent { get; set; }
        public string ParsedContent { get; set; }
        public string Link { get; set; }

        public MarkdownProcessor Processor { get; set; }

        public Post(string fileName, string link, MarkdownProcessor processor, string rawContent)
        {
            Processor = processor;
            FileName = fileName;
            Link = link;
            RawContent = rawContent;
            parseRawContent();
        }


        public string GetPostFileText()
        {
            StringBuilder text = new StringBuilder();

            string metaDataText = JsonConvert.SerializeObject(MetaData, Formatting.Indented);

            text.AppendLine(metaDataText);
            text.Append("---");
            text.Append(PlainContent);

            return text.ToString();
        }

        private void parseRawContent()
        {
            if (RawContent.IndexOf("---") < 0)
                return;

            int firstMarker = RawContent.IndexOf("---");

            string headerString = string.Empty;

            headerString = RawContent.Substring(0, firstMarker);
            PlainContent = RawContent.Substring(firstMarker + 3);

            switch (Processor)
            {
                case MarkdownProcessor.CommonMark:
                    ParsedContent = CommonMarkConverter.Convert(PlainContent);
                    break;
                //case MarkdownProcessor.MarkdownDeep:
                //    var mdd = new MarkdownDeep.Markdown();

                //    mdd.ExtraMode = true;
                //    mdd.SafeMode = false;
                //    //md.MarkdownInHtml = true;
                //    mdd.NewWindowForExternalLinks = true;
                //    ParsedContent = mdd.Transform(PlainContent);
                //    break;
                default:
                    break;
            }

            if (headerString.IndexOf("{") < 0)
                throw new InvalidDataException("Content file metadata section did not contain a JavaScript Object.");

            try
            {
                MetaData = JsonConvert.DeserializeObject<PostHeader>(headerString);
            }
            catch (Exception ex)
            {
                // TODO log exception                
                throw;
            }

        }

        public int CompareTo(Post comparePost)
        {
            if (comparePost == null)
                return 1;

            return this.MetaData.Title.CompareTo(comparePost.MetaData.Title);
        }
    }

}