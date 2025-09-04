namespace Domain;
public sealed class AcessoTest
{
    [Theory]
    [InlineData("Teste@teste.com", "Teste password 1 ")]
    [InlineData("Teste2@teste.com", "Teste password 2 ")]
    [InlineData("Teste3@teste.com", "Teste password 3 ")]
    public void Acesso_Should_Set_Properties_Correctly(string login, string senha)
    {
        var mockUsuario = Mock.Of<Usuario>();
        var id = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();

        // Arrange and Act
        var acesso = new Acesso
        {
            Id = id,
            Login = login,
            Senha = senha,
            UsuarioId = usuarioId,
            Usuario = mockUsuario

        };

        // Assert
        Assert.Equal(id, acesso.Id);
        Assert.Equal(login, acesso.Login);
        Assert.NotNull(acesso.Senha);
        Assert.Equal(usuarioId, acesso.UsuarioId);
        Assert.Equal(mockUsuario, acesso.Usuario);
    }
}