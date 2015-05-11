using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;

namespace ConnectIn.Services
{
    public class PhotoService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public PhotoService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion

        #region Get photo by id
        public Photo GetPhotoById(int photoId)
        {
            var photo = (from p in db.Photos
                where p.PhotoId == photoId
                select p).SingleOrDefault();

            return photo;
        }
        #endregion
    }
}