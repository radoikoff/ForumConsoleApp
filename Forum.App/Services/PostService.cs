﻿using Forum.App.UserInterface.ViewModels;
using Forum.Data;
using Forum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forum.App.Services
{
    public static class PostService
    {
        internal static Category GetCategory(int categoryId)
        {
            var forumData = new ForumData();
            var category = forumData.Categories.Find(c => c.Id == categoryId);
            return category;
        }

        internal static IList<ReplyViewModel> GetPostReplies(int postId)
        {
            var forumData = new ForumData();

            Post post = forumData.Posts.Find(p => p.Id == postId);

            var replies = new List<ReplyViewModel>();

            foreach (var replyId in post.ReplyIds)
            {
                var reply = forumData.Replies.Find(r => r.Id == replyId);
                replies.Add(new ReplyViewModel(reply));
            }

            return replies;
        }

        internal static string[] GetAllGategoryNames()
        {
            var forumData = new ForumData();

            var allCategories = forumData.Categories.Select(c => c.Name).ToArray();

            return allCategories;
        }

        public static IEnumerable<Post> GetPostsByCategory(int categoryId)
        {
            var forumData = new ForumData();
            var postIds = forumData.Categories.First(c => c.Id == categoryId).Posts;
            var posts = forumData.Posts.Where(p => postIds.Contains(p.Id));

            return posts;
        }

        public static PostViewModel GetPostViewModel(int postId)
        {
            var forumData = new ForumData();
            Post post = forumData.Posts.Find(p => p.Id == postId);
            return new PostViewModel(post);
        }

        private static Category EnsureCategory(PostViewModel postView, ForumData forumData)
        {
            var categoryName = postView.Category;
            var category = forumData.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
                var categories = forumData.Categories;
                int categoryId = categories.Any() ? categories.Last().Id + 1 : 1;
                category = new Category(categoryId, categoryName, new List<int>());
                forumData.Categories.Add(category);
            }
            return category;
        }

        public static bool TrySavePost(PostViewModel postView)
        {
            bool emptyCategory = string.IsNullOrWhiteSpace(postView.Category);
            bool emptyTitle = string.IsNullOrWhiteSpace(postView.Title);
            bool emptyContent = !postView.Content.Any();

            if (emptyCategory || emptyTitle || emptyContent)
            {
                return false;
            }

            var forumData = new ForumData();
            Category category = EnsureCategory(postView, forumData);

            int postId = forumData.Posts.Any() ? forumData.Posts.Last().Id + 1 : 1;

            User author = UserService.GetUser(postView.Author);
            int authorId = author.Id;

            var content = string.Join("", postView.Content);

            Post post = new Post(postId, postView.Title, content, category.Id, authorId, new List<int>() { 1 });

            forumData.Posts.Add(post);
            author.PostIds.Add(post.Id);
            category.Posts.Add(post.Id);
            forumData.SaveChages();

            postView.PostId = postId;

            return true;
        }


    }
}
