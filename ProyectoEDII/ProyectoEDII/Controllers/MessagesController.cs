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
    public class MessagesController : ControllerBase
    {
        private readonly MessagesService messages;
        public MessagesController(MessagesService service)
        {
            messages= service;
        }

        // GET: api/Messages
        [HttpGet]
        public IEnumerable<MessagesElements> Get()
        {
            return messages.GetAll();
        }

        // GET: api/Messages/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Messages
        [HttpPost]
        public void Post(MessagesElements message)
        {
            messages.Insertar(message);
        }

        //// PUT: api/Messages/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
