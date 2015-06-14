using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuirketyBlogData
{
    public class PostViewModel : IPostViewModel
    {
        public string GroupTitle { get; set; }

        public Post PostToDisplay { get; set;}

        private List<Tag> _tags = new List<Tag>();
        private List<KeyValuePair<int, string>> _tagCloud = new List<KeyValuePair<int, string>>();
        public List<Tag> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        public List<KeyValuePair<int, string>> TagCloud
        {
            get { return _tagCloud; }
            set { _tagCloud = value; }
        }
    }
}