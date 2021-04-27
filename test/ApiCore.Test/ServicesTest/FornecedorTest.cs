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
using System.Linq.Expressions;
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

        #region Adicionar

        [Fact(DisplayName = "Adicionar fornecedor com sucesso.")]
        [Trait("Fornecedor", "Adicionar")]
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

        [Fact(DisplayName = "Adicionar com documento invalido.")]
        [Trait("Fornecedor", "Adicionar")]
        public async Task Fornecedor_Adicionar_Deve_Estar_Invalido_Documento()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorInvalido();

            //Act
            var result = await _fornecedorService.Adicionar(fornecedor);

            //Asset
            Assert.False(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Never);
        }

        [Fact(DisplayName = "Adicionar CNPJ que já existe.")]
        [Trait("Fornecedor", "Adicionar")]
        public async Task Fornecedor_Adicionar_Deve_Estar_Invalido_Fornecedor_Duplicado()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValido();
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Setup(f => f.Buscar(f => f.Documento == fornecedor.Documento).Result).Returns(new List<Fornecedor> { fornecedor });
            //Act
            var result = await _fornecedorService.Adicionar(fornecedor);

            //Asset
            Assert.False(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Never);
        }

        #endregion Adicionar

        #region Atualizar

        [Fact(DisplayName = "Atualizar com sucesso.")]
        [Trait("Fornecedor", "Atualizar")]
        public async Task Fornecedor_Atualizar_Deve_Estar_Valido()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValido();

            //Act
            var result = await _fornecedorService.Atualizar(fornecedor);

            //Asset
            Assert.True(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Once);
        }

        [Fact(DisplayName = "Atualizar com documento invalido.")]
        [Trait("Fornecedor", "Atualizar")]
        public async Task Fornecedor_Atualizar_Deve_Estar_Invalido_Documento()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorInvalido();

            //Act
            var result = await _fornecedorService.Atualizar(fornecedor);

            //Asset
            Assert.False(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Atualizar(fornecedor), Times.Never);
        }

        [Fact(DisplayName = "Atualizar com CNPJ que já existe em outro fornecedor.")]
        [Trait("Fornecedor", "Atualizar")]
        public async Task Fornecedor_Atualizar_Deve_Estar_Invalido_Fornecedor_Duplicado()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValido();
            var fornecedor2 = _fornecedorFixture.GerarFornecedorValido();
            fornecedor2.Documento = fornecedor.Documento;
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Setup(f => f.Buscar(It.IsAny<Expression<Func<Fornecedor, bool>>>()).Result)
                                                                        .Returns(new List<Fornecedor> { fornecedor2 });

            //Act
            var result = await _fornecedorService.Atualizar(fornecedor);

            //Asset
            Assert.False(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Adicionar(fornecedor), Times.Never);
        }

        #endregion Atualizar

        #region Remover

        [Fact(DisplayName = "Remover com sucesso.")]
        [Trait("Fornecedor", "Remover")]
        public async Task Fornecedor_Remover_Deve_Estar_Valido()
        {
            //Arrange
            var fornecedor = _fornecedorFixture.GerarFornecedorValido();

            _fornecedorFixture.Mocker
                .GetMock<IFornecedorRepository>()
                .Setup(f => f.ObterFornecedorProdutosEndereco(It.IsAny<Guid>()))
                .ReturnsAsync(fornecedor);

            _fornecedorFixture.Mocker
                .GetMock<IEnderecoRepository>()
                .Setup(f => f.ObterEnderecoPorFornecedor(It.IsAny<Guid>()))
                .ReturnsAsync(fornecedor.Endereco);

            //Act
            var result = await _fornecedorService.Remover(fornecedor.Id);

            //Asset
            Assert.True(result);
            _fornecedorFixture.Mocker.GetMock<IFornecedorRepository>().Verify(r => r.Remover(fornecedor.Id), Times.Once);
        }

        [Fact(DisplayName = "Erro ao remover por conter produtos.")]
        [Trait("Fornecedor", "Remover")]
        public async Task Fornecedor_Remover_Invalido_Contem_Produtos()
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

        #endregion Remover
    }
}