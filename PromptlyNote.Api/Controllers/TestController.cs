using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromptlyNote.Core.Exceptions;

namespace PromptlyNote.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult TestForbidden()
        {
            throw new ForbiddenException();
        }

        [HttpGet]
        public IActionResult TestInternalError()
        {
            throw new InternalException();
        }

        [Authorize]
        [HttpGet]
        public IActionResult TestAuthorized()
        {
            return Ok("This is a test endpoint for authorized users.");
        }
    }
}
