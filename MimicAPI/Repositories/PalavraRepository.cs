using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _banco;   
        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }
        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public Palavra Obter(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public PaginationList<Palavra> ObterPalavras(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();  
            var item = _banco.Palavras.AsNoTracking().AsQueryable();
            if (query.Data.HasValue)
            {
                item = item.Where(a => a.Criado > query.Data.Value || a.Atualizado > query.Data.Value);
            }

            if (query.PagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value); //Skip = Pular, Take = pegar

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.PagNumero.Value;
                paginacao.RegistrosPorPagina = query.PagRegistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)quantidadeTotalRegistros / query.PagRegistro.Value); /* 30/10=3pag 21/10=2,1 = 3 */
                
                lista.Paginacao = paginacao;

            }
            lista.AddRange(item.ToList());
            return lista;
        }
    }
}
