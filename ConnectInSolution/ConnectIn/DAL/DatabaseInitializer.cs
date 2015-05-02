using ConnectIn.Models;
using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ConnectIn.DAL
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser{
                    Id = "1",
                    UserName = "davidgudni@gmail.com",
                    PhoneNumber = "8472547"
                }
            };
            var list = new List<Post>
            {
                new Post{
                    PostId = 1,
                    Text = "blblblbla",
                    UserId = "1",
                    Date = DateTime.Now
                }
            };
            list.ForEach(s => context.Posts.Add(s));
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            //base.Seed(context);
        }
    }
}