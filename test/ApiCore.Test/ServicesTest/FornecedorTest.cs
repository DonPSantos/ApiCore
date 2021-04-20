using ApiCore.Business.Intefaces;
using ApiCore.Business.Models;
using ApiCore.Business.Notifications;
using ApiCore.Test.V1.Fixtures;
using DevIO.Business.Services;
using DevIO.Data.Repository;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiCore.Test.ServicesTest
{
    [Collection(nameof(FornecedorCollection))]
    public class FornecedorTest
    {
        private readonly FornecedorFixture _fornecedorFixture;
        private readonly FornecedorService _fornecedorService;

        public FornecedorTest(FornecedorFixture fornecedorFixture)
        {
            _fornecedorFixture = fornecedorFixture;
            _fornecedorService = fornecedorFixture.ObterFornecedorService();
        }

        [Fact]
        public async Task Fornecedor_Adicionar_DeveEstarValido()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValido();

            //Act
            var result = await _fornecedorService.Adicionar(fornecedor);

            //Asset
            Assert.True(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Once);
        }

        [Fact]
        public async Task Fornecedor_Adicionar_DeveEstarInvalido()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorInvalido();

            //Act
            var result = await _fornecedorService.Adicionar(fornecedor);

            //Asset
            Assert.False(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Never);
        }

        [Fact]
        public async Task Fornecedor_Atualizar_DeveEstarValido()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValido();

            //Act
            var result = await _fornecedorService.Atualizar(fornecedor);

            //Asset
            Assert.True(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Once);
        }

        [Fact]
        public async Task Fornecedor_Atualizar_DeveEstarInvalido()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorInvalido();

            //Act
            var result = await _fornecedorService.Atualizar(fornecedor);

            //Asset
            Assert.False(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Never);
        }

        [Fact]
        public async Task Fornecedor_Remover_DeveEstarValido()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValido();
            _fornecedorFixture.Mocker
                .GetMock<IFornecedorRepository>()
                .Setup(f => f.ObterFornecedorProdutosEndereco(It.IsAny<Guid>()))
                .ReturnsAsync(fornecedor);

            //Act
            var result = await _fornecedorService.Remover(fornecedor.Id);

            //Asset
            Assert.True(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Remover(fornecedor.Id), Times.Once);
        }

        [Fact]
        public async Task Fornecedor_Remover_InvalidoPorTerProdutos()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValidoComProduto();
            _fornecedorFixture.Mocker
                .GetMock<IFornecedorRepository>()
                .Setup(f => f.ObterFornecedorProdutosEndereco(It.IsAny<Guid>()))
                .ReturnsAsync(fornecedor);

            //Act
            var result = await _fornecedorService.Remover(fornecedor.Id);

            //Asset
            Assert.False(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Remover(fornecedor.Id), Times.Never);
        }
    }
}