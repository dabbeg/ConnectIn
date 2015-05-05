using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Services
{
    public class CommentService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public CommentService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion

        #region comments of a given post id
        // Get comments by a given user Id
        public Comment GetCommentById(int commentId)
        {
            var comment = (from c in db.Comments
                        where commentId == c.CommentId
                        select c).SingleOrDefault();
            return comment;
        }
        #endregion
    }
}