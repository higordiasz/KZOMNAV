using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KZOMNAV.Models.Blocks
{
    class Block
    {
        public string Username { get; set; }

        public DateTime Inicio { get; set; }
        
        public int Minutes { get; set; }
    }

    static class ExtendBlock
    {
        static public async Task LimparLista (this List<Block> List)
        {
            var aux = List;
            for (var i = 0; i < aux.Count; i++)
            {
                TimeSpan timer = DateTime.Now - aux[i].Inicio;
                if (timer.TotalMinutes >= aux[i].Minutes)
                {
                    List.Remove(aux[i]);
                }
            }
        }

        static public async Task AdicionarBlock (this List<Block> List, string username, int minutes)
        {
            Block b = new Block();
            b.Inicio = DateTime.Now;
            b.Minutes = minutes;
            b.Username = username;
        }

    }
}
