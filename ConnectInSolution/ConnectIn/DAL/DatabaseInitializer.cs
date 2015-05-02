using ConnectIn.Models;
using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ConnectIn.DAL
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var list = new List<Post>
            {
                new Post{
                    ID = 1,
                    text = "blblblbla",
                    userID = "4",
                    date = DateTime.Now
                }
            };
            list.ForEach(s => context.Posts.Add(s));
            //base.Seed(context);
            context.SaveChanges();
        }
    }
}