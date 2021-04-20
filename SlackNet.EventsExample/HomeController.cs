using Microsoft.AspNetCore.Mvc;

namespace SlackNet.EventsExample
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}