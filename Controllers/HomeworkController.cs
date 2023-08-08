using Microsoft.AspNetCore.Mvc;

namespace AjaxDemo.Controllers
{
    public class HomeworkController : Controller
    {
        public IActionResult Jquery()
        {
            return View();
        }
        public IActionResult Travel()
        {
            return View();
        }
    }
}
