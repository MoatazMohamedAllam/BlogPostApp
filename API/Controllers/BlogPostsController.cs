using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BlogPostsController: ControllerBase
    {
        private readonly BlogPostsContext _context;
        private readonly IDataRepository<BlogPost> _repo;

        public BlogPostsController(BlogPostsContext context,IDataRepository<BlogPost> repo)
        {
            _context = context;
            _repo = repo;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            return await _context.BlogPosts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPostById(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);

            if(post == null)
            {
                return NotFound("there is no post.");
            }
            return post;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(int id,BlogPost blogPost)
        {
            if(id != blogPost.PostId)
            {
                return BadRequest();
            }
            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                _repo.Update(blogPost);
                await _repo.SaveAsync(blogPost);
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
               
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost([FromBody]BlogPost blogPost)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repo.Add(blogPost);
            await _repo.SaveAsync(blogPost);

            return CreatedAtAction("GetBlogPostById",new {id = blogPost.PostId},blogPost);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BlogPost>> DeleteBlogPost([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _context.BlogPosts.FindAsync(id);
            if(post == null)
            {
                 return NotFound();   
            }
            _repo.Delete(post);
            await _repo.SaveAsync(post);

            return Ok(post);

        }


        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.PostId == id);
        }
    }
}