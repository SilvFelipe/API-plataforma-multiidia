using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class UsuarioDbContext
    {
        public List<Usuario> Usuarios { get; set; }
        public UsuarioDbContext() 
        {
            Usuarios = new List<Usuario>();
        }
    }
}
