using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class TabelaRelacionamentoCriadorConteudoDbContext
    {
        public List<TabelaRelacionamentoCriadorConteudo> Relacionamentos {  get; set; }

        public TabelaRelacionamentoCriadorConteudoDbContext()
        {
            Relacionamentos = new List<TabelaRelacionamentoCriadorConteudo> ();
        }
    }
}
