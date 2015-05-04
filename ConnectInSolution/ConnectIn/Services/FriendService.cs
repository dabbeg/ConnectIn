using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Services
{
    public class FriendService
    {
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public FriendService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        /*public Friend GetFriendEntryById(string userId, string friendId)
        {
            var post = (from p in db.Friends
                        select p).SingleOrDefault();
            return post;
        }*/
    }
}