using Microsoft.AspNetCore.Mvc;
using GestaoFacil.Dados.Repositories.Interfaces;
using GestaoFacil.Dados.Modelos;
using AutoMapper;
using System;

namespace GestaoFacil.WebApi.Controllers
{


    [ApiController]
    //[Route("api/Funcionario")]
    [Route("api/v{verion:apiVersion}/[controller]")] // é o mesmo que => [Route("api/palavras")]  //Criando rotas por atributos
    [ApiVersion("1.0")]
    //[ApiVersion("1.1")]//fala que o controlador tem as 2 versões.


    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioRepository _funcioanrioRepository;
        private readonly IMapper _mapper;

        public FuncionarioController(IFuncionarioRepository funcioanrioRepository, IMapper mapper)
        {
            _funcioanrioRepository = funcioanrioRepository;
            _mapper = mapper;
        }


        /// <summary>
        /// Operação que pega todos os funcionarios do banco que existem
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Listagem de funcionarios</returns>
        [MapToApiVersion("1.0")]
        //[MapToApiVersion("1.1")]
        //APP  
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas()
        {
            var item = _funcioanrioRepository.GetAll();
            return Ok(item);
        }

        #region Obter funcionario
        /// <summary>
        /// Operação que pega um único funcionario do banco
        /// </summary>
        /// <param name="query">Identificador do funcionario</param>
        /// <returns>Objeto funcioanrio</returns>
        [MapToApiVersion("1.0")]
        //[MapToApiVersion("1.1")]
        //web     
        //ex: /api/palavras/1
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _funcioanrioRepository.Find(id);
            if (obj == null)
                return NotFound();

            return Ok(obj);
        }
        #endregion


        /// <summary>
        /// Operação que cadastrar um funcionario
        /// </summary>
        /// <param name="palavra">Um objeto palavra</param>
        /// <returns>>Um objeto funcionario com seu identificador</returns>
        [MapToApiVersion("1.0")]
        [Route("")]
        // ex: /api/palavras (post:id.nome,ativo,pontuacao,data)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]  Funcionario funcionario)  //Funcionario funcionario como quebrar esse vinculo direto com a model
        {

            if (funcionario == null)
                return BadRequest();

            //Validando dados 
            if(!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            funcionario.DataCadastro = DateTime.Now;
            funcionario.Ativo = true;

            _funcioanrioRepository.Add(funcionario);
            return Created($"/api/funcionario/{funcionario.FuncionarioId}", funcionario);

        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Funcionario funcionario)
        {

            if (funcionario == null)
                return BadRequest();

            //Validando dados 
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var obj = _funcioanrioRepository.Find(id);
            if (obj == null)
                return NotFound();

            funcionario.FuncionarioId = id;
            funcionario.DataCadastro = obj.DataCadastro;

            _funcioanrioRepository.Update(funcionario);
            return Ok();
        }


        [Route("{id}")]
        [HttpDelete]
        public ActionResult Deletar(int id)
        {

            var _funcionario = _funcioanrioRepository.Find(id);

            if (_funcionario == null)
                return NotFound();

            _funcioanrioRepository.Remove(id);
             return NoContent(); //Ok();
        }
    }
}