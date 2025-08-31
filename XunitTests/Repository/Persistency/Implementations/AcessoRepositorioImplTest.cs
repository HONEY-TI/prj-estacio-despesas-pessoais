using __mock__.Entities;
using __mock__.Repository;
using Despesas.Infrastructure.Email;
using Domain.Core.ValueObject;
using Microsoft.EntityFrameworkCore;
using Repository.Persistency.Abstractions;
using Repository.Persistency.Implementations.Fixtures;
using System.Linq.Expressions;

namespace Repository.Persistency.Implementations;

public sealed class AcessoRepositorioImplTest : IClassFixture<AcessoRepositorioFixture>
{
    private readonly AcessoRepositorioFixture _fixture;

    public AcessoRepositorioImplTest(AcessoRepositorioFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Create_Should_Insert_Acesso_When_New()
    {
        // Arrange 
        var context = _fixture.Context;
        var mockRepository = _fixture.Repository.Object;
        var mockAcesso = AcessoFaker.Instance.GetNewFaker();

        // Act
        mockRepository.Create(mockAcesso);

        // Assert
        var acesso = mockRepository.Find(c => c.Login == mockAcesso.Login);
        Assert.NotNull(acesso);
        Assert.Equal(mockAcesso.Login, acesso.Login);
    }    
    
    [Fact(Skip = "Uso do DRY com Global Excepition")]
    public void Create_Should_Throws_Exception()
    {
        // Arrange and Setup mock repository
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(new Acesso());
        mockRepository.Setup(repo => repo.Create(It.IsAny<Acesso>())).Throws(new InvalidOperationException("AcessoRepositorioImpl_Create_Exception"));

        // Act & Assert 
        Acesso? nullControleAceesso = null;
        Assert.Throws<Exception>(() => mockRepository.Object.Create(nullControleAceesso));
        //Assert.Equal("AcessoRepositorioImpl_Create_Exception", exception.Message);
    }

    [Fact]
    public void FindByEmail_Should_Returns_Acesso()
    {
        // Arrange and Setup Repository
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var mockAcesso = context.Acesso.ToList().First();

        // Act
        var result = mockRepository.Object.Find(c => c.Equals(mockAcesso));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Acesso>(result);
        Assert.Equal(mockAcesso, result);
    }

    [Fact]
    public void RecoveryPassword_Should_Run_Successfuly()
    {
        // Arrange
        var newPassword = "!12345";
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "RecoveryPassword_Should_Returns_True").Options;
        var context = new RegisterContext(options);
        context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.Admin));
        context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.User));
        context.SaveChanges();
        var lstAcesso = MockAcesso.Instance.GetAcessos().ToList();
        lstAcesso.ForEach(c => c.Usuario.PerfilUsuario = context.PerfilUsuario.First(tc => tc.Id == c.Usuario.PerfilUsuario.Id));
        var mockAcesso = new Acesso { Login = lstAcesso.Last().Login };
        context.AddRange(lstAcesso);
        context.SaveChanges();
        var repository = new Mock<AcessoRepositorioImpl>(context);

        // Act
        var result = () => repository.Object.RecoveryPassword(mockAcesso.Login, newPassword);

        //Assert
        Assert.IsNotType<Exception>(result);
    }

    [Fact(Skip = "Uso do DRY com Global Excepition")]
    public void RecoveryPassword_Should_Throws_True_Exception()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var mockUsuario = new Usuario { Email = string.Empty };
        var mockAcesso = context.Acesso.ToList().First();
        context.Usuario = Usings.MockDbSet(new List<Usuario> { mockUsuario }).Object;
        var newPassword = Guid.NewGuid().ToString();
        var emailSender = new Mock<EmailSender>();
        mockRepository.Setup(repo => repo.RecoveryPassword(mockAcesso.Login, It.IsAny<string>())).Throws(new Exception());

        // Act && Assert
        var exception = Assert.Throws<Exception>(() => mockRepository.Object.RecoveryPassword(Guid.NewGuid().ToString(), newPassword));
        Assert.IsType<Exception>(exception);
        Assert.Equal("RecoveryPassword_Erro", exception.Message);
    }

    [Fact(Skip = "Uso do DRY com Global Excepition")]
    public void RecoveryPassword_Should_Throws_Exception()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var dataset = context.Acesso.ToList();
        context.Acesso = Usings.MockDbSet(dataset).Object;
        context.SaveChanges();

        // Act && Assert
        var exception = Assert.Throws<Exception>(() => mockRepository.Object.RecoveryPassword("email@erro.com", "newPassword"));
        Assert.IsType<Exception>(exception);
        Assert.Equal("RecoveryPassword_Erro", exception.Message);
    }

    [Fact(Skip = "Uso do DRY com Global Excepition")]
    public void ChangePassword_Should_Throws_Erro_When_Usuario_Null()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var acesso = context.Acesso.ToList().First();
        mockRepository.Setup(repo => repo.ChangePassword(It.IsAny<Guid>(), It.IsAny<string>())).Throws(new Exception());

        // Act && Assert
        var exception = Assert.Throws<Exception>(() => mockRepository.Object.ChangePassword(Guid.Empty, Guid.NewGuid().ToString()));
        Assert.IsType<Exception>(exception);
        Assert.Equal("ChangePassword_Erro", exception.Message);
    }

    [Fact]
    public void ChangePassword_Should_Returns_True()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RegisterContext>().UseInMemoryDatabase(databaseName: "ChangePassword_Should_Returns_True").Options;
        var context = new RegisterContext(options);
        context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.Admin));
        context.PerfilUsuario.Add(new PerfilUsuario(PerfilUsuario.Perfil.User));
        context.SaveChanges();
        var lstAcesso = MockAcesso.Instance.GetAcessos();
        lstAcesso.ForEach(c => c.Usuario.PerfilUsuario = context.PerfilUsuario.First(tc => tc.Id == c.Usuario.PerfilUsuario.Id));
        context.AddRange(lstAcesso);
        context.SaveChanges();
        var repository = new AcessoRepositorioImpl(context);
        var acesso = context.Acesso.Last();

        // Act && Assert
        var result =  Assert.IsType<Action>(() =>  repository.ChangePassword(acesso.UsuarioId, "!12345"));
        Assert.IsNotType<Exception>(result);
    }

    [Fact(Skip = "Uso do DRY com Global Excepition")]
    public void ChangePassword_Should_Throws_Exception()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var acesso = context.Acesso.ToList().First();
        var mockUsuario = acesso.Usuario;
        mockUsuario.Email = "teste@teste.com";
        var dbSetMock = Usings.MockDbSet(context.Acesso.ToList());
        var _dbContextMock = new Mock<RegisterContext>(context);
        _dbContextMock.Setup(c => c.SaveChanges()).Throws<Exception>();

        // Act && Assert
        var exception = Assert.Throws<Exception>(() => mockRepository.Object.ChangePassword(Guid.NewGuid(), Guid.NewGuid().ToString()));
        Assert.IsType<Exception>(exception);
        Assert.Equal("ChangePassword_Erro", exception.Message);
    }

    [Fact(Skip = "Uso do DRY com Global Excepition")]
    public void Create_Should_Throw_Exception_When_User_Already_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var mockAcesso = context.Acesso.ToList().First();

        // Act & Assert
        Assert.Throws<Exception>(() => mockRepository.Object.Create(mockAcesso));
    }


    [Fact]
    public void FindById_Should_Return_Acesso_When_User_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var mockAcesso = context.Acesso.ToList().First();

        // Act
        var result = mockRepository.Object.Find(acesso => acesso.Id.Equals(mockAcesso.Id));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockAcesso, result);
    }

    [Fact]
    public void FindById_Should_Return_Null_When_User_Not_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var nonExistingId = -1;
        var repository = new AcessoRepositorioImpl(context);

        // Act
        var result = repository.Find(acesso => acesso.Id.Equals(nonExistingId));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RevokeToken_Should_Throw_Exception_When_User_Not_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var nonExistingId = Guid.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => mockRepository.Object.RevokeRefreshToken(nonExistingId));
    }

    [Fact]
    public void RevokeToken_Should_Remove_Token_When_User_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var mockAcesso = context.Acesso.Last();
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);

        // Act && Assert
        var exception = Assert.Throws<ArgumentException>(() => mockRepository.Object.RevokeRefreshToken(mockAcesso.UsuarioId));
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Token inexistente!", exception.Message);
    }


    [Fact]
    public void Create_Should_Throw_When_Login_Already_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new AcessoRepositorioImpl(context);
        var mockAcesso = context.Acesso.First();

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => repository.Create(mockAcesso));
    }

    [Fact]
    public void RecoveryPassword_Should_Update_Senha()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new AcessoRepositorioImpl(context);
        var mockAcesso = context.Acesso.First();
        var newPassword = "!12345";

        // Act
        repository.RecoveryPassword(mockAcesso.Login, newPassword);

        // Assert
        var result = context.Acesso.First(c => c.Login == mockAcesso.Login);
        Assert.Equal(newPassword, result.Senha);
    }

    [Fact]
    public void ChangePassword_Should_Update_Senha()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new AcessoRepositorioImpl(context);
        var mockAcesso = context.Acesso.First();
        var newPassword = "!54321";

        // Act
        repository.ChangePassword(mockAcesso.UsuarioId, newPassword);

        // Assert
        var result = context.Acesso.First(c => c.Id == mockAcesso.Id);
        Assert.Equal(newPassword, result.Senha);
    }

    [Fact]
    public void RevokeToken_Should_Clear_Token_When_User_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = _fixture.MockRepository.Object;
        var mockAcesso = context.Acesso.First();
        mockAcesso.RefreshToken = "valid-token";
        mockAcesso.RefreshTokenExpiry = DateTime.UtcNow.AddHours(1);

        // Act
        repository.RevokeRefreshToken(mockAcesso.Id);

        // Assert
        var result = context.Acesso.First(c => c.Id == mockAcesso.Id);
        Assert.Equal(string.Empty, result.RefreshToken);
        Assert.Null(result.RefreshTokenExpiry);
    }

    [Fact]
    public void FindByRefreshToken_Should_Return_Acesso()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = _fixture.MockRepository.Object;
        var mockAcesso = context.Acesso.First();
        mockAcesso.RefreshToken = "refresh-123";
        context.SaveChanges();

        // Act
        var result = repository.FindByRefreshToken("refresh-123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockAcesso.Login, result.Login);
    }

    [Fact]
    public void Find_Should_Return_Acesso_When_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new AcessoRepositorioImpl(context);
        var mockAcesso = context.Acesso.First();

        // Act
        var result = repository.Find(c => c.Id == mockAcesso.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockAcesso.Id, result.Id);
    }

    [Fact]
    public void Find_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new AcessoRepositorioImpl(context);

        // Act
        var result = repository.Find(c => c.Id == Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RefreshTokenInfo_Should_Update_Acesso()
    {
        // Arrange
        var context = _fixture.Context;
        var repository = new AcessoRepositorioImpl(context);
        var mockAcesso = context.Acesso.First();
        var originalToken = mockAcesso.RefreshToken;

        mockAcesso.RefreshToken = "new-refresh-token";

        // Act
        repository.RefreshTokenInfo(mockAcesso);

        // Assert
        var updatedAcesso = context.Acesso.First(c => c.Id == mockAcesso.Id);
        Assert.Equal("new-refresh-token", updatedAcesso.RefreshToken);
        Assert.NotEqual(originalToken, updatedAcesso.RefreshToken);
    }

}
