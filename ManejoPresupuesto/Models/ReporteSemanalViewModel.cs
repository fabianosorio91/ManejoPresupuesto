namespace ManejoPresupuesto.Models
{
    public class ReporteSemanalViewModel
    {
        public decimal Ingresos => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public decimal Gastos => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public decimal Total => Ingresos - Gastos;
        public DateTime Fechareferencia { get; set; }
        public IEnumerable<ResultadoObtenerPorSemana>TransaccionesPorSemana { get; set; }
    }
}
