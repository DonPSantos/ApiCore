using ApiCore.Business.Intefaces;
using ApiCore.Test.Fixtures;
using DevIO.Business.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiCore.Test.ServicesTest
{
    [Collection(nameof(ProdutoCollection))]
    public class ProdutoTest
    {
        private readonly ProdutoFixture _produtoFixture;
        private readonly ProdutoService _produtoService;

        public ProdutoTest(ProdutoFixture produtoFixture)
        {
            _produtoFixture = produtoFixture;
            _produtoService = produtoFixture.ObterProdutoService();
        }

        #region Adicionar

        [Fact(DisplayName = "Adicionar produto com sucesso.")]
        [Trait("Produto", "Adicionar")]
        public async Task Produto_Adicionar_Deve_Estar_Valido()
        {
            //Arrange
            var produto = _produtoFixture.GerarProdutoValido();

            //Act
            await _produtoService.Adicionar(produto);

            //Asset
            _produtoFixture.Mocker.GetMock<IProdutoRepository>().Verify(r => r.Adicionar(produto), Times.Once);
        }

        [Fact(DisplayName = "Adicionar produto com erro.")]
        [Trait("Produto", "Adicionar")]
        public async Task Produto_Adicionar_Deve_Estar_Invalido()
        {
            //Arrange
            var produto = _produtoFixture.GerarProdutoInvalido();

            //Act
            await _produtoService.Adicionar(produto);

            //Asset
            _produtoFixture.Mocker.GetMock<IProdutoRepository>().Verify(r => r.Adicionar(produto), Times.Never);
        }

        #endregion Adicionar

        #region Atualizar

        [Fact(DisplayName = "Atualizar produto com sucesso.")]
        [Trait("Produto", "Atualizar")]
        public async Task Produto_Atualizar_Deve_Estar_Valido()
        {
            //Arrange
            var produto = _produtoFixture.GerarProdutoValido();

            //Act
            await _produtoService.Atualizar(produto);

            //Asset
            _produtoFixture.Mocker.GetMock<IProdutoRepository>().Verify(r => r.Atualizar(produto), Times.Once);
        }

        [Fact(DisplayName = "Atualizar produto com erro.")]
        [Trait("Produto", "Atualizar")]
        public async Task Produto_Atualizar_Deve_Estar_Invalido()
        {
            //Arrange
            var produto = _produtoFixture.GerarProdutoInvalido();

            //Act
            await _produtoService.Atualizar(produto);

            //Asset
            _produtoFixture.Mocker.GetMock<IProdutoRepository>().Verify(r => r.Atualizar(produto), Times.Never);
        }

        #endregion Atualizar
    }
}