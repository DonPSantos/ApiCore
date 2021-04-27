using ApiCore.Business.Models;
using Bogus;
using Bogus.Extensions.Brazil;
using DevIO.Business.Services;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using Xunit;

namespace ApiCore.Test.Fixtures
{
    [CollectionDefinition(nameof(ProdutoCollection))]
    public class ProdutoCollection : ICollectionFixture<ProdutoFixture>
    {
    }

    public sealed class ProdutoFixture : IDisposable
    {
        public ProdutoService ProdutoService;
        public AutoMocker Mocker;
        private readonly Faker _fake;

        public ProdutoFixture()
        {
            _fake = new Faker("pt_BR");
        }

        public void Dispose()
        {
            //Não necessita implementação
        }

        public ProdutoService ObterProdutoService()
        {
            Mocker = new AutoMocker();
            ProdutoService = Mocker.CreateInstance<ProdutoService>();
            return ProdutoService;
        }

        public Produto GerarProdutoValido()
        {
            var guid = Guid.NewGuid();
            var fornecedor = GerarFornecedorValido();

            return new Produto
            {
                Ativo = true,
                Id = guid,
                Nome = _fake.Commerce.ProductName(),
                DataCadastro = DateTime.Now,
                Descricao = _fake.Commerce.ProductDescription(),
                Fornecedor = fornecedor,
                FornecedorId = fornecedor.Id,
                Imagem = _fake.Image.LoremPixelUrl(),
                Valor = decimal.Parse(_fake.Commerce.Price()),
                Visualizacao = _fake.Image.LoremPixelUrl()
            };
        }

        public Produto GerarProdutoInvalido()
        {
            var guid = Guid.NewGuid();
            var fornecedor = GerarFornecedorValido();

            return new Produto
            {
                Ativo = true,
                Id = guid,
                Nome = string.Empty,
                DataCadastro = DateTime.Now,
                Descricao = string.Empty,
                Fornecedor = fornecedor,
                FornecedorId = fornecedor.Id,
                Imagem = _fake.Image.LoremPixelUrl(),
                Valor = 0,
                Visualizacao = _fake.Image.LoremPixelUrl()
            };
        }

        private Fornecedor GerarFornecedorValido()
        {
            var guid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Ativo = true,
                Documento = _fake.Company.Cnpj().Replace("/", "").Replace(".", "").Replace("-", ""),
                Id = guid,
                Nome = _fake.Company.CompanyName(),
                TipoFornecedor = TipoFornecedor.PessoaJuridica
            };
            return fornecedor;
        }
    }
}