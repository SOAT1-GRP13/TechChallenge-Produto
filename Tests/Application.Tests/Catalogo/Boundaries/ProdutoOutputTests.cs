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
            Assert.Equal(produtoOutput.Nome, "Produto Teste");
            Assert.Equal(produtoOutput.Descricao, "Descrição do Produto Teste");
            Assert.True(produtoOutput.Ativo);
            Assert.Equal(produtoOutput.Valor, 99.99m);
            Assert.Equal(produtoOutput.Imagem, "data:image/jpeg;base64,...");
        }
    }
}
