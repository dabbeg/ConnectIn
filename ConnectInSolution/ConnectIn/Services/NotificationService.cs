using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.DAL;

namespace ConnectIn.Services
{
    public class NotificationService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public NotificationService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion
    }
}