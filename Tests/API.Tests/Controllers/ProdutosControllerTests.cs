﻿using Moq;
using MediatR;
using API.Controllers;
using Application.Catalogo.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Application.Catalogo.Queries;
using Application.Catalogo.Commands;
using Application.Catalogo.Boundaries;
using Domain.Base.Communication.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Application.Catalogo.Commands.Validation;
using Domain.Base.Messages.CommonMessages.Notifications;
using Domain.Catalogo;

namespace API.Tests.Controllers
{


    public class ProdutosControllerTests : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IServiceProvider _serviceProvideStartTup;
        private readonly Mock<IMediatorHandler> _mediatorHandlerMock;
        private readonly Mock<IProdutosQueries> _produtosQueriesMock;
        private readonly ProdutosController _produtosController;

        public ProdutosControllerTests()
        {
            _serviceProvider = new ServiceCollection()
               .AddScoped<IMediatorHandler, MediatorHandler>()
               .AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>()
               .BuildServiceProvider();

            _serviceProvideStartTup = new TestStartup().ConfigureServices(new ServiceCollection());
            _mediatorHandlerMock = new Mock<IMediatorHandler>();
            _produtosQueriesMock = new Mock<IProdutosQueries>();
            _produtosController = new ProdutosController(
                _produtosQueriesMock.Object,
                _serviceProvider.GetRequiredService<INotificationHandler<DomainNotification>>(),
                _mediatorHandlerMock.Object);
        }

        #region Testes unitários do metodo Get
        [Fact]
        public async Task AoChamarGet_DeveRetornarListaDeProdutos_QuandoProdutosDisponiveis()
        {
            // Arrange
            var listaProdutos = new List<ProdutoDto>
            {
                new ProdutoDto { Id = new Guid(), Nome = "Produto 1" },
                new ProdutoDto { Id = new Guid(), Nome = "Produto 2" }
            };

            _produtosQueriesMock.Setup(x => x.ObterTodos()).ReturnsAsync(listaProdutos);

            // Act
            var resultado = await _produtosController.Get();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(resultado);
            var produtosRetornados = Assert.IsType<List<ProdutoDto>>(okObjectResult.Value);
            Assert.Equal(listaProdutos, produtosRetornados);
        }

