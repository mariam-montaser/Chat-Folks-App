using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Data;
using SocialApp.Entities;

namespace SocialApp.Controllers
{
    public class BuggyController : BaseController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("notfound")]
        public ActionResult<AppUser> GetNotFound()
        {
            var user = this._context.Users.Find(-1);
            if(user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("servererror")]
        public ActionResult<string> GetServerError()
        {
            var user = this._context.Users.Find(-1).ToString();
            return user; 
        }

        [HttpGet("badrequest")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }


    }
}
