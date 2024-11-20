using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class TabelaRelacionamentoPlaylistUsuariosDbContext
    {
        public List<TabelaRelacionamentoPlaylistUsuarios> Relacionamentos {  get; set; }
        public TabelaRelacionamentoPlaylistUsuariosDbContext()
        {
            Relacionamentos = new List<TabelaRelacionamentoPlaylistUsuarios> ();
        }
    }
}
