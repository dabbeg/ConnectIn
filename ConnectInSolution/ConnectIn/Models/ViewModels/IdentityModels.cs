﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using ConnectIn.Models.Entity;
using ConnectIn.DAL;
using System;
using System.Collections.Generic;

namespace ConnectIn.Models
{
 
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public DateTime Birthday { get; set; }
        public string Work { get; set; }
        public string School { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public bool Privacy { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Friend> Friends { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<LikeDislike> LikesDislikes { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ApplicationUser()
        {
            Birthday = DateTime.Now;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public IDbSet<Comment> Comments { get; set; }
        public IDbSet<Friend> Friends { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<LikeDislike> LikesDislikes { get; set; }
        public IDbSet<Member> Members { get; set; }
        public IDbSet<Notification> Notifications { get; set; }
        public IDbSet<Photo> Photos { get; set; }
        public IDbSet<Post> Posts { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<IdentityUser>()
        //        .ToTable("Users", "dbo").Property(p => p.Id).HasColumnName("User_Id");
        //    modelBuilder.Entity<ApplicationUser>()
        //        .ToTable("Users", "dbo").Property(p => p.Id).HasColumnName("User_Id");
        //}

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}