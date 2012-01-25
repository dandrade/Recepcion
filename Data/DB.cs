using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data;
using MySql.Data.MySqlClient;

using Recepcion.Common;

namespace Recepcion.Data
{
    public class DB
    {
        public Usuario validateUser(string user, string password)
        {
            
            string consulta = "select * from usuarios where usuario = '"+user+"' and password = md5('"+password+"') limit 1";
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            Usuario usuario = new Usuario();

            while (reader.Read())
            {
                usuario.idUsuario = (int)reader["id"];
                usuario.User = reader["usuario"].ToString();
                usuario.Password = reader["password"].ToString();
                usuario.Nombre = reader["nombre"].ToString();
                usuario.ApellidoPaterno = reader["apellidoPaterno"].ToString();
                usuario.ApellidoMaterno = reader["apellidoMaterno"].ToString();
                usuario.LugarNacimiento = reader["lugarNacimiento"].ToString();
                usuario.FechaNacimiento = reader["fechaNacimiento"].ToString();
                usuario.Direccion = reader["direccion"].ToString();
                usuario.Colonia = reader["colonia"].ToString();
                usuario.CP = reader["cp"].ToString();
                usuario.Municipio = reader["municipio"].ToString();
                usuario.Telefono = reader["telefono"].ToString();
                usuario.Celular = reader["celular"].ToString();
                usuario.TelefonoAdicional = reader["telefonoAdicional"].ToString();
                usuario.Foto = reader["foto"].ToString();
                usuario.RolUser = reader["rol"].ToString();
                usuario.Activo = (bool)reader["activo"];
                usuario.Logged = true;

            }

            return usuario;
        }

        public string ProximoPago(string usuario)
        {
            string result = String.Empty;
            string ultimoPago = String.Empty;

            string consulta = "select mensualidad from pagos where usuario = " + usuario + " and (concepto = 'Mensualidad' or concepto = 'Inscripcion' ) order by timestamp desc limit 1";
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            while (reader.Read())
            {
                ultimoPago = reader["mensualidad"].ToString();
                
                int year = Convert.ToInt32("20" + ultimoPago.Split(new char[] { '/' })[2].ToString());
                int month = Convert.ToInt32(ultimoPago.Split(new char[] { '/' })[1].ToString());
                int day = Convert.ToInt32(ultimoPago.Split(new char[] { '/' })[0].ToString());
                
                DateTime ultimoPagoDate = new DateTime(year, month, day);
                
                DateTime dateNow = DateTime.Now.AddDays(-1);
                TimeSpan ts = dateNow - ultimoPagoDate;
                
                int days = ts.Days;
                int total = 0;
                if (days >= 25 && days <= 30)
                {
                    int dias = Convert.ToInt32(days.ToString().Replace('-', ' ').Trim());
                    total = 30 - days;
                    if (total <= 5)
                    {
                        result += "Faltan" + total.ToString().Replace('-', ' ') + " dia(s) para su mensualidad";
                    }
                }
                else if (days <= 0 || days <= 13)
                {
                    result = "";
                }
                else if (days > 30)
                {
                    result += "Mensualidad vencida le invitamos a pagar";
                }
                
            }
            
            return result;
        }

        public List<Usuario> getUsers()
        {
            string consulta = "select * from usuarios";
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            List<Usuario> usuarios = new List<Usuario>();
            

            while (reader.Read())
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = (int)reader["id"];
                usuario.User = reader["usuario"].ToString();
                usuario.Password = reader["password"].ToString();
                usuario.Nombre = reader["nombre"].ToString();
                usuario.ApellidoPaterno = reader["apellidoPaterno"].ToString();
                usuario.ApellidoMaterno = reader["apellidoMaterno"].ToString();
                usuario.LugarNacimiento = reader["lugarNacimiento"].ToString();
                usuario.FechaNacimiento = reader["fechaNacimiento"].ToString();
                usuario.Direccion = reader["direccion"].ToString();
                usuario.Colonia = reader["colonia"].ToString();
                usuario.CP = reader["cp"].ToString();
                usuario.Municipio = reader["municipio"].ToString();
                usuario.Telefono = reader["telefono"].ToString();
                usuario.Celular = reader["celular"].ToString();
                usuario.TelefonoAdicional = reader["telefonoAdicional"].ToString();
                usuario.Mensualidad = reader["mensualidad"].ToString();
                usuario.Foto = reader["foto"].ToString();
                usuario.RolUser = reader["rol"].ToString();
                usuario.Activo = (bool)reader["activo"];

                usuarios.Add(usuario);

            }

            return usuarios;
        }

        public List<Usuario> getUsers(string usr)
        {
            string consulta = "select * from usuarios where nombre like '%"+usr+"%' or apellidoPaterno like '%"+usr+"%' or apellidoMaterno like '%"+usr+"%'";
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            List<Usuario> usuarios = new List<Usuario>();


            while (reader.Read())
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = (int)reader["id"];
                usuario.User = reader["usuario"].ToString();
                usuario.Password = reader["password"].ToString();
                usuario.Nombre = reader["nombre"].ToString();
                usuario.ApellidoPaterno = reader["apellidoPaterno"].ToString();
                usuario.ApellidoMaterno = reader["apellidoMaterno"].ToString();
                usuario.LugarNacimiento = reader["lugarNacimiento"].ToString();
                usuario.FechaNacimiento = reader["fechaNacimiento"].ToString();
                usuario.Direccion = reader["direccion"].ToString();
                usuario.Colonia = reader["colonia"].ToString();
                usuario.CP = reader["cp"].ToString();
                usuario.Municipio = reader["municipio"].ToString();
                usuario.Telefono = reader["telefono"].ToString();
                usuario.Celular = reader["celular"].ToString();
                usuario.TelefonoAdicional = reader["telefonoAdicional"].ToString();
                usuario.Foto = reader["foto"].ToString();
                usuario.RolUser = reader["rol"].ToString();
                usuario.Activo = (bool)reader["activo"];

                usuarios.Add(usuario);

            }

