using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuirketyBlogData
{
    public class PostHeader
    {
        private bool _showInIndex = true;

        private List<string> _tags = new List<string>();

        public string Layout { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorUrl { get; set; }

        [JsonProperty(PropertyName="date")]
        public DateTime PubDate { get; set; }

        [JsonProperty(PropertyName="published")]
        public bool IsPublished { get; set; }

        public bool ShowInIndex
        {
            get { return _showInIndex; }
            set { _showInIndex = value; }
        }

        public List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        public string ListImageUrl { get; set; }
        public string ListImageSmallUrl { get; set; }
    }
}