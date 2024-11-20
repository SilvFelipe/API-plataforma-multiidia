using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Models.Responses
{
    public class UsuarioResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public List<Playlist> Playlists { get; set; }
    }
}
