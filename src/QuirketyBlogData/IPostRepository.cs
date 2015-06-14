using System.Collections.Generic;

namespace QuirketyBlogData
{
    public interface IPostRepository
    {
        string ContentDir { get; set; }
        string ContentOutDir { get; set; }
        MarkdownProcessor DefaultProcessor { get; set; }
        int ItemCount { get; }
        int PageCount { get; }
        int PostCount { get; }
        string SrcDir { get; set; }
        string ViewDir { get; set; }

        Post AddPost(string newKey, Post newPost);
        void DeletePost(string key);
        List<Tag> GetFullTagList(int pageNum, int itemsPerPage);
        List<Post> GetItems(int pageNum, int itemsPerPage, bool showUnpublished, string type, bool showAllTypes);
        List<Post> GetPages(int pageNum, int postsPerPage, bool showUnpublished);
        Post GetPost(string key);
        int GetPostCountForTag(string tagName);
        List<Post> GetPosts(int pageNum, int postsPerPage, bool showUnpublished);
        string GetTagDisplayName(string tagName);
        List<Post> GetTagList(string tagName, int pageNum, int itemsPerPage);
        int GetTagListCount();
        List<string> GetTagNameList();
        void PublishContentFiles();
        Post UpdatePost(string updateKey, Post updatePost);
    }
}