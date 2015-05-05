using ConnectIn.DAL;
using ConnectIn.Models;
using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Services
{
    public class GroupService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public GroupService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion

        public Group GetGroupById(int groupId)
        {
            var gro = (from g in db.Groups
                       where g.GroupId == groupId
                       select g).SingleOrDefault();
            return gro;
        }
        public List<string> GetMembersOfGroup(int groupId)
        {
            // Get the ID of all members in that group
            var list = (from gm in db.Members
                         where gm.GroupId == groupId
                         orderby gm.UserId ascending
                         select gm.UserId).ToList();

            return list;
        }

        // Get the Id of all the users news feeds posts by a given Id of user
        /* public List<string> GetEveryPostsOfGroup(string groupId)
        {
            // Get the Id of all members in that group
            var members = GetMembersOfGroup(groupId);

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where friends.Contains(s.UserId)
                            || s.UserId == userId
                            orderby s.Date descending
                            select s.PostId).Take(20).ToList();
            return statuses;
        }*/
        // get all posts of group
    }
}