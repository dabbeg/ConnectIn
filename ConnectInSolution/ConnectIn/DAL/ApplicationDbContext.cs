using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using ConnectIn.Models.Entity;
using ConnectIn.DAL;
using System;
using System.Collections.Generic;

namespace ConnectIn.DAL
{
    public interface IAppDataContext
    {
        IDbSet<Comment> Comments { get; set; }
        IDbSet<Friend> Friends { get; set; }
        IDbSet<Group> Groups { get; set; }
        IDbSet<LikeDislike> LikesDislikes { get; set; }
        IDbSet<Member> Members { get; set; }
        IDbSet<Notification> Notifications { get; set; }
        IDbSet<Photo> Photos { get; set; }
        IDbSet<Post> Posts { get; set; }
        IDbSet<User> Users { get; set; }
        int SaveChanges();
    }

    public class ApplicationDbContext : IdentityDbContext<User>, IAppDataContext
    {
        // Tables in the database
        public IDbSet<Comment> Comments { get; set; }
        public IDbSet<Friend> Friends { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<LikeDislike> LikesDislikes { get; set; }
        public IDbSet<Member> Members { get; set; }
        public IDbSet<Notification> Notifications { get; set; }
        public IDbSet<Photo> Photos { get; set; }
        public IDbSet<Post> Posts { get; set; }

        // Tells the Context to connect to the database defined in the DefaultConnection
        // connection string which can be found and modified in the Web.config
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        // Overwrites the default behavior the User Table To name the table "User" and
        // name the Id UserId.
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>()
                .ToTable("Users", "dbo").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<User>()
                .ToTable("Users", "dbo").Property(p => p.Id).HasColumnName("UserId");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        /*public System.Data.Entity.DbSet<ConnectIn.Models.Entity.User> IdentityUsers { get; set; }*/
    }
}