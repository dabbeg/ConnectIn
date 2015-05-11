using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class User : IdentityUser
    {
        #region Columns
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Work { get; set; }
        public string School { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public int Privacy { get; set; }
        #endregion

        #region ForeignKeys
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Friend> Friends { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Member> Members { get; set; }
        public ICollection<LikeDislike> LikesDislikes { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        #endregion

        public User()
        {
            Birthday = DateTime.Now;
            Privacy = 0;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}