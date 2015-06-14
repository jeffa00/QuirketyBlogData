using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuirketyBlogData
{
    public class PostsViewModel : QuirketyBlogData.IPostViewModel
    {
        private List<Post> _posts = new List<Post>();
        private List<Tag> _tags = new List<Tag>();
        private List<KeyValuePair<int, string>> _tagCloud = new List<KeyValuePair<int, string>>();

        public string GroupTitle { get; set; }

        public int CurrentPageNumber { get; set; }
        public int PostsPerPage { get; set; }
        public int TotalPages { get; set; }

        public List<Post> Posts
        {
            get { return _posts; }
            set { _posts = value; }
        }

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
