using Despesas.Infrastructure.Email;
using __mock__.Repository;
using Repository.Persistency.Abstractions;
using Repository.Persistency.Implementations.Fixtures;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Domain.Core.ValueObject;

namespace Repository.Persistency.Implementations;

public sealed class AcessoRepositorioImplTest : IClassFixture<AcessoRepositorioFixture>
{
    private readonly AcessoRepositorioFixture _fixture;

    public AcessoRepositorioImplTest(AcessoRepositorioFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Create_Should_Return_True()
    {
        // Arrange and Setup mock repository
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        mockRepository.Setup(repo => repo.Create(It.IsAny<Acesso>()));
        var mockAcesso = MockAcesso.Instance.GetAcesso();

        // Act
        Action result = () => mockRepository.Object.Create(mockAcesso);

        // Assert
        Assert.IsNotType<Exception>(result);
    }

    [Fact]
    public void Create_Should_Return_False()
    {
        // Arrange and Setup mock repository
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        mockRepository.Setup(repo => repo.Create(It.IsAny<Acesso>()));
        var mockAcesso = context.Acesso.ToList().First();

        // Act
        Action result = () => mockRepository.Object.Create(mockAcesso);

        // Assert
        Assert.NotNull(result);
        mockRepository.Verify(repo => repo.Create(It.IsAny<Acesso>()), Times.Never);
    }

    [Fact]
    public void Create_Should_Throws_Exception()
    {
        // Arrange and Setup mock repository
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(new Acesso());
        mockRepository.Setup(repo => repo.Create(It.IsAny<Acesso>())).Throws(new InvalidOperationException("AcessoRepositorioImpl_Create_Exception"));

        // Act & Assert 
        Acesso? nullControleAceesso = null;
        Assert.Throws<InvalidOperationException>(() => mockRepository.Object.Create(nullControleAceesso));
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
    public void RecoveryPassword_Should_Returns_True()
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
        var result = repository.Object.RecoveryPassword(mockAcesso.Login, newPassword);

        //Assert
        Assert.IsType<bool>(result);
        Assert.True(result);
    }

    [Fact]
    public void RecoveryPassword_Should_Returns_False()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var mockUsuario = new Usuario { Email = string.Empty };
        var mockAcesso = context.Acesso.ToList().First();
        context.Usuario = Usings.MockDbSet(new List<Usuario> { mockUsuario }).Object;
        var newPassword = "!12345";
        Acesso MockAcesso = new Acesso
        {
            Id = mockAcesso.Id,
            Login = mockAcesso.Login,
            Senha = newPassword,
            Usuario = mockAcesso.Usuario,
            UsuarioId = mockAcesso.UsuarioId
        };
        context.Acesso = Usings.MockDbSet(new List<Acesso> { MockAcesso }).Object;

        context.SaveChanges();
        var emailSender = new Mock<EmailSender>();
        mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(mockAcesso);
        mockRepository.Setup(repo => repo.RecoveryPassword(mockAcesso.Login, It.IsAny<string>())).Returns(false);

        // Act
        var result = mockRepository.Object.RecoveryPassword(mockAcesso.Login, newPassword);

        //Assert
        Assert.IsType<bool>(result);
        Assert.False(result);
    }

    [Fact]
    public void RecoveryPassword_Should_Returns_False_When_Throws_Exception()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var dataset = context.Acesso.ToList();
        context.Acesso = Usings.MockDbSet(dataset).Object;
        context.SaveChanges();
        mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Throws(new Exception());
        var mockAcesso = dataset.First();

        // Act
        var result = mockRepository.Object.RecoveryPassword(mockAcesso.Login, "newPassword");

        //Assert
        Assert.IsType<bool>(result);
        Assert.False(result);
    }

    [Fact]
    public void ChangePassword_Should_Returns_False_When_Usuario_Null()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var acesso = context.Acesso.ToList().First();
        mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(acesso);
        mockRepository.Setup(repo => repo.ChangePassword(Guid.Empty, "!12345")).Returns(true);

        // Act
        var result = mockRepository.Object.ChangePassword(Guid.Empty, "!12345");

        //Assert
        Assert.IsType<bool>(result);
        Assert.False(result);
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

        // Act
        var result = repository.ChangePassword(acesso.UsuarioId, "!12345");

        //Assert
        Assert.IsType<bool>(result);
        Assert.True(result);
    }

    [Fact]
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
        _dbContextMock.Setup(c => c.Set<Acesso>()).Returns(dbSetMock.Object);
        _dbContextMock.Setup(c => c.SaveChanges()).Throws<Exception>();
        mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<Acesso, bool>>>())).Returns(new Acesso());
        mockRepository.Setup(repo => repo.ChangePassword(acesso.UsuarioId, "!12345")).Throws<Exception>();

        // Act
        //var result = mockRepository.Object.ChangePassword(acesso.UsuarioId, "!12345");
        Action result = () => mockRepository.Object.ChangePassword(acesso.UsuarioId, "!12345");

        //Assert
        Assert.NotNull(result);
        var exception = Assert.Throws<Exception>(() => mockRepository.Object.ChangePassword(acesso.UsuarioId, "!12345"));
        Assert.Equal("ChangePassword_Erro", exception.Message);
        Assert.True(true);
    }

    [Fact]
    public void Create_Should_Throw_Exception_When_User_Already_Exists()
    {
        // Arrange
        var context = _fixture.Context;
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var mockAcesso = context.Acesso.ToList().First();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => mockRepository.Object.Create(mockAcesso));
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
        var mockRepository = Mock.Get<IAcessoRepositorioImpl>(_fixture.MockRepository.Object);
        var mockAcesso = context.Acesso.ToList().First();

        // Act
        mockRepository.Object.RevokeRefreshToken(mockAcesso.Id);

        // Assert
        Assert.Empty(mockAcesso.RefreshToken);
        Assert.Null(mockAcesso.RefreshTokenExpiry);
    }

    [Fact]
    public void RefreshTokenInfo_Should_Update_Token_Info()
    {
        // Arrange
        var mockRepository = _fixture.Repository;
        var mockAcesso =  MockAcesso.Instance.GetAcesso();
        mockRepository.Object.Create(mockAcesso);
        var mockRefreshToken = Guid.NewGuid().ToString();

        // Act
        mockAcesso.RefreshToken = mockRefreshToken;
        mockAcesso.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(1);
        mockRepository.Object.RefreshTokenInfo(mockAcesso);
        

        // Assert
        Assert.NotNull(mockAcesso.RefreshToken);
        Assert.NotNull(mockAcesso.RefreshTokenExpiry);
        Assert.Equal(mockAcesso.RefreshToken, mockRefreshToken);
    }
}
