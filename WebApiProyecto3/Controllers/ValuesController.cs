using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using WebApiProyecto3.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Routing;
using System.Data;
using System.Collections;
//PROBANDOOOOOOOO
namespace WebApiProyecto3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public ValuesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            System.Diagnostics.Debug.WriteLine("JSON-----------------------" + id);
            return "value";
        }


        // POST api/values

        [HttpPost]
        [Route("registrarse")]
        public Respuesta Registrar([FromBody] Usuario usuario)
        {
            Respuesta r = new Respuesta();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //string sql = $"EXEC registra_centro_entrenamiento 'Gym', '123', 'Gimnasio crossfit', 'Costa Rica', '25897589', 'gym@gmail.com', 100, 25, 'mastercard.png'";
               
               string sql = $"EXEC registra_centro_entrenamiento '{usuario.Nombre}','{usuario.Password}',' {usuario.Descripcion}'" +
               $",'{usuario.Ubicacion}','{usuario.Tel}','{usuario.Correo}', {usuario.Capacidad} , {usuario.porcentaje}" +
               $",'{usuario.Logo}'";

                

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var dataReader = command.ExecuteReader())
                    {

                        while (dataReader.Read())
                        {
                            r.Resultado = dataReader["resultado"].ToString();
                           
                        }
                    }
                }
                connection.Close();
            }
            return r;
        }

        [HttpPost]
        [Route("iniciar")]
        public Usuario Iniciar([FromBody] Usuario usuario)
        {
            System.Diagnostics.Debug.WriteLine("**********usuario*************" + usuario.Password);
            Respuesta r = new Respuesta();
            Usuario u = new Usuario();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = $"EXEC iniciar_centro_entrenamiento '{usuario.Nombre}','{usuario.Password}'";

                using (var command = new SqlCommand(sql, connection))
                {

                    using (var dataReader = command.ExecuteReader())
                    {

                        while (dataReader.Read())
                        {
                            r.Resultado = dataReader["resultado"].ToString();
                        }
                    }
                }
                connection.Close();
            }

            if (r.Resultado == "1")
            {

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = $"EXEC info_CentrosEntrenamiento '{usuario.Nombre}','{usuario.Password}'";

                    using (var command = new SqlCommand(sql, connection))
                    {

                        using (var dataReader = command.ExecuteReader())
                        {

                            while (dataReader.Read())
                            {

                                u.Nombre = dataReader["NombreGYM"].ToString();
                                u.Descripcion = dataReader["Descripcion"].ToString();
                                u.Capacidad = dataReader["Capacidad"].ToString();
                                u.porcentaje = dataReader["PorcentajePermitido"].ToString();
                                u.Logo = dataReader["Logo"].ToString();
                                u.Ubicacion = dataReader["Ubicación"].ToString();
                                u.Tel = dataReader["teléfono"].ToString();
                                u.Correo = dataReader["Correo"].ToString();

                            }
                        }
                    }
                    connection.Close();
                }

                return u;
            }
            else
            {
                u.Nombre = "0";
                u.Descripcion = "0";
                u.Capacidad = "0";
                u.porcentaje = "0";
                u.Logo = "0";
                u.Ubicacion = "0";
                u.Tel = "0";
                u.Correo = "0";

                return u;
            }

        }

        [HttpPost]
        [Route("loginCliente")]
        public String loginCliente([FromBody]Cliente cliente)
        {
            String salida = "";

            if (ModelState.IsValid)
            {
                string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = $"EXEC login_clientes '{cliente.usuario}','{cliente.password}'";
                    using (SqlCommand command = new SqlCommand(sqlQuery,connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            salida = reader[0].ToString();
                        }
                        connection.Close();
                    }
                }
            }

            return salida;
        }

        [HttpGet]
        [Route("consultarGimnasios")]
        public List<Usuario> consultarClienteCentros()
        {
            List<Usuario> usuarios = new List<Usuario>();

            if (ModelState.IsValid)
            {
                string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = $"EXEC consultar_cliente_centros";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Usuario temp = new Usuario();
                            temp.id = reader[0].ToString();
                            temp.Logo = reader[1].ToString();
                            temp.Nombre = reader[2].ToString();
                            temp.Ubicacion = reader[3].ToString();

                            usuarios.Add(temp);
                        }
                        connection.Close();
                    }
                }
            }

            return usuarios;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
