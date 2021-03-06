using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using keepr.Models;
using keepr.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Keepr.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeepsController : ControllerBase
    {
        private readonly KeepRepository _repo;
        public KeepsController(KeepRepository repo)
        {
            _repo = repo;
        }

        //GET ALL PUBLIC KEEPS
        [HttpGet]
        public ActionResult<IEnumerable<Keep>> GetAllKeeps()
        {
            return Ok(_repo.GetAll());
        }

        //GET USER KEEPs
        [HttpGet("user")]
        public ActionResult<IEnumerable<Keep>> GetUserKeeps()
        {
            var id = HttpContext.User.Identity.Name;
            IEnumerable<Keep> result = _repo.GetKeepsByUserId(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        // GET INDIVIDUAL KEEP
        [HttpGet("{id}")]
        public ActionResult<Keep> Get(int id)
        {
            Keep result = _repo.GetKeepById(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        //POST A KEEP
        [Authorize]
        [HttpPost]
        public ActionResult<List<Keep>> Post([FromBody] Keep keep)
        {
            keep.UserId = HttpContext.User.Identity.Name;
            Keep result = _repo.AddKeep(keep);
            return Created("api/keeps/" + result.Id, result);

        }

        [Authorize]
        [HttpPut("{id}")]
        public Keep Put(int id, [FromBody] Keep newKeep)
        {
            return _repo.EditKeep(id, newKeep);
        }


        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {
            if (_repo.DeleteKeep(id))
            {
                return Ok("Successfully Deleted!");
            }
            return BadRequest("Unable to Delete!");
        }
    }
}
