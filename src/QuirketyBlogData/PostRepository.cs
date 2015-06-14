using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;

namespace QuirketyBlogData
{
    public enum MarkdownProcessor
    {
        CommonMark,
        MarkdownDeep
    }

    public class PostRepository : IPostRepository
    {
        // Source Directory
        public string SrcDir { get; set; }
        public string ContentDir { get; set; }
        public string ContentOutDir { get; set; }
        public string ViewDir { get; set; }

        public MarkdownProcessor DefaultProcessor { get; set; }

        // Singleton dictionary that stores the posts.
        private static ConcurrentDictionary<string, Post> Posts = new ConcurrentDictionary<string, Post>();
        private static Dictionary<string, Tag> TagIndex = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);

        private static System.Threading.Timer _timer;
        public PostRepository()
        {

        }

        /*
         * Since it really doesn't make sense to create a PostRepository without a source directory,
         * I'm not creating a bare constructor.
         * */
        public PostRepository(string srcDir, string contentDir, string contentOutDir, string viewDir, MarkdownProcessor defaultProcessor)
        {
            SrcDir = srcDir;
            ContentDir = contentDir;
            ContentOutDir = contentOutDir;
            ViewDir = viewDir;
            DefaultProcessor = defaultProcessor;

            LoadAllPosts();

            CopyContentFiles();
            CopyViewFiles();

            _timer = new Timer(_timer_Elapsed, null, 0, 6000);
        }

        private void CopyViewFiles()
        {
            CopyContentDir(SrcDir + "files\\views\\", ViewDir);
        }

        private void CopyContentFiles()
        {
            CopyContentDir(SrcDir + "files\\content\\", ContentDir);
        }

        public void PublishContentFiles()
        {
            CopyContentDir(ContentDir, ContentOutDir);
        }
        //private void CopyContentDir(string srcDir, string targetDir)
        //{
        //    var srcFiles = System.IO.Directory.EnumerateFiles(srcDir, "*.*");
        //    var srcDirs = System.IO.Directory.EnumerateDirectories(srcDir);

        //    if (Directory.Exists(targetDir) == false)
        //        Directory.CreateDirectory(targetDir);

        //    Parallel.ForEach(srcFiles, file =>
        //    {
        //        try
        //        {

        //            FileInfo fileInfo = new FileInfo(file);
        //            string newTargetFileName = targetDir + "\\" + fileInfo.Name;

        //            File.Copy(file, newTargetFileName, true);
        //        }
        //        catch (IOException ex)
        //        {

        //            //throw;
        //        }
        //    });

        //    Parallel.ForEach(srcDirs, dir =>
        //    {
        //        try
        //        {

        //            DirectoryInfo dirInfo = new DirectoryInfo(dir);
        //            string newTargetDir = targetDir + "\\" + dirInfo.Name;

        //            if (Directory.Exists(newTargetDir) == false)
        //                Directory.CreateDirectory(newTargetDir);

        //            CopyContentDir(dir, newTargetDir);
        //        }
        //        catch (IOException ex)
        //        {

