using ApiCore.Api.Configurations.Authorization;
using ApiCore.Api.Controllers;
using ApiCore.Api.ViewModel;
using ApiCore.Business.Intefaces;
using ApiCore.Business.Models;
using AutoMapper;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ApiCore.Api.V1.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutosController : MainController
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoRepository produtoRepository,
                                    IProdutoService produtoService,
                                    IConfiguration configuration,
                                    IMapper mapper,
                                    INotificador notificador,
                                    IUser user) : base(notificador, user)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAsync([FromForm] ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            produtoViewModel.Imagem = Guid.NewGuid() + "-" + produtoViewModel.ImagemUpload.FileName;

            produtoViewModel.Visualizacao = await UploadArquivo(produtoViewModel.ImagemUpload, produtoViewModel.Imagem);

            produtoViewModel.DataCadastro = DateTime.Now;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [ClaimsAuthorize("Produto", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> AtualizarAsync(Guid id, [FromForm] ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
            {
                NotificarErro("Os ids informados não são iguais!");
                return CustomResponse();
            }

            var produtoAtualizacao = await ObterProduto(id);

            if (string.IsNullOrEmpty(produtoViewModel.Imagem))
                produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (produtoViewModel.ImagemUpload is not null)
            {
                produtoViewModel.Imagem = Guid.NewGuid() + "-" + produtoViewModel.ImagemUpload.FileName;

                produtoAtualizacao.Visualizacao = await UploadArquivo(produtoViewModel.ImagemUpload, produtoViewModel.Imagem);

                produtoAtualizacao.Imagem = produtoViewModel.Imagem;
            }

            produtoAtualizacao.FornecedorId = produtoViewModel.FornecedorId;
            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            return CustomResponse(produtoViewModel);
        }

        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [ClaimsAuthorize("Produto", "Remover")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ExcluirAsync(Guid id)
        {
            var produtoViewModel = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));

            if (produtoViewModel == null) return NotFound();

            if (string.IsNullOrEmpty(produtoViewModel.Imagem))
                await ApagarImagem(produtoViewModel.Imagem);

            await _produtoService.Remover(id);

            return CustomResponse(produtoViewModel);
        }

        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorIdAsync(Guid id)
        {
            var produtoViewModel = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));

            if (produtoViewModel == null) return NotFound();

            return produtoViewModel;
        }

        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodosAsync()
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
        }

        private async Task<string> UploadArquivo(IFormFile arquivo, string nomeArquivo)
        {
            var accessToken = _configuration.GetSection("DropBoxAccessToken").Value;

            if (arquivo is null || arquivo.Length <= 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return string.Empty;
            }

            using (var _dropBox = new DropboxClient(accessToken))
            using (var _memoryStream = new MemoryStream())
            {
                await arquivo.CopyToAsync(_memoryStream);
                _memoryStream.Position = 0;
                var updated = await _dropBox.Files.UploadAsync("/" + nomeArquivo, WriteMode.Overwrite.Instance, body: _memoryStream);
                var result = await _dropBox.Sharing.CreateSharedLinkWithSettingsAsync("/" + nomeArquivo);
                return result.Url + "&raw=1";
            }
        }

        private async Task ApagarImagem(string nomeImagem)
        {
            var accessToken = _configuration.GetSection("DropBoxAccessToken").Value;

            using (var _dropBox = new DropboxClient(accessToken))
                await _dropBox.Files.DeleteV2Async("/" + nomeImagem);
        }
    }
}