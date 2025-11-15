using AutoMapper;
using Application.Abstractions;
using Application.Dtos;
using GlobalException.CustomExceptions;
using GlobalException.CustomExceptions.Core;
using Repository.UnitOfWork.Abstractions;
using Domain.Core.ValueObject;
using Domain.Entities;
using Repository.Persistency.Generic;

namespace Application.Implementations;
public class ReceitaBusinessImpl<Dto> : BusinessBase<Dto, Receita> where Dto : ReceitaDto, new()
{
    private readonly IRepositorio<Receita> _repositorio;
    private readonly IUnitOfWork<Categoria> _unitOfWorkCategoria;
    private readonly IMapper _mapper;
    public ReceitaBusinessImpl(IMapper mapper, IRepositorio<Receita> repositorio, IUnitOfWork<Receita> unitOfWork, IUnitOfWork<Categoria> unitOfWorkCategoria = null) : base(mapper, repositorio, unitOfWork)
    {
        _mapper = mapper;
        _repositorio = repositorio;
        _unitOfWorkCategoria = unitOfWorkCategoria;
    }

    private async Task IsValidReceita(Receita dto)
    {
        var receita = await UnitOfWork!.Repository.Get(dto.Id);
        if (receita == null || receita.UsuarioId != dto.UsuarioId)
            throw new ReceitaUsuarioInvalidaException();
    }

    private async Task IsValidCategoria(Receita dto)
    {
        var categoria = await _unitOfWorkCategoria.Repository.Get(dto.CategoriaId);
        if (categoria == null
            || categoria.UsuarioId != dto.UsuarioId
            || categoria == null
            || categoria.TipoCategoria != TipoCategoria.CategoriaType.Receita
            || categoria.Id != dto.CategoriaId)
            throw new CategoriaUsuarioInvalidaException();
    }

    public override async Task<Dto> Create(Dto dto)
    {
        try
        {
            var receita = _mapper.Map<Receita>(dto);
            await IsValidCategoria(receita);
            await UnitOfWork.Repository.Insert(receita);
            await UnitOfWork.CommitAsync();
            receita = await UnitOfWork.Repository.Get(receita.Id)
                ?? throw new CustomException("Não foi possível realizar o cadastro da receita.");
            return _mapper.Map<Dto>(receita);
        }
        catch
        {
            throw;
        }
    }

    public override async Task<List<Dto>> FindAll(Guid idUsuario)
    {
        var result = await UnitOfWork.Repository.Find(repo => repo.UsuarioId == idUsuario);
        var receitas = result.ToList();
        return _mapper.Map<List<Dto>>(receitas);
    }

    public override async Task<Dto> FindById(Guid id, Guid idUsuario)
    {
        var receita = await UnitOfWork.Repository.Get(id);
        if (receita is null) return null;
        receita.UsuarioId = idUsuario;
        await IsValidReceita(receita);
        var receitaDto = _mapper.Map<Dto>(receita);
        return receitaDto;
    }

    public override async Task<Dto> Update(Dto dto)
    {
        try
        {
            var receita = _mapper.Map<Receita>(dto);
            await IsValidReceita(receita);
            await IsValidCategoria(receita);
            await UnitOfWork.Repository.Update(receita);
            await UnitOfWork.CommitAsync();
            receita = await UnitOfWork.Repository.Get(dto.Id.Value)
                ?? throw new CustomException("Não foi possível atualizar o cadastro da receita.");
            return _mapper.Map<Dto>(receita);
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
            Receita receita = _mapper.Map<Receita>(dto);
            await IsValidReceita(receita);
            await UnitOfWork.Repository.Delete(receita.Id);
            await UnitOfWork.CommitAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }  
}
