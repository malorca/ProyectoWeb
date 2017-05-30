using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.Models
{
    public class EServicioTipoTarifa
    {
        public string codServ { get; set; }
        public string codTipTar { get; set; }
        public string Descripcion { get; set; }
        public decimal porcentaje { get; set; }
    }
}