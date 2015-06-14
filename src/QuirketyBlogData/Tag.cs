using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuirketyBlogData
{
    public class Tag : LinkableItem, IComparable<Tag>
    {
        private List<Post> _posts = new List<Post>();

        public string Title { get; set; }
        public string Link 
        {
            get
            {
                return TitleToUrl(Title);
            }
        }
        public List<Post> Posts
        {
            get { return _posts; }
            set { _posts = value; }
        }

        public int TotalPosts
        {
            get
            {
                return Posts.Count();
            }
        }

        public int CompareTo(Tag compareTag)
        {
            if (compareTag == null)
                return 1;

            return this.Title.CompareTo(compareTag.Title);
        }
    }
}
