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
        // POST: api/Messages
        //[HttpPost]
        //public void Post(MessagesElements message)
        //{
        //    messages.Insertar(message);
        //}
        //[HttpPost]
        //[Route("api/Token/Post/{key}")]
        //public IActionResult SetToken([FromBody]Dictionary<string, object> json, string key)
        //{
        //    key = key.PadLeft(256, ' ');
        //    var stringJson = string.Join(",", json.Select(x => x.Key + ":" + x.Value).ToArray());
        //    return RedirectToAction("GetToken", new { key = key, json = stringJson });
        //}
        //[HttpGet]
        //[Route("api/Token/Get/{key}")]
        //public ActionResult GetToken(string key, string json)
        //{
        //    var token = TokenGenerator.generator(key, json);
        //    return Ok(token);
        //}
    }
}
