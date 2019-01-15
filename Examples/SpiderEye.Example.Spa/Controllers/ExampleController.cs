using System.Threading.Tasks;
using SpiderEye.Mvc;
using SpiderEye.Routing;

namespace SpiderEye.Example.Spa.Controllers
{
    [Route("api/v1/[controller]")]
    public class ExampleController : Controller
    {
        [HttpGet]
        public string GetExample(string name)
        {
            return $"Here is an example of your parameter: \"{name ?? "<null>"}\"";
        }

        [HttpGet]
        public async Task<string> GetSlowExample(string name)
        {
            await Task.Delay(1000);
            return $"Here is a slow example of your parameter: \"{name ?? "<null>"}\"";
        }
    }
}
