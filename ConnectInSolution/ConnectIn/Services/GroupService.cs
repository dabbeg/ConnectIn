using ConnectIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Services
{
    public class GroupService
    {
        public List<string> GetMembersOfGroup(string groupID)
        {
            var db = new ApplicationDbContext();

            // Get the ID of all members in that group
            var list = (from gm in db.Members
                         where gm.groupID == groupID
                         orderby gm.userID ascending
                         select gm.userID).ToList();

            return list;
        }

        // get all posts of group
    }
}