        //            //throw;
        //        }
        //    });
        //}
        private void CopyContentDir(string srcDir, string targetDir)
        {
            var srcFiles = System.IO.Directory.EnumerateFiles(srcDir, "*.*");
            var srcDirs = System.IO.Directory.EnumerateDirectories(srcDir);

            if (Directory.Exists(targetDir) == false)
                Directory.CreateDirectory(targetDir);

            foreach (var file in srcFiles)
            {
                try
                {

                    FileInfo fileInfo = new FileInfo(file);
                    string newTargetFileName = targetDir + "\\" + fileInfo.Name;

                    File.Copy(file, newTargetFileName, true);
                }
                catch (IOException ex)
                {

                    //throw;
                }
            }

            foreach (var dir in srcDirs)
            {
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    string newTargetDir = targetDir + "\\" + dirInfo.Name;

                    if (Directory.Exists(newTargetDir) == false)
                        Directory.CreateDirectory(newTargetDir);

                    CopyContentDir(dir, newTargetDir);
                }
                catch (IOException ex)
                {

                    //throw;
                }
            }
        }

        void _timer_Elapsed(object sender)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            LoadAllPosts();
            CopyContentFiles();
            //CopyViewFiles();

            _timer.Change(0, 6000);
        }

        private void LoadAllPosts()
        {
            DateTime startTime = DateTime.Now;

            ConcurrentBag<string> currentFiles = new ConcurrentBag<string>();

            //if (Posts.Count < 1)
            LoadDir(SrcDir + "documents", currentFiles);

            LoadTagIndex();

            foreach (var file in currentFiles)
            {
                if (Posts.ContainsKey(file) == false)
                    DeletePost(file);
            }

            DateTime endTime = DateTime.Now;

            var ellapsed = endTime.Subtract(startTime).TotalMilliseconds;
        }

        private Task LoadTagIndex()
        {
            Dictionary<string, Tag> newDictionary = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);

            var items = GetItems(1, Posts.Count, false, "", true).ToList();

            foreach (var item in items)
            {
                foreach (var tagName in item.MetaData.Tags)
                {
                    Tag tag;

                    if (newDictionary.TryGetValue(Tag.TitleToUrl(tagName), out tag) == false)
                    {
                        tag = new Tag();
                        tag.Title = tagName;

                        newDictionary.Add(tag.Link, tag);
                    }
                    tag.Posts.Add(item);
                }
            }

            TagIndex = newDictionary;

            return null;
        }

        private void LoadDir(string dirPath, ConcurrentBag<string> currentFiles)
        {
            var files = Directory.EnumerateFiles(dirPath, "*.md");
            var dirs = Directory.EnumerateDirectories(dirPath);

            Parallel.ForEach(files, file =>
            {
                string fileContents = File.ReadAllText(file);

                AddPost(file, new Post(file, file.Replace(dirPath, "").Replace(".html", "").Replace(".md", ""), DefaultProcessor, fileContents));
                currentFiles.Add(file);
            });

            Parallel.ForEach(dirs, dir =>
            {
                LoadDir(dir, currentFiles);
            });
        }

        public int PostCount
        {
            get
            {
                return Posts.Values
                .Where(m => m.MetaData.Type == "post")
                .Where(m => m.MetaData.PubDate != null)
                .Where(m => m.MetaData.PubDate <= DateTime.Now)
                .Where(m => m.MetaData.IsPublished == true)
                .Where(m => m.MetaData.ShowInIndex == true)
                .Count();
            }
        }
        public int PageCount
        {
            get
            {
                return Posts.Values
                .Where(m => m.MetaData.Type == "page")
                .Where(m => m.MetaData.PubDate != null)
                .Where(m => m.MetaData.PubDate <= DateTime.Now)
                .Where(m => m.MetaData.IsPublished == true)
                .Where(m => m.MetaData.ShowInIndex == true)
                .Count();
            }
        }

        public int ItemCount
        {
            get
            {
                return PostCount + PageCount;
            }
        }

        public List<Post> GetPosts(int pageNum, int postsPerPage, bool showUnpublished)
        {
            return GetItems(pageNum, postsPerPage, showUnpublished, "post", false);
        }

        public List<Post> GetPages(int pageNum, int postsPerPage, bool showUnpublished)
        {
            return GetItems(pageNum, postsPerPage, showUnpublished, "page", false);
        }

        public List<Post> GetItems(int pageNum, int itemsPerPage, bool showUnpublished, string type, bool showAllTypes)
        {
            var postList = Posts.Values.Where(m => m.MetaData != null)
                .Where(m => m.MetaData.PubDate != null)
                .Where(m => m.MetaData.PubDate <= DateTime.Now || showUnpublished)
                .Where(m => m.MetaData.IsPublished == true || showUnpublished)
                .Where(m => m.MetaData.ShowInIndex == true || showUnpublished)
                .Where(m => showAllTypes == true || m.MetaData.Type == type)
                .OrderByDescending(d => d.MetaData.PubDate)
                .Skip((pageNum - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            return postList;
        }

        public List<string> GetTagNameList()
        {
            return TagIndex.Keys.ToList();
        }

        public string GetTagDisplayName(string tagName)
        {
            var keys = TagIndex.AsQueryable();

            var displayName = keys.FirstOrDefault(c => c.Key.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            return displayName.Key;
        }

        public List<Tag> GetFullTagList(int pageNum, int itemsPerPage)
        {
            List<Tag> list = TagIndex.Values.ToList();
            list.Sort();

            return list
                .Skip((pageNum - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();
        }

        public int GetTagListCount()
        {
            return TagIndex.Count;
        }

        public int GetPostCountForTag(string tagName)
        {
            if (TagIndex.ContainsKey(tagName) == false)
                return 0;

            return TagIndex[tagName].Posts.Count;
        }

        public List<Post> GetTagList(string tagName, int pageNum, int itemsPerPage)
        {
            List<Post> list = TagIndex[tagName].Posts;
            //list.Sort();

            return list
                .Skip((pageNum - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();
        }

        public Post AddPost(string newKey, Post newPost)
        {
            Posts.AddOrUpdate(newKey, key => newPost, (key, oldValue) => newPost);
            return PersistPost(newKey, newPost);
        }

        public Post UpdatePost(string updateKey, Post updatePost)
        {
            Posts.AddOrUpdate(updateKey, key => updatePost, (key, oldValue) => updatePost);
            return PersistPost(updateKey, updatePost);
        }

        private Post PersistPost(string fileName, Post post)
        {
            if (string.IsNullOrWhiteSpace(post.MetaData.Title) ||
                string.IsNullOrWhiteSpace(post.MetaData.Type))
                return post;

            if (post.RawContent != post.GetPostFileText())
                System.IO.File.WriteAllText(fileName, post.GetPostFileText(), Encoding.UTF8);

            return post;
        }

        public Post GetPost(string key)
        {
            Post post = null;

            bool keyExisted = Posts.TryGetValue(key, out post);

            if (keyExisted == false)
                return null;

            return post;
        }

        public void DeletePost(string key)
        {
            Post deletedPost = null;

            Posts.TryRemove(key, out deletedPost);

        }
    }
}