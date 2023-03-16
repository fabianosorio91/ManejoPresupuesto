using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Authentication;
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

        public IActionResult Registro() 
        {
            return View();
        }
        [HttpPost]

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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transacciones");
        }

    }
}
