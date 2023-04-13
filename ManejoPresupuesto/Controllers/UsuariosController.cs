using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class UsuariosController: Controller
    {
        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public UserManager<Usuario> UserManager { get; }
        public SignInManager<Usuario> SignInManager { get; }

        [AllowAnonymous]
        public IActionResult Registro() 
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult>Registro (RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var usuario = new Usuario() { Email = modelo.Email };
            var resultado = await UserManager.CreateAsync(usuario, password: modelo.Password);


            if (resultado.Succeeded)
            {

                await SignInManager.SignInAsync(usuario, isPersistent: true );
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(modelo);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {

                return View(modelo);
            }
            var resultado = await SignInManager.PasswordSignInAsync(modelo.Email,
                modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre del Usuario ó contraseña Incorrecta.");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transacciones");
        }

    }
}
