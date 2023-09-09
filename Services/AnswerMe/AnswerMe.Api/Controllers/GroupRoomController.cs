using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnswerMe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupRoomController : ControllerBase
    {
     
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
