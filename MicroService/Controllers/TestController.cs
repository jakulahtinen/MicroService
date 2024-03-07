using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MicroService.TestServices;

namespace MicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITransientService _transientService;
        private readonly IScopedService _scopedService;
        private readonly ISingletonService _singletonService;



        public TestController(ITransientService transientService,
                              IScopedService scopedService,
                              ISingletonService singletonService)
        {
            _transientService = transientService;
            _scopedService = scopedService;
            _singletonService = singletonService;
        }

        [HttpGet("Serve")]
        public IActionResult Get()
        {
            _transientService.Serve();
            _scopedService.Serve();
            _singletonService.Serve();

            return Ok("Services used.");
        }
    }
}