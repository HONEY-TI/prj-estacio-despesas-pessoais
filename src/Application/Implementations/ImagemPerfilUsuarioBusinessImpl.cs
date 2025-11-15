using AutoMapper;
using Application.Abstractions;
using Application.Dtos;
using Infrastructure.Amazon.Abstractions;
using Domain.Entities;
using Repository.Persistency.Generic;

namespace Application.Implementations;
public class ImagemPerfilUsuarioBusinessImpl<Dto, DtoUsuario> : IImagemPerfilUsuarioBusiness<Dto, DtoUsuario> where Dto : ImagemPerfilDto, new() where DtoUsuario : UsuarioDto, new()
{
    private readonly IMapper _mapper;
    private readonly IRepositorio<ImagemPerfilUsuario> _repositorio;
    private readonly IRepositorio<Usuario> _repositorioUsuario;
    private readonly IAmazonS3Bucket _amazonS3Bucket;
    public ImagemPerfilUsuarioBusinessImpl(IMapper mapper, IRepositorio<ImagemPerfilUsuario> repositorio, IRepositorio<Usuario> repositorioUsuario, IAmazonS3Bucket amazonS3Bucket)
    {
        _mapper = mapper;
        _repositorio = repositorio;
        _repositorioUsuario = repositorioUsuario;
        _amazonS3Bucket = amazonS3Bucket;
    }

    public async Task<Dto> Create(Dto dto)
    {
        ImagemPerfilUsuario? perfilFile = _mapper.Map<ImagemPerfilUsuario>(dto);        
        try
        {
            perfilFile.Url = _amazonS3Bucket.WritingAnObjectAsync(perfilFile, dto.Arquivo).GetAwaiter().GetResult();
            perfilFile.Usuario = _repositorioUsuario.Get(perfilFile.UsuarioId) ?? throw new();
            _repositorio.Insert(perfilFile);
            return await Task.FromResult(_mapper.Map<Dto>(perfilFile));
        }
        catch (Exception ex) 
        {
            _amazonS3Bucket.DeleteObjectNonVersionedBucketAsync(perfilFile).GetAwaiter();
            throw new ArgumentException(ex.Message);
           //throw new ArgumentException("Erro ao criar imagem de perfil.");
        }
    }

    public async Task<List<Dto>> FindAll(Guid idUsuario)
    {
        var lstPerfilFile = _repositorio.GetAll();
        return await Task.FromResult(_mapper.Map<List<Dto>>(lstPerfilFile));
    }

    public async Task<Dto> FindById(Guid id, Guid idUsuario)
    {
        var imagemPerfilUsuario = _mapper.Map<Dto>(_repositorio.Get(id));
        if (imagemPerfilUsuario.UsuarioId != idUsuario)
            return null;

        return await Task.FromResult(imagemPerfilUsuario);
    }

    public async Task<Dto> Update(Dto dto)
    {
        try
        {
            var validImagemPerfil = _repositorio.GetAll().Find(prop => prop.UsuarioId.Equals(dto.UsuarioId));
            if (validImagemPerfil == null)
                throw new();

            _amazonS3Bucket.DeleteObjectNonVersionedBucketAsync(validImagemPerfil).GetAwaiter().GetResult();
            validImagemPerfil.Url = _amazonS3Bucket.WritingAnObjectAsync(validImagemPerfil, dto.Arquivo).GetAwaiter().GetResult();
            _repositorio.Update(validImagemPerfil);
            return await Task.FromResult(_mapper.Map<Dto>(validImagemPerfil));
        }
        catch
        {
            throw new ArgumentException("Erro ao atualizar iamgem do perfil!");
        }
    }

    public async Task<bool> Delete(Guid idUsuario)
    {
        var imagemPerfilUsuario = _repositorio.Find(prop => prop.UsuarioId.Equals(idUsuario)).FirstOrDefault();
        if (imagemPerfilUsuario != null)
        {
            var result = _amazonS3Bucket.DeleteObjectNonVersionedBucketAsync(imagemPerfilUsuario).GetAwaiter().GetResult();
            if (result)
            {
                _repositorio.Delete(new ImagemPerfilUsuario { Id = imagemPerfilUsuario.Id });
                return await Task.FromResult(true);
            }
        }
        return false;
    }
}