using Microsoft.AspNetCore.Mvc;
using PIM_VIII.API.Entities;
using PIM_VIII.API.Models.Requests;
using PIM_VIII.API.Models.Responses;
using PIM_VIII.API.Persistence;

namespace PIM_VIII.API.Controllers
{
    [Route("api/playlist")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistDbContext _context;
        private readonly UsuarioDbContext _usuarioDbContext;
        private readonly ConteudoDbContext _conteudoDbContext;
        private readonly TabelaRelacionamentoPlaylistECriadorPlaylistDbContext _relacionamentoPlaylistCriadorPlaylistDbContext;
        private readonly TabelaRelacionamentoPlaylistConteudoDbContext _relacionamentoPlaylistConteudoContext;
        private readonly TabelaRelacionamentoPlaylistUsuariosDbContext _relacionamentoPlaylistUsuariosContext;
        public PlaylistController(PlaylistDbContext context, UsuarioDbContext usuarioDbContext, ConteudoDbContext conteudoDbContext, TabelaRelacionamentoPlaylistECriadorPlaylistDbContext relacionamentoPLaylistCriadorPlaylistDbContext, TabelaRelacionamentoPlaylistConteudoDbContext relacionamentoPlaylistConteudoContext, TabelaRelacionamentoPlaylistUsuariosDbContext tabelaRelacionamentoPlaylistUsuariosDbContext)
        {
            _context = context;
            _usuarioDbContext = usuarioDbContext;
            _conteudoDbContext = conteudoDbContext;
            _relacionamentoPlaylistCriadorPlaylistDbContext = relacionamentoPLaylistCriadorPlaylistDbContext;
            _relacionamentoPlaylistConteudoContext = relacionamentoPlaylistConteudoContext;
            _relacionamentoPlaylistUsuariosContext = tabelaRelacionamentoPlaylistUsuariosDbContext;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var playlists = _context.Playlists.Where(playlist => !playlist.IsDeleted).ToList();

            List<PlaylistResponse> response = new List<PlaylistResponse>();

            playlists.ForEach(playlist => response.Add(this.MontarPlaylistResponseComUsuarioConteudos(playlist)));

            return Ok(response); 
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(Guid id)
        {
            var playlistResponse = _context.Playlists.SingleOrDefault(playlist => playlist.Id == id && !playlist.IsDeleted);

            if (playlistResponse == null)
            {
                return NotFound();
            }

            return Ok(this.MontarPlaylistResponseComUsuarioConteudos(playlistResponse));
        }

        [HttpPost("create/{idUsuario}")]
        public IActionResult Post(Guid idUsuario, PlaylistRequest playlist)
        {
            if (_usuarioDbContext.Usuarios.SingleOrDefault(usuario => usuario.Id == idUsuario && !usuario.IsDeleted) == null)
            {
                return NotFound();
            }

            var validarExistenciaRelacionamento = _relacionamentoPlaylistCriadorPlaylistDbContext.Relacionamentos.SingleOrDefault(relacionamento => relacionamento.IdPlaylist.Equals(playlist.Id) && relacionamento.IdCriadorPlaylist.Equals(idUsuario));
                
            if (validarExistenciaRelacionamento != null)
            {
                return BadRequest("Playlist já criada para esse usuário");
            }


            var relacionamentoPlaylistCriadorPlaylist = new TabelaRelacionamentoPlaylistECriadorPlaylist();
            relacionamentoPlaylistCriadorPlaylist.IdPlaylist = playlist.Id;
            relacionamentoPlaylistCriadorPlaylist.IdCriadorPlaylist = idUsuario;
           
            var playlistEntity = new Playlist();
            playlistEntity.Id = playlist.Id;
            playlistEntity.Nome = playlist.Nome;
            playlistEntity.IsDeleted = false;

            _relacionamentoPlaylistCriadorPlaylistDbContext.Relacionamentos.Add(relacionamentoPlaylistCriadorPlaylist);
            _context.Playlists.Add(playlistEntity);

            return CreatedAtAction(nameof(GetById), new {id = playlist.Id}, playlist);
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(Guid id, PlaylistRequest playlistRequest)
        {
            var playlistUpdate = _context.Playlists.SingleOrDefault(playlist => playlist.Id == id && !playlist.IsDeleted);

            if (playlistUpdate == null)
            {
                return NotFound();
            }

            playlistUpdate.Update(playlistRequest.Nome);

            return NoContent();
        }

        [HttpDelete("delete-playlist/{id}")]
        public IActionResult Delete(Guid id)
        {
            var playlistDeleted = _context.Playlists.SingleOrDefault(playlist => playlist.Id == id && !playlist.IsDeleted);

            if (playlistDeleted == null)
            {
                return NotFound();
            }

            playlistDeleted.Delete();

            return NoContent();
        }

        [HttpGet("add-conteudo-into-playlist/{idPlaylist}/conteudo/{idConteudo}")] 
        public IActionResult AdicionarConteudo(Guid idConteudo, Guid idPlaylist)
        {
            var conteudoAdicionado = _conteudoDbContext.Conteudos.SingleOrDefault(conteudo => conteudo.Id == idConteudo && !conteudo.IsDeleted);

            var playlistAdicionada = _context.Playlists.SingleOrDefault(playlist => playlist.Id == idPlaylist && !playlist.IsDeleted);

            if (playlistAdicionada == null)
            {
                return NotFound("Playlist não encontrada");
            }

            if (conteudoAdicionado == null)
            {
                return NotFound("Conteúdo não encontrado");
            }

            if (_relacionamentoPlaylistConteudoContext.Relacionamentos.SingleOrDefault(relacionamento => relacionamento.IdPlaylist.Equals(idPlaylist) && relacionamento.IdConteudo.Equals(conteudoAdicionado.Id)) != null)
            {
                return BadRequest("Conteúdo já existe na playlist");
            }

            var relacionamentoPlaylistConteudo = new TabelaRelacionamentoPlaylistConteudo();
            relacionamentoPlaylistConteudo.IdPlaylist = idPlaylist;
            relacionamentoPlaylistConteudo.IdConteudo = conteudoAdicionado.Id;
            _relacionamentoPlaylistConteudoContext.Relacionamentos.Add(relacionamentoPlaylistConteudo);

            return NoContent();
        }

        [HttpDelete("remove-conteudo-from-playlist/{idPlaylist}/conteudo/{idConteudo}")]
        public IActionResult ExcluirConteudo(Guid idConteudo, Guid idPlaylist)
        {
            var playlistAtual = _context.Playlists.SingleOrDefault(playlist => playlist.Id == idPlaylist && !playlist.IsDeleted);

            if ( playlistAtual == null)
            {
                return NotFound("Playlist não encontrada");
            }
            var relacionamentoPlaylistConteudo = _relacionamentoPlaylistConteudoContext.Relacionamentos.SingleOrDefault(relacionamento => relacionamento.IdPlaylist.Equals(idPlaylist) && relacionamento.IdConteudo.Equals(idConteudo));

            if (relacionamentoPlaylistConteudo == null)
            {
                return NotFound("Essa playlist não contém esse conteúdo");
            }

            _relacionamentoPlaylistConteudoContext.Relacionamentos.RemoveAt(_relacionamentoPlaylistConteudoContext.Relacionamentos.IndexOf(relacionamentoPlaylistConteudo));
                        
            return NoContent();
        }

        private PlaylistResponse MontarPlaylistResponseComUsuarioConteudos(Playlist playlist)
        {
            var response = new PlaylistResponse();
            response.Id = playlist.Id;
            response.Nome = playlist.Nome;

            var relacionamentosConteudoPlaylist = _relacionamentoPlaylistConteudoContext.Relacionamentos.Where(relacionamento => relacionamento.IdPlaylist.Equals(playlist.Id)).ToList();
            if (relacionamentosConteudoPlaylist.Count() > 0)
            {
                List<Guid> conteudosId = new List<Guid>();
                relacionamentosConteudoPlaylist.ForEach(r => conteudosId.Add(r.IdConteudo));
                response.Conteudos = _conteudoDbContext.Conteudos.Where(conteudo => conteudosId.Contains(conteudo.Id) && !conteudo.IsDeleted).ToList();
            }
            else
            {
                response.Conteudos = new List<Conteudo>();
            }

            var relacionamentoCriadorPlaylist = _relacionamentoPlaylistCriadorPlaylistDbContext.Relacionamentos.SingleOrDefault(r => r.IdPlaylist.Equals(playlist.Id));

            if (relacionamentoCriadorPlaylist != null)
            {
                var criador = _usuarioDbContext.Usuarios.SingleOrDefault(user => user.Id.Equals(relacionamentoCriadorPlaylist.IdCriadorPlaylist) && !user.IsDeleted);

                if (criador != null) 
                {
                    response.Criador = criador;
                }
            }

            var relacionamentoPlaylistUsuarios = _relacionamentoPlaylistUsuariosContext.Relacionamentos.Where(relacionamento => relacionamento.IdPlaylist == playlist.Id).ToList();
            if (relacionamentoPlaylistUsuarios.Count() > 0)
            {
                List<Guid> usuariosId = new List<Guid>();
                relacionamentoPlaylistUsuarios.ForEach(r => usuariosId.Add(r.IdUsuario));
                response.Usuarios = _usuarioDbContext.Usuarios.Where(user => usuariosId.Contains(user.Id) && !user.IsDeleted).ToList();
            } else
            {
                    response.Usuarios = new List<Usuario>();
            }

            return response;
        }
    }
}
