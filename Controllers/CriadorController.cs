using Microsoft.AspNetCore.Mvc;
using PIM_VIII.API.Entities;
using PIM_VIII.API.Models.Requests;
using PIM_VIII.API.Models.Responses;
using PIM_VIII.API.Persistence;

namespace PIM_VIII.API.Controllers
{
    [Route("api/criador")]
    [ApiController]
    public class CriadorController : ControllerBase
    {
        private readonly CriadorDbContext _context;
        private readonly TabelaRelacionamentoCriadorConteudoDbContext _contextRelacionamento;
        private readonly ConteudoDbContext _contextConteudo;

        public CriadorController(CriadorDbContext context, TabelaRelacionamentoCriadorConteudoDbContext contextRelacionamento, ConteudoDbContext contextConteudo)
        {
            _context = context;
            _contextRelacionamento = contextRelacionamento;
            _contextConteudo = contextConteudo;
        }

        [HttpGet("gat-all")]
        public IActionResult GetAll()
        {
            var criadores = _context.Criadores.Where(criador => !criador.IsDeleted).ToList();

            List<CriadorResponse> response = new List<CriadorResponse>();

            criadores.ForEach(c => response.Add(this.MontarCriadorRespnseComConteudo(c)));

            return Ok(response);
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(Guid id)
        {
            var criadorResponse = _context.Criadores.SingleOrDefault(criador =>  criador.Id == id && !criador.IsDeleted);

            if (criadorResponse == null)
            {
                return NotFound();
            }

            return Ok(this.MontarCriadorRespnseComConteudo(criadorResponse));
        }

        [HttpPost("create")]
        public IActionResult Post(CriadorRequest criador)
        {
            var criadorEntity = new Criador();
            criadorEntity.Id = criador.Id;
            criadorEntity.Nome= criador.Nome;
            criadorEntity.IsDeleted = false;

            _context.Criadores.Add(criadorEntity);
            
            return CreatedAtAction(nameof(GetById), new { id = criador.Id }, criador);
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(Guid id, CriadorRequest criadorRequest)
        {
            var criadorUpdate = _context.Criadores.SingleOrDefault(criador => criador.Id == id && !criador.IsDeleted);

            if (criadorUpdate == null)
            {
                return NotFound();
            }

            criadorUpdate.Update(criadorRequest.Nome);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var criadorDeleted = _context.Criadores.SingleOrDefault(criador => criador.Id == id && !criador.IsDeleted);

            if (criadorDeleted == null)
            {
                return NotFound();
            }

            criadorDeleted.Delete();

            return NoContent();
        }

        private CriadorResponse MontarCriadorRespnseComConteudo(Criador criador)
        {
            List<Guid> conteudosId = new List<Guid>();

            _contextRelacionamento.Relacionamentos.Where(
                r => r.idCriador == criador.Id
            ).ToList().ForEach(rel => conteudosId.Add(rel.idConteudo));

            var response = new CriadorResponse();
            response.Id = criador.Id;
            response.Nome = criador.Nome;
            response.Conteudos = _contextConteudo.Conteudos.Where(c => conteudosId.Contains(c.Id) && !c.IsDeleted).ToList();

            return response;
        }
    }
}
