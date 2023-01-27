using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        [Display(Name = "Fecha Transaccion")]
        public int UsuarioId { get; set; }
        [Display(Name = "Usuario Id")]
        [DataType(DataType.DateTime)]
        //public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:MM tt"));
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("g"));
        public decimal Monto { get; set; }
        [Range( 1, maximum: int.MaxValue, ErrorMessage ="Debe seleccionar una categoria")]
        [Display(Name = "Categoria Id")]
        public int CategoriaId { get; set; }
        [StringLength(maximumLength:1000, ErrorMessage ="La Nota no debe superar los {1} caracteres")]
        public string Nota { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        [Display(Name = "Cuenta Id")]
        public int CuentaId { get; set; }
        [Display(Name ="Tipo Operacion")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;
        public string Cuenta { get; set; }
        public string Categoria { get; set; }

    }
}
