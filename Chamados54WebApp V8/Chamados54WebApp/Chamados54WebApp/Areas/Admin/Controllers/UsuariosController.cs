using Chamados54WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chamados54WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        BancoDados bancoDados;
        [HttpGet]
        public IActionResult Index()
        {
            bancoDados = new BancoDados();
            var usuarios = bancoDados.Usuarios.ToList();
            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string busca)
        {
            bancoDados =new BancoDados();
            var usuarios = new List<Usuario>();
            if(string.IsNullOrWhiteSpace(busca))
            {
                 usuarios = bancoDados.Usuarios.ToList();
            }
            else
            {
                usuarios = bancoDados.Usuarios.Where(e => e.Email.Contains(busca)).ToList();
            }
            return View(usuarios);
        }
    }
}