            return usuarios;
        }

        

        public List<Servicio> getServices()
        {
            string consulta = "select * from servicios where tipo = 'Ejercicio'";
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            List<Servicio> servicios = new List<Servicio>();


            while (reader.Read())
            {
                Servicio servicio = new Servicio();
                servicio.idServicio = (int)reader["id"];
                servicio.Nombre = reader["nombre"].ToString();
                servicio.Descripcion = reader["descripcion"].ToString();
                servicio.Costo = reader["costo"].ToString();
                servicio.Tipo = reader["tipo"].ToString();
                servicios.Add(servicio);

            }

            return servicios;
        }

        public List<Servicio> getServices(string usuario)
        {
            string consulta = "select su.id, s.nombre, s.descripcion, s.costo, s.tipo from servicios_usuarios su join servicios s on s.id = su.servicio where su.usuario = "+usuario+" and su.tipo = 'Cliente'";
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            List<Servicio> servicios = new List<Servicio>();


            while (reader.Read())
            {
                Servicio servicio = new Servicio();
                servicio.idServicio = (int)reader["id"];
                servicio.Nombre = reader["nombre"].ToString();
                servicio.Descripcion = reader["descripcion"].ToString();
                servicio.Costo = reader["costo"].ToString();
                servicio.Tipo = reader["tipo"].ToString();
                servicios.Add(servicio);

            }

            return servicios;
        }

        
        public List<Horario> getHorarios(string day, string usuario)
        {
            Log.WriteLog("Obteniendo Horarios.");
            string consulta = "select h.id, s.nombre, s.descripcion, h.inicio, h.fin, (select CONCAT(nombre, ' ', apellidoPaterno, ' ', apellidoMaterno)  from usuarios where id = h.instructor) as instructor from usuarios as u join servicios_usuarios as su on su.usuario = u.id join servicios as s on s.id = su.servicio join horarios as h on h.servicio = su.servicio where su.tipo = 'Cliente' and h."+day+" = 1 and u.id = "+usuario+" order by h.inicio";
            
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            List<Horario> horarios = new List<Horario>();


            while (reader.Read())
            {
                Horario horario = new Horario();
                horario.ID = (int)reader["id"];
                horario.Nombre = reader["nombre"].ToString();
                horario.Descripcion = reader["descripcion"].ToString();
                horario.Inicio = reader["inicio"].ToString();
                horario.Fin = reader["fin"].ToString();
                horario.Instructor = reader["instructor"].ToString();
                horarios.Add(horario);

            }
            Log.WriteLog("Regresando los Horarios.");
            return horarios;
        }

        

        public string isInOut(string usr)
        {
            string consulta = "select id, fecha, timestamp, fecha_salida, salida, tipo, usuario from visitas v where fecha = '"+this.getFecha()+"' and usuario = "+usr;
            MySqlDataReader reader = Ejecutar.ExecuteSQL(consulta);

            string result = "In";
            
            while (reader.Read())
            {
                string salida = reader["fecha_salida"].ToString();
                if (String.IsNullOrEmpty(salida))
                {
                    result = "Out";
                }
            }
            Log.WriteLog("Verificando si va a entrar o a salir, resultado: " + result);
            return result;
        }

        public bool eliminarUsuario(string id)
        { 
            string query = "delete from usuarios where id = "+id;
            if (Ejecutar.ExecuteNonSQL(query) > 0)
                return true;
            else
                return false;
        }

        private string getFecha()
        {
            string[] fechaA = DateTime.Now.ToShortDateString().Split(new char[] { '/' });
            string dia = fechaA[0].ToString();
            string mes = fechaA[1].ToString();
            string ano = fechaA[2].ToString().Replace("20", "");
            string fecha = dia + "/" + mes + "/" + ano;
            return fecha;
        }

        private string getFechaUSD()
        {
            string[] fechaA = DateTime.Now.ToShortDateString().Split(new char[] { '/' });
            string dia = fechaA[0].ToString();
            string mes = fechaA[1].ToString();
            string ano = fechaA[2].ToString();
            string fecha = ano + "-" + mes + "-" + dia + " " + DateTime.Now.ToString("HH:mm:ss");
            return fecha;
        }

        public bool registrarEntrada(List<int> horarios, string usuario)
        {
            string fecha = this.getFecha();
            int count = 0;
            foreach (int id in horarios)
            {
                string insert = "insert into visitas (horario, fecha, usuario) values ("+id+",'"+fecha+"',"+usuario+")";
                
                if(Ejecutar.ExecuteNonSQL(insert) > 0)
                    count++;
            }

            if (count > 0)
                return true;
            else
                return false;
        }

        public bool registrarEntrada(string usuario)
        {
            string fecha = this.getFecha();
            
            string insert = "insert into visitas (horario, fecha, usuario) values (3,'" + fecha + "'," + usuario + ")";

            if (Ejecutar.ExecuteNonSQL(insert) > 0)
                return true;
            else
                return false;
        }

        public bool registrarSalida(string usuario)
        {
            string insert = "update visitas set fecha_salida = '" + this.getFecha() + "', salida = '" + this.getFechaUSD() + "' where usuario = " + usuario + " and fecha = '" + this.getFecha() + "' and fecha_salida is null";
            if (Ejecutar.ExecuteNonSQL(insert) > 0)
                return true;
            else
                return false;
        }
    }
}
    