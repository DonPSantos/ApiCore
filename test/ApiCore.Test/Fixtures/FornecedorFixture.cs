using ApiCore.Business.Models;
using Bogus;
using Bogus.Extensions.Brazil;
using CountryData.Bogus;
using DevIO.Business.Services;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using Xunit;

namespace ApiCore.Test.V1.Fixtures
{
    [CollectionDefinition(nameof(FornecedorCollection))]
    public class FornecedorCollection : ICollectionFixture<FornecedorFixture>
    {
    }

    public class FornecedorFixture : IDisposable
    {
        public FornecedorService FornecedorService;
        public AutoMocker Mocker;
        private Faker _fake;

        public FornecedorFixture()
        {
            _fake = new Faker("pt_BR");
        }

        public void Dispose()
        {
        }

        public Fornecedor GerarFornecedorInvalido()
        {
            var guid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Ativo = true,
                Documento = _fake.Person.Cpf().Replace("-", "").Replace(".", ""),
                Endereco = GerarEnderecoValido(guid),
                Id = guid,
                Nome = _fake.Company.CompanyName(),
                TipoFornecedor = TipoFornecedor.PessoaJuridica
            };

            return fornecedor;
        }

        public Fornecedor GerarFornecedorValido()
        {
            var guid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Ativo = true,
                Documento = _fake.Company.Cnpj().Replace("/", "").Replace(".", "").Replace("-", ""),
                Endereco = GerarEnderecoValido(guid),
                Id = guid,
                Nome = _fake.Company.CompanyName(),
                TipoFornecedor = TipoFornecedor.PessoaJuridica
            };
            return fornecedor;
        }

        public FornecedorService ObterFornecedorService()
        {
            Mocker = new AutoMocker();
            FornecedorService = Mocker.CreateInstance<FornecedorService>();
            return FornecedorService;
        }

        public Fornecedor GerarFornecedorValidoComProduto()
        {
            var guid = Guid.NewGuid();
            var fornecedor = new Fornecedor
            {
                Ativo = true,
                Documento = _fake.Company.Cnpj().Replace("/", "").Replace(".", "").Replace("-", ""),
                Endereco = GerarEnderecoValido(guid),
                Id = guid,
                Nome = _fake.Company.CompanyName(),
                TipoFornecedor = TipoFornecedor.PessoaJuridica,
                Produtos = new List<Produto> { GerarProdutoValido(guid) }
            };
            return fornecedor;
        }

        private Endereco GerarEnderecoValido(Guid fornecedorid)
        {
            return new Endereco
            {
                Bairro = "Teste",
                Cep = _fake.Address.ZipCode().Replace("-", ""),
                Cidade = _fake.Country().Brazil().Place().Name,
                Complemento = "Prédio",
                Estado = _fake.Country().Brazil().State().Name,
                FornecedorId = fornecedorid,
                Id = Guid.NewGuid(),
                Logradouro = _fake.Address.StreetName(),
                Numero = _fake.Address.StreetAddress().ToString()
            };
        }

        private Produto GerarProdutoValido(Guid fornecedorid)
        {
            return new Produto
            {
                Ativo = true,
                DataCadastro = DateTime.Now,
                Descricao = _fake.Commerce.ProductDescription(),
                FornecedorId = fornecedorid,
                Id = Guid.NewGuid(),
                Imagem = _fake.Image.LoremFlickrUrl(),
                Nome = _fake.Commerce.Product(),
                Valor = decimal.Parse(_fake.Commerce.Price()),
                Visualizacao = _fake.Image.LoremFlickrUrl()
            };
        }
    }
}