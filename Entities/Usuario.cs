using System.ComponentModel.DataAnnotations;

namespace PIM_VIII.API.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }

        public void Update(string Nome, string Email)
        {
            this.Nome = Nome;
            this.Email = Email;
        }
        public void Delete()
        {
            this.IsDeleted = true;
        }
    }
}
