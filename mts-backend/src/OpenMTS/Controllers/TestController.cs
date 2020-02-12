using Microsoft.AspNetCore.Mvc;

namespace OpenMTS.Controllers
{
    [Route("/")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult TestAction()
        {
            return Ok();
        }
    }
}
