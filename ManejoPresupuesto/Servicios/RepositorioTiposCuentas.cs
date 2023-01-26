using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    //Abstraccion de dependencias
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        //codigo para insertar un dato en la base de datos
        //contructor
        public RepositorioTiposCuentas(IConfiguration configuration)//para acceder al conectionString
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar",
                                       new {usuarioId = tipoCuenta.UsuarioId,
                                       nombre = tipoCuenta.Nombre},
                                       commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            //query para verificar la existencia de un registro, sin traer la data de ese registro
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"
                                     SELECT 1
                                     FROM TiposCuentas
                                     WHERE Nombre = @Nombre AND UsuarioID = @UsuarioId",
                                     new {nombre, usuarioId});

            return existe == 1;        
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"
                                     SELECT Id, Nombre, Orden
                                     FROM TiposCuentas
                                     WHERE UsuarioId = @UsuarioId
                                     ORDER BY Orden", 
                                     new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        { 
            using var connetion = new SqlConnection(connectionString);
            await connetion.ExecuteAsync(@"
                                     UPDATE TiposCuentas 
                                     SET Nombre = @Nombre
                                     WHERE Id = @Id",
                                     tipoCuenta);

        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connetion = new SqlConnection(connectionString);
            return await connetion.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                                      SELECT Id, Nombre, Orden 
                                      FROM TiposCuentas 
                                      WHERE Id = @Id AND UsuarioId = @UsuarioId",
                                      new {id, usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                                          DELETE TiposCuentas
                                          WHERE Id = @Id",
                                          new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados) {

            var query = "UPDATE tiposCuentas SET Orden = @Orden WHERE Id = @Id;";
            using var connetion = new SqlConnection(connectionString);
            await connetion.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
