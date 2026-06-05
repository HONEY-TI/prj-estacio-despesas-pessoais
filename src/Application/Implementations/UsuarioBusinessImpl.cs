using AutoMapper;
using Application.Abstractions;
using Application.Dtos;
using GlobalException.CustomExceptions;
using GlobalException.CustomExceptions.Core;
using Repository.UnitOfWork.Abstractions;
using Domain.Core.ValueObject;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Repository.Persistency.Generic;

namespace Application.Implementations;

public class UsuarioBusinessImpl<Dto> : BusinessBase<Dto, Usuario>, IUsuarioBusiness<Dto> where Dto : UsuarioDto, new()
{
    private readonly IRepositorio<Usuario> _repositorio;
    private readonly IMapper _mapper;

    public UsuarioBusinessImpl(IMapper mapper, IRepositorio<Usuario> repositorio, IUnitOfWork<Usuario>? unitOfWork)
        : base(mapper, repositorio, unitOfWork)
    {
        _mapper = mapper;
        _repositorio = repositorio;
    }

    private async Task IsValidPerfilAdministrador(Dto dto)
    {
        var adm = await UnitOfWork!.Repository.Get(dto.UsuarioId);
        if (adm.PerfilUsuario != PerfilUsuario.Perfil.Admin)
            throw new UsuarioNaoAutorizadoException();
    }

    private async Task IsValidPerfilAdministrador(Usuario usuario)
    {
        var adm = await UnitOfWork!.Repository.Get(usuario.Id);
        if (adm.PerfilUsuario != PerfilUsuario.Perfil.Admin)
            throw new UsuarioNaoAutorizadoException();
    }

    private async Task<byte[]> ConvertToImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new CustomException("Arquivo inválido.");

        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            throw new CustomException("Apenas arquivos jpg, jpeg ou png são aceitos.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public override async Task<Dto> Create(Dto dto)
    {
        await IsValidPerfilAdministrador(dto);
        var usuario = Mapper.Map<Usuario>(dto).CreateUsuario(Mapper.Map<Usuario>(dto));
        await UnitOfWork!.Repository.Insert(usuario);
        await UnitOfWork.CommitAsync();
        var created = await UnitOfWork.Repository.Get(dto.UsuarioId);
        return Mapper.Map<Dto>(created);
    }

    public override async Task<List<Dto>> FindAll(Guid idUsuario)
    {
        var usuario = await UnitOfWork!.Repository.Find(u => u.Id == idUsuario);
        await IsValidPerfilAdministrador(usuario.FirstOrDefault());
        var all = await UnitOfWork.Repository.GetAll();
        return Mapper.Map<List<Dto>>(all);
    }

    public override async Task<Dto> Update(Dto dto)
    {
        try
        {
            var usuario = Mapper.Map<Usuario>(dto);
            await UnitOfWork!.Repository.Update(usuario);
            await UnitOfWork.CommitAsync();
            var updated = await UnitOfWork.Repository.Get(usuario.Id)
                ?? throw new UsuarioNaoEncontradoException();
            return Mapper.Map<Dto>(updated);
        }
        catch
        {
            throw;
        }
    }

    public override async Task<Dto> FindById(Guid id)
    {
        try
        {
            var usuario = await UnitOfWork!.Repository.Find(u => u.Id == id)
                ?? throw new UsuarioNaoEncontradoException();
            return Mapper.Map<Dto>(usuario.FirstOrDefault());
        }
        catch
        {
            throw;
        }        
    }

    public override async Task<bool> Delete(Dto dto)
    {
        try
        {
            await IsValidPerfilAdministrador(dto);
            var usuario = Mapper.Map<Usuario>(dto);
            await UnitOfWork!.Repository.Delete(usuario.Id);
            await UnitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<byte[]> GetProfileImage(Guid userIdentity)
    {
        var usuario = await UnitOfWork!.Repository.Get(userIdentity)
            ?? throw new UsuarioNaoEncontradoException();
        return usuario.Profile ?? Array.Empty<byte>();
    }

    public async Task<byte[]> UpdateProfileImage(Guid userIdentity, IFormFile file)
    {
        try
        {
            var usuario = await UnitOfWork!.Repository.Get(userIdentity)
                ?? throw new UsuarioNaoEncontradoException();

            usuario.Profile = await ConvertToImage(file);
            await UnitOfWork!.Repository.Update(usuario);
            await UnitOfWork.CommitAsync();
            return usuario.Profile;
        }
        catch
        {
            throw new CustomException("Erro ao atualizar a imagem de perfil. Tente novamente mais tarde.");
        }
    }
}
