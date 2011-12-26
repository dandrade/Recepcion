using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recepcion.Common
{
    public class Horario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Inicio { get; set; }
        public string Fin { get; set; }
        public string Instructor { get; set; }
    }
}
