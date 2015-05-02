using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Services
{
    public class PostService
    {
        //This service folder is to access the database
        //This class PostService is to get posts, possible functions
        //could be like getAllPosts(user) which gets all posts for this user
        //TODO: Create functions to access the database and return information
        public List<Post> getAllPosts(string userName)
        {
            return new List<Post>();
        }
    }
}