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
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public GroupService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }

        public Group GetGroupById(string groupId)
        {
            var gro = (from g in db.Groups
                       where g.Id == groupId
                       select g).SingleOrDefault();
            return gro;
        }
        public List<string> GetMembersOfGroup(string groupId)
        {
            // Get the ID of all members in that group
            var list = (from gm in db.Members
                         where gm.GroupId == groupId
                         orderby gm.UserId ascending
                         select gm.UserId).ToList();

            return list;
        }

        // get all posts of group
    }
}