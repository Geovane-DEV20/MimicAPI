using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;
        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }

        //api/palavras?data=2019-05-05
        //api/palavras?pagNumero=3&pagRegistroPag=2
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery] PalavraUrlQuery query)
        {


            var paginacao = new Paginacao();

            if (query.PagNumero > paginacao.TotalPaginas)
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

            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);

            if (obj == null) // Caso o objeto não tenha no banco de dados
            {
                return NotFound();
            }
            return Ok();
        }

        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {

            return Created($"api/palavras/{palavra.Id}", palavra);

        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            palavra.Id = id;


            return Ok();
        }



        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id); //O método Find do EF Core encontra um registro com os valores de chave primária fornecidos .

            if (palavra == null) // Caso o objeto não tenha no banco de dados
            {
                return NotFound();
            }

            if (palavra.Ativo == true)
            {
                palavra.Ativo = false;
            }

            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            return NoContent();
        }

    }
}
