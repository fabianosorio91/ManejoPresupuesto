using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        //[Display (Name = "Nombre del tipo cuenta")]
        [Remote(action: "VerificarExisteTipoCuenta", controller:"TiposCuentas", AdditionalFields =nameof(Id))]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        /*
        --------------Pruebas de otras validaciones-------------------
         [Emai [Range (minimum : 18, maximun: 100, ErrorMessage = "La edad debe estar entre {1} y {2} años ")]
         [Url  (ErrorMessage = "El campo debe ser una URL valida")]
         [StringLength (maximumLength:50, MinimumLength = 3, ErrorMessage = "La longitud del campo {0} debe de estar entre {2} y {1} caracteres")]

        -------validaciones personalizadas-----------
         */


    }
}
