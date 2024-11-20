using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Models.Responses
{
    public class CriadorResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public List<Conteudo> Conteudos { get; set; }
    }
}
