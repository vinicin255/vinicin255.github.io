using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Chamados54WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador,Tecnico,Cliente")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
