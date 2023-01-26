using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class CuentaCreacionViewModel: Cuenta //herencia, tiene todo lo de cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; } //Ienumrable es una lista
    }
}
