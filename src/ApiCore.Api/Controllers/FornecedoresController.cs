﻿using ApiCore.Api.ViewModel;
using ApiCore.Business.Intefaces;
using ApiCore.Business.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository, IFornecedorService fornecedorService, IEnderecoRepository enderecoRepository, IMapper mapper, INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodosAsync()
        {
            var fornecedores = await _fornecedorRepository.ObterTodos();

            return _mapper.Map<IEnumerable<FornecedorViewModel>>(fornecedores);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorIdAsync(Guid id)
        {
            var fornecedor = await _fornecedorRepository.ObterFornecedorProdutosEndereco(id);

            if (fornecedor == null) return NotFound();

            return _mapper.Map<FornecedorViewModel>(fornecedor);
        }

        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> AdicionarAsync(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));
            return CustomResponse(fornecedorViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> AtualizarAsync(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo passado na query.");
                return CustomResponse(fornecedorViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(fornecedorViewModel);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ExcluirAsync(Guid id)
        {
            var fornecedorViewModel = _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));

            if (fornecedorViewModel == null) return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse(fornecedorViewModel);
        }

        [HttpGet("endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
        {
            return _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
        }

        [HttpPut("endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return CustomResponse(enderecoViewModel);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));

            return CustomResponse(enderecoViewModel);
        }
    }
}