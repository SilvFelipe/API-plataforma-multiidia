using System.Data;

namespace PIM_VIII.API.Entities
{
    public class Conteudo
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public bool IsDeleted { get; set; }
        public void Update(string titulo, string tipo)
        {
            this.Titulo = titulo;
            this.Tipo = tipo;
        }
        public void Delete()
        {
            this.IsDeleted = true;
        }
    }
}