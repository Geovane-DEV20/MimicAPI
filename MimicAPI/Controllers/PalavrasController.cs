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
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {
            var item = _banco.Palavras.AsQueryable();
            if (data.HasValue)
            {
                item = item.Where(a => a.Criado > query.Data.Value || a.Atualizado > query.Data.Value);
            }

            if (pagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value); //Skip = Pular, Take = pegar

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.RegistrosPorPagina = query.PagRegistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPaginas = (int) Math.Ceiling((double)quantidadeTotalRegistros / query.PagRegistro.Value) ; /* 30/10=3pag 21/10=2,1 = 3 */

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if(query.PagNumero > paginacao.TotalPaginas)
                {
                    return NotFound();
                }
            }

            return Ok(item);
        }



        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _banco.Palavras.Find(id);

            if(obj == null) // Caso o objeto não tenha no banco de dados
            {
                return NotFound();
            }
            return Ok();
        }

        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
            return Created($"api/palavras/{palavra.Id}", palavra);

        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a=>a.Id == id);

            if (obj == null) 
            {
                return NotFound();
            }

            palavra.Id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

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
