using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KZOMNAV.Models.Retornos
{
    class Retorno
    {
        public string Mensagem { get; set; }
        public int Status { get; set; }
    }

    class Retorno_Two
    {
        public int Status { get; set; }
        public string Response { get; set; }
    }

}
