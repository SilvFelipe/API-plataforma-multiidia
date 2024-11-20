
using Microsoft.AspNetCore.Mvc;
using PIM_VIII.API.Entities;
using PIM_VIII.API.Persistence;
using PIM_VIII.API.Models.Responses;
using PIM_VIII.API.Models.Requests;

namespace PIM_VIII.API.Controllers
{
    [Route("api/conteudo")]
    [ApiController]
    public class ConteudoController : ControllerBase
    {
        private readonly ConteudoDbContext _context;
        private readonly CriadorDbContext _contextCriador;
        private readonly TabelaRelacionamentoCriadorConteudoDbContext _contextRelacionamento;
        public ConteudoController(ConteudoDbContext context, CriadorDbContext contextCriador, TabelaRelacionamentoCriadorConteudoDbContext contextRelacionamento)
        {
            _context = context;
            _contextCriador = contextCriador;
            _contextRelacionamento = contextRelacionamento;
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var conteudos = _context.Conteudos.Where(conteudo => !conteudo.IsDeleted).ToList();

            List<ConteudoResponse> response = new List<ConteudoResponse>();


            conteudos.ForEach(c => response.Add(this.MontarConteudoResponseComCriadores(c)));

            response = response.Where(resp => resp != null).ToList();

            return Ok(response);

            
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(Guid id)
        {
            var conteudoResponse = _context.Conteudos.SingleOrDefault(conteudo => conteudo.Id == id && !conteudo.IsDeleted);

            if (conteudoResponse == null)
            {
                return NotFound();
            }

            var response = this.MontarConteudoResponseComCriadores(conteudoResponse);

            if (response != null)
            {
                return Ok(response);
            }

            return NotFound();
        }

        [HttpPost("create/{idCriador}")]
        public IActionResult Post(Guid idCriador, ConteudoRequest conteudo)
        {            
            if (_contextCriador.Criadores.SingleOrDefault(criador => criador.Id == idCriador && !criador.IsDeleted) == null)
            {
                return NotFound();
            }

            var validaExistenciaRelacionamento = _contextRelacionamento.Relacionamentos.SingleOrDefault(relacionamento => relacionamento.idConteudo.Equals(conteudo.Id) && relacionamento.idCriador.Equals(idCriador));

            if (validaExistenciaRelacionamento != null)
            {
                return BadRequest("Conteúdo já criado para esse criador");
            }

            var novoRelacionamento = new TabelaRelacionamentoCriadorConteudo();
            novoRelacionamento.idCriador = idCriador;
            novoRelacionamento.idConteudo = conteudo.Id;
            _contextRelacionamento.Relacionamentos.Add(novoRelacionamento);

            var conteudoEntity = new Conteudo();
            conteudoEntity.Id = conteudo.Id;
            conteudoEntity.Titulo = conteudo.Titulo;
            conteudoEntity.Tipo = conteudo.Tipo;
            conteudoEntity.IsDeleted = false;

            _context.Conteudos.Add(conteudoEntity);

            return CreatedAtAction(nameof(GetById), new { id = conteudo.Id }, conteudo);
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(Guid id, ConteudoRequest conteudoRequest)
        {
            var conteudoUpdate = _context.Conteudos.SingleOrDefault(conteudo => conteudo.Id == id && !conteudo.IsDeleted);

            if (conteudoUpdate == null)
            {
                return NotFound();
            }

            conteudoUpdate.Update(conteudoRequest.Titulo, conteudoRequest.Tipo);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var conteudoDeleted = _context.Conteudos.SingleOrDefault(conteudo => conteudo.Id == id && !conteudo.IsDeleted);

            if (conteudoDeleted == null)
            {
                return NotFound();
            }

            conteudoDeleted.Delete();

            return NoContent();
        }

        private ConteudoResponse MontarConteudoResponseComCriadores(Conteudo conteudo)
        {
            var relacionamento = _contextRelacionamento.Relacionamentos.SingleOrDefault(r => r.idConteudo == conteudo.Id);

            if (relacionamento != null)
            {
                var criador = _contextCriador.Criadores.SingleOrDefault(c => c.Id == relacionamento.idCriador && !c.IsDeleted);

                if (criador != null)
                {
                    var response = new ConteudoResponse();
                    response.Id = conteudo.Id;
                    response.Titulo = conteudo.Titulo;
                    response.Tipo = conteudo.Tipo;
                    response.Criador = criador;

                    return response;
                }
            }            

            return null;
        }
    }
}
