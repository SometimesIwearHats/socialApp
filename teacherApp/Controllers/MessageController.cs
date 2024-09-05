using Microsoft.AspNetCore.Mvc;

namespace socialApp.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
