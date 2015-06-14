using System;
namespace QuirketyBlogData
{
    public interface IPostViewModel
    {
        string GroupTitle { get; set; }
        System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, string>> TagCloud { get; set; }
        System.Collections.Generic.List<Tag> Tags { get; set; }
    }
}
