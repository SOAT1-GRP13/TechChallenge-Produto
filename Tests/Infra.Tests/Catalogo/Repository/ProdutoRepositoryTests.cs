using Moq;
using Infra.Catalogo;
using Domain.Catalogo;
using Infra.Catalogo.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infra.Tests.Catalogo.Repository
{
    public class ProdutoRepositoryTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        public ProdutoRepositoryTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            _mockConfiguration = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task AoObterTodos_DeveRetornarTodosProdutos()
        {
            

            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            // Povoar o banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                context.Produtos.Add(new Produto("Produto 1", "Descrição 1", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem1.jpg"));
                context.Produtos.Add(new Produto("Produto 2", "Descrição 2", true, 200m, Guid.NewGuid(), DateTime.UtcNow, "imagem2.jpg"));
                await context.SaveChangesAsync();
            }

            // Utilizar o contexto populado para o teste
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                var produtos = await repository.ObterTodos();

                // Assert
                Assert.NotNull(produtos);             
            }
        }

        [Fact]
        public async Task AoObterObterPorId_DeveRetornarIdSelecionado()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var produto1 = new Produto("Produto 1", "Descrição 1", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem1.jpg");

            // Povoar o banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                context.Produtos.Add(produto1);
                context.Produtos.Add(new Produto("Produto 2", "Descrição 2", true, 200m, Guid.NewGuid(), DateTime.UtcNow, "imagem2.jpg"));
                await context.SaveChangesAsync();
            }

            // Utilizar o contexto populado para o teste
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                var produto = await repository.ObterPorId(produto1.Id);

                // Assert
                Assert.NotNull(produto);
                Assert.Equal(produto1.Id, produto.Id);
            }
        }

        [Fact]
        public async Task AoObterObterPorCategoria_DeveRetornarProdutoPorCategoria()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var produto1 = new Produto("Produto 1", "Descrição 1", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem1.jpg");
            var categoria = new Categoria("Categoria 1", 1, Guid.NewGuid());
            produto1.AlterarCategoria(categoria);

            // Povoar o banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                context.Produtos.Add(produto1);
                context.Produtos.Add(new Produto("Produto 2", "Descrição 2", true, 200m, Guid.NewGuid(), DateTime.UtcNow, "imagem2.jpg"));
                await context.SaveChangesAsync();
            }

            // Utilizar o contexto populado para o teste
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                var produtos = await repository.ObterPorCategoria(produto1.Categoria.Codigo);

                // Assert
                Assert.NotNull(produtos);
                Assert.Equal(1, produtos.Count());
            }
        }

        [Fact]
        public async Task AoObterCategorias_DeveRetornarTodasCategorias()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            // Povoar o banco de dados em memória com categorias
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                context.Categorias.Add(new Categoria("Categoria 1", 1));
                context.Categorias.Add(new Categoria("Categoria 2", 2));
                await context.SaveChangesAsync();
            }

            // Utilizar o contexto populado para o teste
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                var categorias = await repository.ObterCategorias();

                // Assert
                Assert.NotNull(categorias);
            }
        }

        [Fact]
        public async Task AoAdicionar_DeveInserirProduto()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var produto = new Produto("Produto Teste", "Descrição Teste", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem_teste.jpg");

            // Adicionando o produto ao banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                await repository.Adicionar(produto);
            }

            // Verificando se o produto foi adicionado
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var produtoAdicionado = await context.Produtos.FirstOrDefaultAsync(p => p.Id == produto.Id);

                // Assert
                Assert.NotNull(produtoAdicionado);
                Assert.Equal("Produto Teste", produtoAdicionado.Nome);
            }
        }

        [Fact]
        public async Task AoAtualizar_DeveAlterarDadosDoProduto()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var produtoId = Guid.NewGuid();
            var produtoOriginal = new Produto("Produto Original", "Descrição Original", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem_original.jpg") { Id = produtoId };

            // Criar e adicionar um produto ao banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                context.Produtos.Add(produtoOriginal);
                await context.SaveChangesAsync();
            }

            var produtoAtualizado = new Produto("Produto Atualizado", "Descrição Atualizada", false, 200m, Guid.NewGuid(), DateTime.UtcNow, "imagem_atualizada.jpg") { Id = produtoId };

            // Atualizar o produto no banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                await repository.Atualizar(produtoAtualizado);
            }

            // Verificar se o produto foi atualizado
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var produtoDoBanco = await context.Produtos.FirstOrDefaultAsync(p => p.Id == produtoId);

                // Assert
                Assert.NotNull(produtoDoBanco);
                Assert.Equal("Produto Atualizado", produtoDoBanco.Nome);
                Assert.Equal("Descrição Atualizada", produtoDoBanco.Descricao);
                Assert.Equal(200m, produtoDoBanco.Valor);
                Assert.False(produtoDoBanco.Ativo);
            }
        }

        [Fact]
        public async Task Remover_DeveExcluirProduto()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var produtoId = Guid.NewGuid();
            var produto = new Produto("Produto Teste", "Descrição Teste", true, 100m, Guid.NewGuid(), DateTime.UtcNow, "imagem_teste.jpg") { Id = produtoId };

            // Adicionar um produto ao banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                context.Produtos.Add(produto);
                await context.SaveChangesAsync();
            }

            // Remover o produto do banco de dados em memória
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                await repository.Remover(produto);
            }

            // Verificar se o produto foi removido
            await using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var produtoDoBanco = await context.Produtos.FirstOrDefaultAsync(p => p.Id == produtoId);

                // Assert
                Assert.Null(produtoDoBanco);
            }
        }

        [Fact]
        public void AdicionarCategoria_DeveInserirCategoria()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var categoria = new Categoria("Categoria Teste", 1);

            // Adicionando a categoria ao banco de dados em memória
            using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                repository.Adicionar(categoria);
                context.SaveChanges(); // Salvando as alterações, pois o método Adicionar para Categoria é síncrono
            }

            // Verificando se a categoria foi adicionada
            using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var categoriaAdicionada = context.Categorias.FirstOrDefault(c => c.Nome == "Categoria Teste");

                // Assert
                Assert.NotNull(categoriaAdicionada);
                Assert.Equal("Categoria Teste", categoriaAdicionada.Nome);
                Assert.Equal(1, categoriaAdicionada.Codigo);
            }
        }

        [Fact]
        public void AtualizarCategoria_DeveAlterarDadosDaCategoria()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var categoriaId = Guid.NewGuid();
            var categoriaOriginal = new Categoria("Categoria Original", 1) { Id = categoriaId };

            // Criar e adicionar uma categoria ao banco de dados em memória
            using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                context.Categorias.Add(categoriaOriginal);
                context.SaveChanges();
            }

            var categoriaAtualizada = new Categoria("Categoria Atualizada", 2) { Id = categoriaId };

            // Atualizar a categoria no banco de dados em memória
            using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var repository = new ProdutoRepository(context, _mockConfiguration.Object);
                repository.Atualizar(categoriaAtualizada);
                context.SaveChanges();
            }

            // Verificar se a categoria foi atualizada
            using (var context = new CatalogoContext(options, _mockConfiguration.Object))
            {
                var categoriaDoBanco = context.Categorias.FirstOrDefault(c => c.Id == categoriaId);

                // Assert
                Assert.NotNull(categoriaDoBanco);
                Assert.Equal("Categoria Atualizada", categoriaDoBanco.Nome);
                Assert.Equal(2, categoriaDoBanco.Codigo);
            }
        }

        [Fact]
        public void Dispose_DeveLiberarRecursos()
        {
            // Configura o banco de dados em memória
            var options = new DbContextOptionsBuilder<CatalogoContext>()
                .UseInMemoryDatabase(databaseName: "CatalogoTestDb")
                .Options;

            var context = new CatalogoContext(options, _mockConfiguration.Object);
            var repository = new ProdutoRepository(context, _mockConfiguration.Object);

            // Act
            repository.Dispose();

            // Assert
            Assert.Throws<ObjectDisposedException>(() => context.Set<Produto>().ToList());
        }
    }
}
