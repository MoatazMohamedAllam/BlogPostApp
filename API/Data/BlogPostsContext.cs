using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class BlogPostsContext : DbContext
    {
        public BlogPostsContext(DbContextOptions<BlogPostsContext> options)
            :base(options)
        {
            
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

    }
}