using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using WebApiProyecto3.Models;
using System.Data.SqlClient;

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
        public Respuesta Iniciar([FromBody] Usuario usuario)
        {
            System.Diagnostics.Debug.WriteLine("**********usuario*************"+ usuario.Password);
            Respuesta r = new Respuesta();
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
            return r;
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
