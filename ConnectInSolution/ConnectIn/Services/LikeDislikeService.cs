using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using System.Linq;

namespace ConnectIn.Services
{
    public class LikeDislikeService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public LikeDislikeService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion

        #region get likes/dislikes from userid and postid
        // Return the LikeDislike from userid and postid
        public LikeDislike GetLikeDislike(string userId, int postId)
        {
            var ld = (from n in db.LikesDislikes
                      where n.UserId == userId
                            && n.PostId == postId
                      select n).SingleOrDefault();

            return ld;
        }
#endregion
    }
}