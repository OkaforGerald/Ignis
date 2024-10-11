using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignis.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IServiceManager manager;
        public ImagesController(IServiceManager manager)
        {
            this.manager = manager;
        }

        [HttpPost("upload")]
        // [Authorize]
        public async Task<IActionResult> UploadImages([FromQuery] List<IFormFile> files)
        {
            // var user = User.Identity?.Name;
            var user = "gerokafor360@gmail.com";

            await manager.Image.ProcessImages(user, files);

            return Ok();
        }
    }
}
