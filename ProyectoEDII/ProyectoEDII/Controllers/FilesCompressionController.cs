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
    public class FilesCompressionController : ControllerBase
    {
        private readonly FilesCompressionService files;
        public FilesCompressionController(FilesCompressionService service)
        {
            files = service;
        }
        // GET: api/Messages
        [HttpGet]
        public IEnumerable<FilesCompressionElements> Get()
        {
            return files.GetAll();
        }
        // POST: api/FilesCompression
        [HttpPost]
        public void Post(FilesCompressionElements file)
        {
            files.Insertar(file);
        }
    }
}
