using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace ManejoPresupuesto.Servicios
{
    //creamos la interface
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones //implementamos la interface en la clase
    {
        private readonly string connectionString;
        //usamos el constructor
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id= id;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>
                (@"SELECT t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoria, 
                    cu.Nombre as Cuenta, c.TipoOperacionId
                    from Transacciones t
                    inner join Categorias c
                    on c.Id = t.CategoriaId
                    inner join Cuentas cu
                    on cu.Id = t.CuentaId
                    where t.CuentaId = @CuentaId and t.UsuarioId = @UsuarioId
                    and FechaTransaccion between @FechaInicio and @FechaFin", 
                    modelo);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(
            ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Transaccion>
                (@"SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre AS Categoria,
                    CU.Nombre AS Cuenta, C.TipoOperacionId
		            FROM  Transacciones T
		            INNER JOIN  Categorias C 
                    ON C.Id = T.CategoriaId
		            INNER JOIN  Cuentas CU 
                    ON CU.Id = T.CuentaId
		            WHERE T.UsuarioId = @UsuarioId 
                    AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
		            ORDER BY  T.fechaTransaccion DESC", modelo);          
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, 
            int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar", 
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT Transacciones.*, cat.TipoOperacionId
                FROM Transacciones  
                INNER JOIN Categorias cat
                ON cat.Id = Transacciones.CategoriaId
                WHERE Transacciones.Id = @Id AND  Transacciones.UsuarioId = @UsuarioId",
                new { id, usuarioId});
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>>ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo) 
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"
                    SELECT  DATEDIFF (d, @fechaInicio, FechaTransaccion) / 7 + 1 AS Semana,
                    SUM(Monto) AS Monto, cat.TipoOperacionId
                    FROM Transacciones
                    INNER JOIN Categorias cat
                    ON cat.Id = Transacciones.CategoriaId
                    WHERE Transacciones.UsuarioId = @UsuarioId AND 
                    FechaTransaccion BETWEEN @fechaInicio AND @fechaFin
                    GROUP BY DATEDIFF (d, @fechaInicio, FechaTransaccion) /7, cat.TipoOperacionId
                    ", modelo);
        
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
