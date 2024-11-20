using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Models.Responses
{
    public class ConteudoResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public Criador Criador { get; set; }
    }
}
