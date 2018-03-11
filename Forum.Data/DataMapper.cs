using Forum.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Forum.Data
{
    public class DataMapper
    {
        private const string DATA_PATH = "../data/";
        private const string CONFIG_PATH = "config.ini";
        private const string DEFAULT_CONFIG = "users=users.csv\r\ncategories=categories.csv\r\nposts=posts.csv\r\nreplies=replies.csv";
        private static readonly Dictionary<string, string> config; //entity collection group, file name

        static DataMapper()
        {
            Directory.CreateDirectory(DATA_PATH);
            config = LoadConfig(DATA_PATH + CONFIG_PATH);
        }

        private static void EnsureConfigFile(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                File.WriteAllText(configFilePath, DEFAULT_CONFIG);
            }
        }

        private static void EnsureFile(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
        }

        private static Dictionary<string, string> LoadConfig(string configFilePath)
        {
            EnsureConfigFile(configFilePath);

            string[] contents = ReadLines(configFilePath);
            var config = contents.Select(x => x.Split('=')).ToDictionary(t => t[0], t => DATA_PATH + t[1]);

            return config;
        }

        private static string[] ReadLines(string path)
        {
            EnsureFile(path);
            var lines = File.ReadAllLines(path);
            return lines;
        }

        private static void WriteLines(string path, string[] lines)
        {
            File.WriteAllLines(path, lines);
        }

        public static List<Category> LoadCategories()
        {
            var categories = new List<Category>();
            var dataLines = ReadLines(config["categories"]);

            foreach (var line in dataLines)
            {
                var args = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(args[0]);
                var name = args[1];
                var postIds = args[2].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                categories.Add(new Category(id, name, postIds));
            }

            return categories;
        }

        public static void SaveCategories(List<Category> categories)
        {
            var lines = new List<string>();
            const string categoryFormat = "{0};{1};{2}";

            foreach (var category in categories)
            {
                string line = string.Format(categoryFormat,
                        category.Id,
                        category.Name,
                        string.Join(",", category.Posts)
                        );
                lines.Add(line);
            }

            WriteLines(config["categories"], lines.ToArray());
        }

        public static List<User> LoadUsers()
        {
            var users = new List<User>();
            var dataLines = ReadLines(config["users"]);

            foreach (var line in dataLines)
            {
                var args = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(args[0]);
                var username = args[1];
                var password = args[2];
                var postIds = args[3].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                users.Add(new User(id, username, password, postIds));
            }

            return users;
        }

        public static void SaveUsers(List<User> users)
        {
            var lines = new List<string>();
            const string userFormat = "{0};{1};{2};{3}";

            foreach (var user in users)
            {
                string line = string.Format(userFormat,
                        user.Id,
                        user.Username,
                        user.Password,
                        string.Join(",", user.PostIds)
                        );
                lines.Add(line);
            }

            WriteLines(config["users"], lines.ToArray());
        }

        public static List<Post> LoadPosts()
        {
            var posts = new List<Post>();
            var dataLines = ReadLines(config["posts"]);

            foreach (var line in dataLines)
            {
                var args = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(args[0]);
                var title = args[1];
                var content = args[2];
                var categoryId = int.Parse(args[3]);
                var authorId = int.Parse(args[4]);
                var replyIds = args[5].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                posts.Add(new Post(id, title, content, categoryId, authorId, replyIds));
            }

            return posts;
        }

        public static void SavePosts(List<Post> posts)
        {
            var lines = new List<string>();
            const string postFormat = "{0};{1};{2};{3};{4};{5}";

            foreach (var post in posts)
            {
                string line = string.Format(postFormat,
                        post.Id,
                        post.Title,
                        post.Content,
                        post.CategoryId,
                        post.AuthorId,
                        string.Join(",", post.ReplyIds)
                        );
                lines.Add(line);
            }

            WriteLines(config["posts"], lines.ToArray());
        }

        public static List<Reply> LoadReplies()
        {
            var replies = new List<Reply>();
            var dataLines = ReadLines(config["replies"]);

            foreach (var line in dataLines)
            {
                var args = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var id = int.Parse(args[0]);
                var content = args[1];
                var authorId = int.Parse(args[2]);
                var postId = int.Parse(args[3]);
                replies.Add(new Reply(id, content, authorId, postId));
            }

            return replies;
        }

        public static void SaveReplies(List<Reply> replies)
        {
            var lines = new List<string>();
            const string postFormat = "{0};{1};{2};{3}";

            foreach (var reply in replies)
            {
                string line = string.Format(postFormat,
                        reply.Id,
                        reply.Content,
                        reply.AuthorId,
                        reply.PostId
                        );
                lines.Add(line);
            }

            WriteLines(config["replies"], lines.ToArray());
        }
    }
}
