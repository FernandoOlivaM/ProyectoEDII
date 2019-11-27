using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoEDII.LogInServices;
using ProyectoEDII.Models;
using DLLS;
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
        //POST: api/Messages
       [HttpPost]
        public void Post(MessagesElements message)
        {
            messages.Insertar(message);
        }
    }
}
