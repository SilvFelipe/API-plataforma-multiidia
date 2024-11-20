using Microsoft.AspNetCore.Mvc;
using PIM_VIII.API.Entities;
using PIM_VIII.API.Models.Requests;
using PIM_VIII.API.Models.Responses;
using PIM_VIII.API.Persistence;

namespace PIM_VIII.API.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioDbContext _context;
        private readonly PlaylistDbContext _playlistContext;
        private readonly TabelaRelacionamentoPlaylistUsuariosDbContext _relacionamentoPlaylistUsuariosContext;
        private readonly TabelaRelacionamentoPlaylistECriadorPlaylistDbContext _relacionamentoPlaylstCriadorPlaylist;

        public UsuarioController(UsuarioDbContext context, PlaylistDbContext playlistDbContext, TabelaRelacionamentoPlaylistUsuariosDbContext relacionamentoPlaylistUsuariosContext, TabelaRelacionamentoPlaylistECriadorPlaylistDbContext relacionamentoPlaylstCriadorPlaylist)
        {
            _context = context;
            _playlistContext = playlistDbContext;
            _relacionamentoPlaylistUsuariosContext = relacionamentoPlaylistUsuariosContext;
            _relacionamentoPlaylstCriadorPlaylist = relacionamentoPlaylstCriadorPlaylist;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var usuarios = _context.Usuarios.Where(usuario => !usuario.IsDeleted).ToList();

            var response = new List<UsuarioResponse>();

            usuarios.ForEach(user => response.Add(this.MontarUsuarioResponseComPlaylists(user)));

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(Guid id)
        {
            var usuarioResponse = _context.Usuarios.SingleOrDefault(usuario => usuario.Id == id && !usuario.IsDeleted);

            if (usuarioResponse == null)
            {
                return NotFound();
            }

            return Ok(this.MontarUsuarioResponseComPlaylists(usuarioResponse));
        }

        [HttpPost("create")]
        public IActionResult Post(UsuarioRequest usuario)
        {
            var usuarioEntity = new Usuario();
            usuarioEntity.Id = usuario.Id;
            usuarioEntity.Nome = usuario.Nome;
            usuarioEntity.Email = usuario.Email;
            usuarioEntity.IsDeleted = false;

            _context.Usuarios.Add(usuarioEntity);

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(Guid id, UsuarioRequest usuarioRequest)
        {
            var usuarioUpdate = _context.Usuarios.SingleOrDefault(usuario => usuario.Id == id && !usuario.IsDeleted);

            if (usuarioUpdate == null)
            {
                return NotFound();
            }

            usuarioUpdate.Update(usuarioRequest.Nome, usuarioRequest.Email);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var usuarioDeleted = _context.Usuarios.SingleOrDefault(usuario => usuario.Id == id && !usuario.IsDeleted);

            if (usuarioDeleted == null)
            {
                return NotFound();
            }

            usuarioDeleted.Delete();

            return NoContent();
        }

        [HttpGet("add-playlist-into-usuario/{idUsuario}/playlist/{idPlaylist}")]
        public IActionResult AdicionarPlaylist(Guid idUsuario, Guid idPlaylist)
        {
            var usuarioAtual = _context.Usuarios.SingleOrDefault(usuario => usuario.Id == idUsuario && !usuario.IsDeleted);

            if (usuarioAtual == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var playlist = _playlistContext.Playlists.SingleOrDefault(playlist => playlist.Id == idPlaylist && !playlist.IsDeleted);

            if (playlist == null)
            {
                return NotFound("Playlist não encontrada");
            }

            var relacionamentoUsuariosPlaylist = new TabelaRelacionamentoPlaylistUsuarios();
            relacionamentoUsuariosPlaylist.IdPlaylist = idPlaylist;
            relacionamentoUsuariosPlaylist.IdUsuario = idUsuario;
            _relacionamentoPlaylistUsuariosContext.Relacionamentos.Add(relacionamentoUsuariosPlaylist);

            return NoContent();
        }

        [HttpDelete("remove-playlist-from-usuario/{idUsuario}/playlist/{idPlaylist}")]
        public IActionResult ExcluirPlaylist(Guid idUsuario, Guid idPlaylist)
        {
            var usuarioAtual = _context.Usuarios.SingleOrDefault(usuario => usuario.Id == idUsuario && !usuario.IsDeleted);

            if (usuarioAtual == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var playlistUsuario = _playlistContext.Playlists.SingleOrDefault(playlist => playlist.Id == idPlaylist && !playlist.IsDeleted);

            if (playlistUsuario == null)
            {
                return NotFound("Playlist não encontrada");
            }

            var relacionamentoPlaylistUsuario = _relacionamentoPlaylistUsuariosContext.Relacionamentos.SingleOrDefault(relacionamento => relacionamento.IdPlaylist.Equals(idPlaylist) && relacionamento.IdUsuario.Equals(idUsuario));
            if (relacionamentoPlaylistUsuario == null)
            {
                return NotFound("Esse usuário não tem essa playlist");
            }

            _relacionamentoPlaylistUsuariosContext.Relacionamentos.RemoveAt(_relacionamentoPlaylistUsuariosContext.Relacionamentos.IndexOf(relacionamentoPlaylistUsuario));

            return NoContent();
        }
        private UsuarioResponse MontarUsuarioResponseComPlaylists(Usuario usuario)
        {
            var response = new UsuarioResponse();
            response.Id = usuario.Id;
            response.Nome = usuario.Nome;
            response.Email = usuario.Email;
            response.Playlists = new List<Playlist> ();

            var relacionamentoUsuariosPlaylist = _relacionamentoPlaylistUsuariosContext.Relacionamentos.Where(relacionamento => relacionamento.IdUsuario.Equals(usuario.Id)).ToList();
            if (relacionamentoUsuariosPlaylist.Count() > 0)
            {
                List<Guid> playlistsId = new List<Guid> ();
                relacionamentoUsuariosPlaylist.ForEach(r => playlistsId.Add(r.IdPlaylist));
                _playlistContext.Playlists.Where(playlist => playlistsId.Contains(playlist.Id) && !playlist.IsDeleted).ToList().ForEach(play => response.Playlists.Add(play));
            }

            var relacinamentoCriadorPlaylist = _relacionamentoPlaylstCriadorPlaylist.Relacionamentos.Where(relacionamento => relacionamento.IdCriadorPlaylist.Equals(usuario.Id)).ToList();
            if (relacinamentoCriadorPlaylist.Count() > 0)
            {
                List<Guid> playlistsCriadasId = new List<Guid>();
                relacinamentoCriadorPlaylist.ForEach(r => playlistsCriadasId.Add(r.IdPlaylist));
                _playlistContext.Playlists.Where(playlist => playlistsCriadasId.Contains(playlist.Id) && !playlist.IsDeleted).ToList().ForEach(play => response.Playlists.Add(play));
            }

            return response;
        }
    }
}