        [Fact]
        public async Task AoChamarGet_DeveRetornarNotFound_QuandoNaoHaProdutos()
        {

            // Configurar o mock para retornar uma lista vazia
            _produtosQueriesMock.Setup(x => x.ObterTodos()).ReturnsAsync((List<ProdutoDto>)null);

            // Act
            var resultado = await _produtosController.Get();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal("Nenhum produto encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task AoChamarGet_DeveRetornarStatusCode500_QuandoOcorreExcecao()
        {

            // Configurar o mock para lançar uma exceção
            _produtosQueriesMock.Setup(x => x.ObterTodos()).ThrowsAsync(new Exception("Simulando uma exceção"));

            // Act
            var resultado = await _produtosController.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar recuperar produtos. Erro: Simulando uma exceção", objectResult.Value);
        }
        #endregion

        #region Testes unitários do metodo GetGategories
        [Fact]
        public async Task AoChamarGetGategories_DeveRetornarListaDeCategorias_QuandoCategoriasDisponiveis()
        {
            var listaCategorias = new List<CategoriaDto>
            {
                new CategoriaDto { Id = new Guid(), Nome = "Categoria 1" },
                new CategoriaDto { Id = new Guid(), Nome = "Categoria 2" }
            };

            _produtosQueriesMock.Setup(x => x.ObterCategorias()).ReturnsAsync(listaCategorias);

            // Act
            var resultado = await _produtosController.GetGategories();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(resultado);
            var categoriasRetornadas = Assert.IsType<List<CategoriaDto>>(okObjectResult.Value);
            Assert.Equal(listaCategorias, categoriasRetornadas);
        }

        [Fact]
        public async Task AoChamarGetGategories_DeveRetornarNotFound_QuandoNaoHaCategorias()
        {

            // Configurar o mock para retornar uma lista vazia
            _produtosQueriesMock.Setup(x => x.ObterCategorias()).ReturnsAsync((List<CategoriaDto>)null);

            // Act
            var resultado = await _produtosController.GetGategories();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal("Nenhuma categoria encontrada.", notFoundResult.Value);
        }

        [Fact]
        public async Task AoChamarGetGategories_DeveRetornarStatusCode500_QuandoOcorreExcecao()
        {

            // Configurar o mock para lançar uma exceção
            _produtosQueriesMock.Setup(x => x.ObterCategorias()).ThrowsAsync(new Exception("Simulando uma exceção"));

            // Act
            var resultado = await _produtosController.GetGategories();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar recuperar categorias. Erro: Simulando uma exceção", objectResult.Value);
        }
        #endregion

        #region Testes unitários do metodo Get por Id
        [Fact]
        public async Task AoChamarGetById_DeveRetornarProduto_QuandoProdutoDisponivel()
        {
            var produto = new ProdutoDto { Id = new Guid(), Nome = "Produto 1" };

            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(produto);

            // Act
            var resultado = await _produtosController.Get(It.IsAny<Guid>());

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(resultado);
            var produtoRetornado = Assert.IsType<ProdutoDto>(okObjectResult.Value);
            Assert.Equal(produto, produtoRetornado);
        }

        [Fact]
        public async Task AoChamarGetById_DeveRetornarNotFound_QuandoNaoHaProduto()
        {

            // Configurar o mock para retornar uma lista vazia
            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).ReturnsAsync((ProdutoDto)null);

            // Act
            var resultado = await _produtosController.Get(It.IsAny<Guid>());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal("Produto não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task AoChamarGetById_DeveRetornarStatusCode500_QuandoOcorreExcecao()
        {

            // Configurar o mock para lançar uma exceção
            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>())).ThrowsAsync(new Exception("Simulando uma exceção"));

            // Act
            var resultado = await _produtosController.Get(It.IsAny<Guid>());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar recuperar produto. Erro: Simulando uma exceção", objectResult.Value);
        }
        #endregion

        #region Testes unitários do metodo Get por Código
        [Fact]
        public async Task AoChamarGetByCodigo_DeveRetornarListaDeProdutos_QuandoProdutosDisponiveis()
        {
            var codigo = 1;
            var listaProdutos = new List<ProdutoDto>
            {
                new ProdutoDto { Id = new Guid(), Nome = "Produto 1" },
                new ProdutoDto { Id = new Guid(), Nome = "Produto 2" }
            };

            _produtosQueriesMock.Setup(x => x.ObterPorCategoria(codigo)).ReturnsAsync(listaProdutos);

            // Act
            var resultado = await _produtosController.Get(codigo);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(resultado);
            var produtosRetornados = Assert.IsType<List<ProdutoDto>>(okObjectResult.Value);
            Assert.Equal(listaProdutos, produtosRetornados);
        }

        [Fact]
        public async Task AoChamarGetByCodigo_DeveRetornarNotFound_QuandoNaoHaProdutos()
        {
            var codigo = 1;

            // Configurar o mock para retornar uma lista vazia
            _produtosQueriesMock.Setup(x => x.ObterPorCategoria(codigo)).ReturnsAsync((List<ProdutoDto>)null);

            // Act
            var resultado = await _produtosController.Get(codigo);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal("Produto não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task AoChamarGetByCodigo_DeveRetornarStatusCode500_QuandoOcorreExcecao()
        {
            var codigo = 1;

            // Configurar o mock para lançar uma exceção
            _produtosQueriesMock.Setup(x => x.ObterPorCategoria(codigo)).ThrowsAsync(new Exception("Simulando uma exceção"));

            // Act
            var resultado = await _produtosController.Get(codigo);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar recuperar produto. Erro: Simulando uma exceção", objectResult.Value);
        }
        #endregion

        #region Testes unitários do metodo Post
        [Fact]
        public async Task AoChamarPost_DeveRetornarOk_QuandoCadastroProdutoBemSucedido()
        {
            var produtoInput = new ProdutoInput
            {
                CategoriaId = Guid.NewGuid(),
                Nome = "Produto Teste",
                Ativo = true,
                Valor = 100.00m,
                Imagem = "Base64Imagem",
                Descricao = "Descrição do produto teste"
            };

            // Configurar o mock do MediatorHandler para simular publicação de notificações
            _mediatorHandlerMock.Setup(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()));

            // Configurar o mock para retornar um produto simulado
            _mediatorHandlerMock.Setup(x => x.EnviarComando<AdicionarProdutoCommand, ProdutoOutput>(It.IsAny<AdicionarProdutoCommand>()))
                .ReturnsAsync(new ProdutoOutput
                {
                    Id = Guid.NewGuid(),
                    Nome = produtoInput.Nome,
                    Valor = produtoInput.Valor
                });

            // Act
            var resultado = await _produtosController.Post(produtoInput);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(resultado);
            var produtoRetornado = Assert.IsType<ProdutoOutput>(objectResult.Value);
            Assert.Equal(produtoInput.Nome, produtoRetornado.Nome);
            Assert.Equal(produtoInput.Valor, produtoRetornado.Valor);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [Fact]
        public async Task AoChamarPost_DeveRetornarBadRequest_QuandoCadastroProdutoInvalido()
        {
            var domainNotificationHandler = _serviceProvideStartTup.GetRequiredService<INotificationHandler<DomainNotification>>();
            var mediatorHandler = _serviceProvideStartTup.GetRequiredService<IMediatorHandler>();

            var produtosController = new ProdutosController(
                _produtosQueriesMock.Object,
                domainNotificationHandler,
                mediatorHandler
            );

            var produtoInput = new ProdutoInput
            {
                CategoriaId = Guid.Empty, // CategoriaId inválido
                Nome = "", // Nome inválido
                Ativo = false,
                Valor = 0, // Valor inválido
                Imagem = "", // Imagem inválida
                Descricao = "" // Descrição inválida
            };

            // Act
            var resultado = await produtosController.Post(produtoInput);

            // Assert
            var badRequestObjectResult = Assert.IsType<ObjectResult>(resultado);
            var mensagensErro = Assert.IsType<List<string>>(badRequestObjectResult.Value);
            Assert.Contains(AdicionarProdutoValidation.IdCategoriaErroMsg, mensagensErro);
            Assert.Contains(AdicionarProdutoValidation.NomeErroMsg, mensagensErro);
            Assert.Contains(AdicionarProdutoValidation.ValorErroMsg, mensagensErro);
            Assert.Contains("Imagem é obrigatório", mensagensErro);
            Assert.Contains("Descrição é obrigatório", mensagensErro);
        }

        [Fact]
        public async Task AoChamarPost_DeveRetornarStatusCode500_QuandoOcorreExcecao()
        {
            var produtoInput = new ProdutoInput
            {
                CategoriaId = Guid.NewGuid(),
                Nome = "Produto Teste",
                Ativo = true,
                Valor = 100.00m,
                Imagem = "Base64Imagem",
                Descricao = "Descrição do produto teste"
            };

            // Configurar o mock para lançar uma exceção
            _mediatorHandlerMock.Setup(x => x.EnviarComando<AdicionarProdutoCommand, ProdutoOutput>(It.IsAny<AdicionarProdutoCommand>()))
                .ThrowsAsync(new Exception("Simulando uma exceção"));

            // Act
            var resultado = await _produtosController.Post(produtoInput);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar adicionar produto. Erro: Simulando uma exceção", objectResult.Value);
        }
        #endregion

        #region Testes unitários do metodo Put
        [Fact]
        public async Task AoChamarPut_DeveRetornarOk_QuandoAtualizacaoProdutoBemSucedida()
        {
            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(new ProdutoDto
                {
                    Id = Guid.NewGuid(),
                    CategoriaId = Guid.NewGuid(),
                    Nome = "Nome Antigo",
                    Ativo = true,
                    Valor = 100.00m,
                    Imagem = "Base64ImagemAntiga",
                    Descricao = "Descrição Antiga"
                });

            var produtoEditarInput = new ProdutoEditarInput
            {
                Id = Guid.NewGuid(),
                CategoriaId = Guid.NewGuid(),
                Nome = "Novo Nome",
                Ativo = true,
                Valor = 150.00m,
                Imagem = "Base64NovaImagem",
                Descricao = "Nova descrição do produto"
            };

            var command = new AtualizarProdutoCommand(produtoEditarInput);
            var produtoAtualizado = new ProdutoOutput
            {
                Id = produtoEditarInput.Id,
                Nome = produtoEditarInput.Nome,
                Valor = produtoEditarInput.Valor
            };

            _mediatorHandlerMock.Setup(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()));
            _mediatorHandlerMock.Setup(x => x.EnviarComando<AtualizarProdutoCommand, ProdutoOutput>(It.IsAny<AtualizarProdutoCommand>()))
                .ReturnsAsync(produtoAtualizado);

            // Act
            var resultado = await _produtosController.Put(produtoEditarInput);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(resultado);
            var produtoRetornado = Assert.IsType<ProdutoOutput>(objectResult.Value);
            Assert.Equal(produtoEditarInput.Nome, produtoRetornado.Nome);
            Assert.Equal(produtoEditarInput.Valor, produtoRetornado.Valor);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }

        [Fact]
        public async Task AoChamarPut_DeveRetornarBadRequest_QuandoCadastroProdutoInvalido()
        {
            var domainNotificationHandler = _serviceProvideStartTup.GetRequiredService<INotificationHandler<DomainNotification>>();
            var mediatorHandler = _serviceProvideStartTup.GetRequiredService<IMediatorHandler>();

            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(new ProdutoDto
                {
                    Id = Guid.NewGuid(),
                    CategoriaId = Guid.NewGuid(),
                    Nome = "Nome Antigo",
                    Ativo = true,
                    Valor = 100.00m,
                    Imagem = "Base64ImagemAntiga",
                    Descricao = "Descrição Antiga"
                });

            var produtosController = new ProdutosController(
                _produtosQueriesMock.Object,
                domainNotificationHandler,
                mediatorHandler
            );

            var produtoInput = new ProdutoEditarInput
            {
                CategoriaId = Guid.Empty, // CategoriaId inválido
                Nome = "", // Nome inválido
                Ativo = false,
                Valor = 0, // Valor inválido
                Imagem = "", // Imagem inválida
                Descricao = "" // Descrição inválida
            };

            // Act
            var resultado = await produtosController.Put(produtoInput);

            // Assert
            var badRequestObjectResult = Assert.IsType<ObjectResult>(resultado);
            var mensagensErro = Assert.IsType<List<string>>(badRequestObjectResult.Value);
            Assert.Contains(AdicionarProdutoValidation.IdCategoriaErroMsg, mensagensErro);
            Assert.Contains(AdicionarProdutoValidation.NomeErroMsg, mensagensErro);
            Assert.Contains(AdicionarProdutoValidation.ValorErroMsg, mensagensErro);
            Assert.Contains("Imagem é obrigatório", mensagensErro);
            Assert.Contains("Descrição é obrigatório", mensagensErro);
        }

        [Fact]
        public async Task AoChamarPut_DeveRetornarNotFound_QuandoProdutoNaoEncontrado()
        {
            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync((ProdutoDto)null);  // Produto não encontrado

            var produtoEditarInput = new ProdutoEditarInput
            {
                Id = Guid.NewGuid(),
                CategoriaId = Guid.NewGuid(),
                Nome = "Novo Nome",
                Ativo = true,
                Valor = 150.00m,
                Imagem = "Base64NovaImagem",
                Descricao = "Nova descrição do produto"
            };

            _mediatorHandlerMock.Setup(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()));

            _mediatorHandlerMock.Setup(x => x.EnviarComando<AtualizarProdutoCommand, ProdutoOutput>(It.IsAny<AtualizarProdutoCommand>()))
                .ReturnsAsync((ProdutoOutput)null);

            // Act
            var resultado = await _produtosController.Put(produtoEditarInput);

            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(resultado);
            var mensagemErro = Assert.IsType<string>(notFoundObjectResult.Value);
            Assert.Equal("Produto não encontrado.", mensagemErro);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundObjectResult.StatusCode);
        }

