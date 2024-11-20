using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class TabelaRelacionamentoPlaylistConteudoDbContext
    {
        public List<TabelaRelacionamentoPlaylistConteudo> Relacionamentos { get; set; }
        public TabelaRelacionamentoPlaylistConteudoDbContext()
        {
            Relacionamentos = new List<TabelaRelacionamentoPlaylistConteudo>();
        }
    }
}
