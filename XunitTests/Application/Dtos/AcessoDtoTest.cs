using Despesas.Application.Dtos;

namespace Application.Dtos;
public sealed class AcessoDtoTest
{

    [Theory]
    [InlineData("Usuario 1 ", "Teste Usuario 1", "(21) 99999-9999", "user1@user.com", "senhaUser1", "senhaUser1")]
    [InlineData("Usuario 2 ", "Teste Usuario 2", "(21) 99999-9999", "user2@user.com", "senhaUser2", "senhaUser2")]
    [InlineData("Usuario 3 ", "Teste Usuario 3", "(21) 99999-9999", "user3@user.com", "senhaUser3", "senhaUser3")]
    public void AcessoDto_Should_Set_Properties_Correctly(string nome, string sobreNome, string telefone, string email, string senha, string confirmaSenha)
    {
        // Arrange and Act
        var id = Guid.NewGuid();

        var acessoDto = new AcessoDto
        {
            Id = id,
            Nome = nome,
            SobreNome = sobreNome,
            Telefone = telefone,
            Email = email,
            Senha = senha,
            ConfirmaSenha = confirmaSenha
        };

        // Assert
        Assert.Equal(id, acessoDto.Id);
        Assert.Equal(nome, acessoDto.Nome);
        Assert.Equal(sobreNome, acessoDto.SobreNome);
        Assert.Equal(telefone, acessoDto.Telefone);
        Assert.Equal(email, acessoDto.Email);
        Assert.Equal(senha, acessoDto.Senha);
        Assert.Equal(confirmaSenha, acessoDto.ConfirmaSenha);
    }
}