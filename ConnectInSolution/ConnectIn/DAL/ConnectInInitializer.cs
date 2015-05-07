using ConnectIn.Models;
using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ConnectIn.DAL
{
    public class ConnectInInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        // A trick to disconnect all connections to the database to the database can be dropped.
        // Basically, just switching from multi-user to single-user and back clears the connections.
        /*public override void InitializeDatabase(ApplicationDbContext context)
        {
            context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction
                , string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.Database.Connection.Database));
        
            base.InitializeDatabase(context);
        }*/

        // Seeds the new database with some hardcoded data
        protected override void Seed(ApplicationDbContext context)
        {
            var users = new List<User>
            {
                new User{
                    Id = "1",
                    UserName = "davidgui@gmail.com",
                    PhoneNumber = "8472547",
                }
            };
            var list = new List<Post>
            {
                new Post{
                    //PostId = "1",
                    Text = "Davíð er mjög kúl gæji",
                    UserId = "1",
                    Date = DateTime.Now
                },
                new Post{
                    //PostId = "2",
                    Text = "Það er skítalykt af Darra!",
                    UserId = "1",
                    Date = DateTime.Now
                },
                new Post{
                    //PostId = "3",
                    Text = "Davíð lyktar eins og blóm",
                    UserId = "1",
                    Date = DateTime.Now
                },
                new Post{
                    //PostId = "4",
                    Text = "Darri lítur út eins og skítur",
                    UserId = "1",
                    Date = DateTime.Now
                }
            };
            list.ForEach(s => context.Posts.Add(s));
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();
        }
    }
}