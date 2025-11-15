using AutoMapper;
using Application.Abstractions;
using GlobalException.CustomExceptions;
using GlobalException.CustomExceptions.Core;
using Repository.UnitOfWork.Abstractions;
using Domain.Core.ValueObject;
using Domain.Entities;
using MediatR;
using Repository.Persistency.Generic;

namespace Application.Implementations;
public class CategoriaBusinessImpl<Dto> : BusinessBase<Dto, Categoria>, ICategoriaBusiness<Dto, Categoria> where Dto : class, new()
{
    private readonly IMediator _mediator;
    private readonly IRepositorio<Categoria> _repositorio;
    public CategoriaBusinessImpl(IMediator mediator, IMapper mapper, IUnitOfWork<Categoria> unitOfWork, IRepositorio<Categoria> repositorio) : base(mapper, repositorio, unitOfWork)
    {
        _mediator = mediator;
        _repositorio = repositorio;
    }

    public override async Task<Dto> Create(Dto dto)
    {
        try
        {
            IsValidTipoCategoria(dto);
            var categoria = this.Mapper.Map<Categoria>(dto);
            await UnitOfWork.Repository.Insert(categoria);
            await UnitOfWork.CommitAsync();
            categoria = await UnitOfWork.Repository.Get(categoria.Id)
              ?? throw new CustomException("Não foi possível realizar o cadastro de uma nova categoria, tente mais tarde ou entre em contato com o suporte.");
            return this.Mapper.Map<Dto>(categoria);
        }
        catch
        {
            throw;
        }
    }

    public override async Task<List<Dto>> FindAll(Guid idUsuario)
    {
        var lstCategoria = await UnitOfWork.Repository.Find(c => c.UsuarioId == idUsuario);
        return this.Mapper.Map<List<Dto>>(lstCategoria);
    }

    public override async Task<Dto> FindById(Guid id, Guid idUsuario)
    {
        var result = await UnitOfWork.Repository.Find(c => c.Id == id && c.Usuario.Id == idUsuario);
        var categoria = result.FirstOrDefault();
        return this.Mapper.Map<Dto>(categoria);
    }

    public override async Task<Dto> Update(Dto dto)
    {
        try
        {
            IsValidTipoCategoria(dto);
            await IsValidCategoria(dto);
            var categoria = this.Mapper.Map<Categoria>(dto);
            await UnitOfWork.Repository.Update(categoria);
            await UnitOfWork.CommitAsync();
            categoria = await UnitOfWork.Repository.Get(categoria.Id)
                ?? throw new CategoriaUpdateException();
            return this.Mapper.Map<Dto>(categoria);
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
            await IsValidCategoria(dto);
            var categoria = this.Mapper.Map<Categoria>(dto);
            await UnitOfWork.Repository.Delete(categoria.Id);
            await UnitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void IsValidTipoCategoria(Dto dto)
    {
        var categoria = this.Mapper.Map<Categoria>(dto);
        if (categoria.TipoCategoriaId != (int)TipoCategoria.CategoriaType.Despesa && categoria.TipoCategoriaId != (int)TipoCategoria.CategoriaType.Receita)
            throw new TipoCategoriaInvalidaException();
    }

    private async Task IsValidCategoria(Dto dto)
    {
        var categoria = await UnitOfWork.Repository.Get(this.Mapper.Map<Categoria>(dto).Id);
        if (categoria.Usuario?.Id != categoria.UsuarioId)
            throw new CategoriaUsuarioInvalidaException();
    }

    public async Task<List<Dto>> FindByTipocategoria(Guid idUsuario, int idTipoCategoria)
    {
        var result = await UnitOfWork.Repository.Find(c => c.UsuarioId == idUsuario && c.TipoCategoria.Id == idTipoCategoria);
        var categorias = result
            .Where(c => c.UsuarioId == idUsuario 
            && (idTipoCategoria == 0 || c.TipoCategoriaId == idTipoCategoria)).ToList();

        return Mapper.Map<List<Dto>>(categorias);
    }
}