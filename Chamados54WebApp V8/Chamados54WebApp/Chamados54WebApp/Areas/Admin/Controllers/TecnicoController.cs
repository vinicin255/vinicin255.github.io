using Chamados54WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chamados54WebApp.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class TecnicoController : Controller
    {
        BancoDados bancoDados;
        [HttpGet]
        public IActionResult Index()
        {
            bancoDados = new BancoDados();
            var tecnicos = bancoDados.Tecnicos.ToList();
            return View(tecnicos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string busca)
        {
            bancoDados = new BancoDados();
            var tecnicos = new List<Tecnico>();
            if (string.IsNullOrWhiteSpace(busca))
            {
                tecnicos = bancoDados.Tecnicos.ToList();
            }
            else
            {
                tecnicos = bancoDados.Tecnicos.Where(e => e.Nome.Contains(busca)).ToList();
            }
            return View(tecnicos);
        }
    }
}
