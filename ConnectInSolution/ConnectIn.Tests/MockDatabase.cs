using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeDbSet;
using System.Data.Entity;
using ConnectIn.Models;
using ConnectIn.Models.Entity;
using ConnectIn.DAL;

namespace ConnectIn.Tests
{
    /// <summary>
    /// This is an example of how we'd create a fake database by implementing the 
    /// same interface that the BookeStoreEntities class implements.
    /// </summary>
    public class MockDatabase : IAppDataContext
    {
        /// <summary>
        /// Sets up the fake database.
        /// </summary>
        public MockDatabase()
        {
            // We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.
            this.Comments = new InMemoryDbSet<Comment>();
            this.Friends = new InMemoryDbSet<Friend>();
            this.Groups = new InMemoryDbSet<Group>();
            this.LikesDislikes = new InMemoryDbSet<LikeDislike>();
            this.Members = new InMemoryDbSet<Member>();
            this.Notifications = new InMemoryDbSet<Notification>();
            this.Photos = new InMemoryDbSet<Photo>();
            this.Posts = new InMemoryDbSet<Post>();
            this.Users = new InMemoryDbSet<User>();
        }

        public IDbSet<Comment> Comments { get; set; }
        public IDbSet<Friend> Friends { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<LikeDislike> LikesDislikes { get; set; }
        public IDbSet<Member> Members { get; set; }
        public IDbSet<Notification> Notifications { get; set; }
        public IDbSet<Photo> Photos { get; set; }
        public IDbSet<Post> Posts { get; set; }
        public IDbSet<User> Users { get; set; }

        public int SaveChanges()
        {
            // Pretend that each entity gets a database id when we hit save.
            int changes = 0;
            // changes += DbSetHelper.IncrementPrimaryKey<Author>(x => x.AuthorId, this.Authors);
            // changes += DbSetHelper.IncrementPrimaryKey<Book>(x => x.BookId, this.Books);

            return changes;
        }

        public void Dispose()
        {
            // Do nothing!
        }
    }
}