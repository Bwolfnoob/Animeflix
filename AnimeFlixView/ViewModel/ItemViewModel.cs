using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimeFlixView.ViewModel
{
    public class ItemViewModel
    {
        public ItemViewModel(string categoria)
        {
            Categoria = categoria;
            Videos = new List<VideoViewModel>();
        }

        public string Categoria { get; set; }
        public IList<VideoViewModel> Videos { get; set; }
    }
}
