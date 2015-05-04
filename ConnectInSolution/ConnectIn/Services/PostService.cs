using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.Models.Entity;
using ConnectIn.Models;
using ConnectIn.DAL;

namespace ConnectIn.Services
{
    public class PostService
    {
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public PostService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        public Post GetPostById(string userId)
        {
            var post = (from p in db.Posts
                        where userId == p.PostId
                        select p).SingleOrDefault();
            return post;
        }

        
        
        // comments likes dislikes
        // group service
    }
}