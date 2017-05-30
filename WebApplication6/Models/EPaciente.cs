using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication6.Models
{
    public class EPaciente
    {


        public int Historia { get; set; }

        [Required(ErrorMessage = "Dato Requeridos")]
        public string ApePat { get; set; }
        [Required(ErrorMessage = "Dato Requeridos")]
        public string ApeMat { get; set; }
    
        public string NomPac { get; set; }

        public DateTime FecNac { get; set; }
        [Required(ErrorMessage = "Dato Requeridos")]
        public string NumDoc { get; set; }
        public bool estado { get; set; }
    }
}