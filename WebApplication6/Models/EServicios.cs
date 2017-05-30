using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication6.Models
{
    public class EServicios
    {
        public string codServicio { get; set; }
        [Required(ErrorMessage="Dato Requerido")]
        public string nomServicio { get; set; }
        public string CodEspec { get; set; }
        public string CodEmp { get; set; }
        public string codSed { get; set; }    
        public bool Estado { get; set; }
        public string codTipTar { get; set; }
        public string descTipTar { get; set; }
        public int porcentaje { get; set; }
    }
}