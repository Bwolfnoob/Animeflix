using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimeFlixView.ViewModel
{
    public class MainTelaViewModel
    {
        public string Capa { get; set; }
        public string TituloCanal { get; set; }
        public string Descricao { get; set; }
        public string CanalId { get; set; }
        public IList<ItemViewModel> Itens { get; set; }
    }
}
