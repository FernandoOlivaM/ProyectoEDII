using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoEDII.LogInServices;
using ProyectoEDII.Models;

namespace ProyectoEDII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private readonly LogInService login;
        public LogInController(LogInService service)
        {
            login = service;
        }

        // GET: api/LogIn
        [HttpGet]
        public IEnumerable<LogInElements> Get()
        {
            return login.GetAll();
        }

        // GET: api/LogIn/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(string id)
        {
            var user = login.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/LogIn
        [HttpPost]
        public void Post(LogInElements user)
        {
            login.Insertar(user);
        }

        // PUT: api/LogIn/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] LogInElements UserElements)
        {
            var user = login.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            login.Modificar(id, UserElements);
            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var user = login.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            login.Eliminar(id);
            return NoContent();
        }
    }
}
