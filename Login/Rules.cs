using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recepcion.Common;
using Recepcion.Data;

namespace Recepcion.Logic
{
    public class Rules
    {
        
        public Usuario validateUser(string usuario, string password)
        {
            return new DB().validateUser(usuario, password);
        }

        public string ProximoPago(string usuario)
        {
            return new DB().ProximoPago(usuario);
        }

        public List<Usuario> getUsers()
        {
            return new DB().getUsers();    
        }

        public bool eliminarUsuario(string id)
        {
            return new DB().eliminarUsuario(id);
        }

        public List<Usuario> getUsers(string usuario)
        {
            return new DB().getUsers(usuario);
        }

        public List<Servicio> getServices()
        {
            return new DB().getServices();
        }


        public List<Servicio> getServices(string usuario)
        {
            return new DB().getServices(usuario);
        }

        public string isInOut(string usr)
        {
            return new DB().isInOut(usr);
        }

        public bool registrarSalida(string usr)
        {
            return new DB().registrarSalida(usr);
        }

        public List<Horario> getHorarios(string day, string usuario)
        {
            return new DB().getHorarios(day, usuario);
        }

        public bool registrarEntrada(List<int> horarios, string usuario)
        {
            return new DB().registrarEntrada(horarios, usuario);
        }

        public bool registrarEntrada(string usuario)
        {
            return new DB().registrarEntrada(usuario);
        }
    }
}
