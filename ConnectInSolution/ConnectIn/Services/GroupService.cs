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


        #region Get group by Id
        // Get a group by a Id
        public Group GetGroupById(int groupId)
        {
            var gro = (from g in db.Groups
                       where g.GroupId == groupId
                       select g).SingleOrDefault();
            return gro;
        }
        #endregion

        #region Members and posts of groups
        // Get the Id of all memebers in a group, given its Id
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
        public List<int> GetAllPostsOfGroup(int groupId)
        {
            // Get the Id of all members in that group
            var members = GetMembersOfGroup(groupId);

            // Get all the posts from friends with that postgroupId
            var statuses = (from s in db.Posts
                            where members.Contains(s.UserId)
                            && s.GroupId == groupId
                            orderby s.Date descending
                            select s.PostId).Take(20).ToList();
            return statuses;
        }

        //Get a certain member by a given Id of user and Id of group
        public Member GetMemberByUserIdAndGroupId(int groupId, string userId)
        {
            var member = (from m in db.Members
                            where m.UserId == userId
                            && m.GroupId == groupId
                            select m).SingleOrDefault();
            return member;
        }

        // return true if member is in group
        public bool IsMemberOfGroup(int groupId, string userId)
        {
            var member = (from m in db.Members
                      where m.UserId == userId
                      && m.GroupId == groupId
                      select m).SingleOrDefault();

            return member != null;
        }

        #endregion

        #region Regarding group notifications
        // return all notifications of that group
        public ICollection<Notification> GetNotificatonsForGroup(int groupId)
        {
            var not = (from n in db.Notifications
                    where n.GroupId == groupId
                    select n).ToList();
            return not;
        }


        #endregion

    }
}