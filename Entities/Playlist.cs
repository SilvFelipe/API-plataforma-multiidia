using System.Data;

namespace PIM_VIII.API.Entities
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public bool IsDeleted { get; set; }
        public void Update(string nome)
        {
            this.Nome = nome;
        }
        public void Delete()
        {
            this.IsDeleted = true;
        }
    }
}