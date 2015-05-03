using ConnectIn.DAL;
using ConnectIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Services
{
    public class GroupService
    {
        public List<string> GetMembersOfGroup(string groupId)
        {
            var db = new ApplicationDbContext();

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