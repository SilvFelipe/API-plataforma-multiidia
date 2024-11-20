using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class ConteudoDbContext
    {
        public ConteudoDbContext()
        {
            Conteudos = new List<Conteudo>();
        }
        public List<Conteudo> Conteudos { get; set; }
    }
}
