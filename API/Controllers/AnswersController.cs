using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnswersController> _logger;

        public AnswersController(IConfiguration configuration, ILogger<AnswersController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("user")]
        public IActionResult GetUser()
        {
            var result = new
            {
                name = _configuration["Identifiers:name"],
                token = _configuration["Identifiers:token"]
            };

            return Ok(result);

        }
    }
}
