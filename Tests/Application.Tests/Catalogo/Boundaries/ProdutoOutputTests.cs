using Application.Catalogo.Boundaries;

namespace Application.Tests.Catalogo.Boundaries
{
    public class ProdutoOutputTests
    {
        [Fact]
        public void AtribuirEObterValores_DeveFuncionarCorretamente()
        {
            // Arrange
            var produtoOutput = new ProdutoOutput
            {
                Id = Guid.NewGuid(),
                CategoriaId = Guid.NewGuid(),
                Nome = "Produto Teste",
                Descricao = "Descrição do Produto Teste",
                Ativo = true,
                Valor = 99.99m,
                Imagem = "data:image/jpeg;base64,..."
            };

            // Act & Assert
            Assert.Equal("Produto Teste", produtoOutput.Nome);
            Assert.Equal("Descrição do Produto Teste", produtoOutput.Descricao);
            Assert.True(produtoOutput.Ativo);
            Assert.Equal(99.99m, produtoOutput.Valor);
            Assert.Equal("data:image/jpeg;base64,...", produtoOutput.Imagem);
        }
    }
}
