using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using U3RazasPerros.Models;

namespace U3RazasPerros.Areas.Admin.Models
{
    public class RazasIndexViewModel
    {
        public Razas Razas { get; set; }
        public IEnumerable<U3RazasPerros.Models.Paises> Paises { get; set; }
        public IEnumerable<U3RazasPerros.Models.Caracteristicasfisicas> CategoriasFisicas { get; set; }
    }
}
