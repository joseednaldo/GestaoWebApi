using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoFacil.Dados.Modelos;
using GestaoFacil.Dados.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoFacil.WebApi.Controllers
{
    [Route("api/Servico")]
    public class ServicoController : ControllerBase
    {

        private readonly IServicoRepository _servicoRepository;

        public ServicoController(IServicoRepository servico)
        {
            _servicoRepository = servico;
        }

        //APP  
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodos()
        {
            var item = _servicoRepository.GetAll();
            return Ok(item);
        }

        //web     
        //ex: /api/servicos/1
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _servicoRepository.Find(id);
            if (obj == null)
                return NotFound();

            return Ok(obj);
        }


        [Route("")]
        // ex: /api/palavras (post:id.nome,ativo,pontuacao,data)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]  Servico servico)  //Servico Servico como quebrar esse vinculo direto com a model
        {

            if (servico == null)
                return BadRequest();

            //Validando dados 
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            servico.DataCadastro = DateTime.Now;
            servico.DtAtualizacao = DateTime.Now;
            servico.IsDescontinuado = false;

            _servicoRepository.Add(servico);

            return Created($"/api/servico/{servico.ServicoId}", servico);
        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Servico servico)//Servico Servico como quebrar esse vinculo direto com a model
        {

            if (servico == null)
                return BadRequest();

            //Validando dados 
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);


            var obj = _servicoRepository.Find(id);
            if (obj == null)
                return NotFound();

            servico.ServicoId = id;
            servico.DataCadastro = obj.DataCadastro;
            servico.DtAtualizacao = DateTime.Now;

            _servicoRepository.Update(servico);
            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var ret = _servicoRepository.Remove(id);
            if (ret == 0)
                return NotFound();

            return NoContent(); 
        }

    }
}