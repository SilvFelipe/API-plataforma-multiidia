using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class TabelaRelacionamentoPlaylistECriadorPlaylistDbContext
    {
        public List<TabelaRelacionamentoPlaylistECriadorPlaylist> Relacionamentos { get; set; }
        public TabelaRelacionamentoPlaylistECriadorPlaylistDbContext()
        {
            Relacionamentos = new List<TabelaRelacionamentoPlaylistECriadorPlaylist> ();
        }
    }
}