        [Fact]
        public async Task AoChamarPut_DeveRetornarStatusCode500_QuandoOcorreExcecao()
        {

            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(new ProdutoDto
                {
                    Id = Guid.NewGuid(),
                    CategoriaId = Guid.NewGuid(),
                    Nome = "Nome Antigo",
                    Ativo = true,
                    Valor = 100.00m,
                    Imagem = "Base64ImagemAntiga",
                    Descricao = "Descrição Antiga",
                    DataCadastro = DateTime.Now
                });

            var produtoEditarInput = new ProdutoEditarInput
            {
                Id = Guid.NewGuid(),
                CategoriaId = Guid.NewGuid(),
                Nome = "Novo Nome",
                Ativo = true,
                Valor = 150.00m,
                Imagem = "Base64NovaImagem",
                Descricao = "Nova descrição do produto"
            };

            _mediatorHandlerMock.Setup(x => x.EnviarComando<AtualizarProdutoCommand, ProdutoOutput>(It.IsAny<AtualizarProdutoCommand>()))
                .ThrowsAsync(new Exception("Simulando uma exceção"));

            // Act
            var resultado = await _produtosController.Put(produtoEditarInput);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar atualizar produto. Erro: Simulando uma exceção", objectResult.Value);
        }
        #endregion

