using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        public PalavrasController(IPalavraRepository repository)
        {
            _repository = repository;
        }

        //api/palavras?data=2019-05-05
        //api/palavras?pagNumero=3&pagRegistroPag=2
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery] PalavraUrlQuery query)
        {
            var item = _repository.Obter(query);

            var paginacao = new Paginacao();

            if (query.PagNumero > item.paginacao.TotalPaginas)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

            return Ok(item.ToList());
        }



        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {

            var obj = _repository.Obter(id);

            if (obj == null) // Caso o objeto não tenha no banco de dados
            {
                return NotFound();
            }
            return Ok(obj);
        }

        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"api/palavras/{palavra.Id}", palavra);

        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _repository.Obter(id);
            if (obj == null)
            {
                return NotFound();
            }

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _repository.Obter(id); //O método Find do EF Core encontra um registro com os valores de chave primária fornecidos .
            if (palavra == null) // Caso o objeto não tenha no banco de dados
            {
                return NotFound();
            }
            _repository.Deletar(id);

            return NoContent();
        }

    }
}
