﻿using Chamados54WebApp.Data;
using Chamados54WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chamados54WebApp.Controllers
{
    public class ContaController : Controller
    {
        BancoDados bancoDados;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            ContaViewModel conta = new ContaViewModel();
            return View(conta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastro(ContaViewModel conta)
        {
            //se os dados sao validos
            if (ModelState.IsValid)
            {
                //cadastro do usuario
                Usuario usuario = new Usuario();
                usuario.Email = conta.Email;
                usuario.Senha = conta.Senha;
                usuario.Perfil = conta.Perfil;
                //cadastra o usuario no banco de dados
                bancoDados = new BancoDados(); //inicia o banco de dados
                bancoDados.Usuarios.Add(usuario); //comando insert
                bancoDados.SaveChanges(); //executa o comando                
                if (conta.Perfil == PerfilUsuario.Cliente)
                {
                    //cadastra o cliente
                    Cliente cliente = new Cliente();
                    cliente.Id = usuario.Id;
                    cliente.Nome = conta.Nome;
                    bancoDados.Clientes.Add(cliente); //comando insert
                    bancoDados.SaveChanges(); //executa o comando                
                }
                if (conta.Perfil == PerfilUsuario.Tecnico)
                {
                    //cadastra o tecnico
                    Tecnico tecnico = new Tecnico();
                    tecnico.Id = usuario.Id;
                    tecnico.Nome = conta.Nome;
                    bancoDados.Tecnicos.Add(tecnico); //comando insert
                    bancoDados.SaveChanges();
                }
                                
                return RedirectToAction("Index", "Home");
            }
            return View(conta);
        }


        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel login = new LoginViewModel();
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                //obtem o usuario do login
                bancoDados = new BancoDados();
                var usuario = bancoDados.Usuarios
                    .FirstOrDefault(e => e.Email == login.Email && e.Senha == login.Senha);
                if (AutenticaUsuario(usuario))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });                  
                }
                else
                {
                    ModelState.AddModelError("Senha", "Usuário ou senha inválidos");
                }                
            }
            return View(login);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        //metodos
        public bool AutenticaUsuario(Usuario usuario)
        {
            if (usuario != null)
            {
                bancoDados = new BancoDados();
                string nome = string.Empty;
                switch (usuario.Perfil)
                {
                    case PerfilUsuario.Cliente:
                        var cliente = bancoDados.Clientes.FirstOrDefault(e => e.Id == usuario.Id);
                        nome = cliente.Nome;
                        break;
                    case PerfilUsuario.Tecnico:
                        var tecnico = bancoDados.Tecnicos.FirstOrDefault(e => e.Id == usuario.Id);
                        break;
                    case PerfilUsuario.Administrador:
                        nome = "Administrador";
                        break;                    
                }

                //credencial do usuario
                var credencial = new List<Claim>();
                credencial.Add(new Claim(ClaimTypes.Name, nome));
                credencial.Add(new Claim(ClaimTypes.Email, usuario.Email));
                credencial.Add(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
                credencial.Add(new Claim(ClaimTypes.Role, usuario.Perfil.ToString()));

                //configura a identidade de acesso
                var identidade = new ClaimsIdentity(credencial, 
                    CookieAuthenticationDefaults.AuthenticationScheme);

                //configura a autenticacao no servidor
                var autenticacaoCookie = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IssuedUtc = DateTime.UtcNow, //inicio do tempo 
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30), //termino do tempo
                    //RedirectUri = @"~/"
                };

                //autentica o usuario no servidor por Cookies
                var autenticacaoUsuario = new ClaimsPrincipal(identidade);
                HttpContext.SignInAsync(autenticacaoUsuario, autenticacaoCookie);

                //redireciona para o painel administrativo
                return true;
            }

            return false;
        }
    }
}