        #region Testes unitários do metodo Delete
        [Fact]
        public async Task AoChamarDelete_DeveRetornarOk_QuandoProdutoExcluidoComSucesso()
        {

            _produtosQueriesMock.Setup(x => x.ObterPorId(It.IsAny<Guid>()))
                .ReturnsAsync(new ProdutoDto
                {
                    Id = Guid.NewGuid(),
                    CategoriaId = Guid.NewGuid(),
                    Nome = "Nome Antigo",
                    Ativo = true,
                    Valor = 100.00m,
                    Imagem = "Base64ImagemAntiga",
                    Descricao = "Descrição Antiga",
                    DataCadastro = DateTime.Now
                });

            var idProduto = Guid.NewGuid();

            _mediatorHandlerMock.Setup(x => x.PublicarNotificacao(It.IsAny<DomainNotification>()));

            _mediatorHandlerMock.Setup(x => x.EnviarComando<RemoverProdutoCommand, bool>(It.IsAny<RemoverProdutoCommand>()))
                               .ReturnsAsync(true)
                               .Callback<RemoverProdutoCommand>(cmd =>
                                   Assert.Equal(idProduto, cmd.idProduto));

            // Act
            var resultado = await _produtosController.Delete(idProduto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal("Produto excluído com sucesso.", okResult.Value);
        }

        [Fact]
        public async Task AoChamarDelete_DeveRetornarNotFound_QuandoProdutoNaoEncontrado()
        {
            // Arrange
            var idProdutoInexistente = Guid.NewGuid();

            _produtosQueriesMock.Setup(x => x.ObterPorId(idProdutoInexistente))
                               .ReturnsAsync((ProdutoDto)null);
            // Act
            var resultado = await _produtosController.Delete(idProdutoInexistente);

            // Assert
            Assert.IsType<NotFoundObjectResult>(resultado);
        }

        [Fact]
        public async Task AoChamarDelete_DeveRetornarStatusCode500_QuandoOcorreExcecao()
        {
            // Arrange
            var idProduto = Guid.NewGuid();

            _produtosQueriesMock.Setup(x => x.ObterPorId(idProduto))
                               .ThrowsAsync(new Exception("Erro ao acessar o banco de dados"));

            // Act
            var resultado = await _produtosController.Delete(idProduto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar excluir produto. Erro: Erro ao acessar o banco de dados", objectResult.Value);
        }

        public void Dispose()
        {
            _produtosController.Dispose();
            _serviceProvider.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
