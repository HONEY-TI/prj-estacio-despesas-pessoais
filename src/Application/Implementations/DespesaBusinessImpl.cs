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
public class DespesaBusinessImpl<Dto> : BusinessBase<Dto, Despesa>, IBusinessBase<Dto, Despesa> where Dto : DespesaDto, new()
{
    private readonly IRepositorio<Despesa> _repositorio;
    private readonly IUnitOfWork<Categoria> _unitOfWorkCategoria;
    private readonly IMapper _mapper;

    public DespesaBusinessImpl(IMapper mapper, IRepositorio<Despesa> repositorio, IUnitOfWork<Despesa> unitOfWork, IUnitOfWork<Categoria> unitOfWorkCategoria = null) : base(mapper, repositorio, unitOfWork)
    {
        _repositorio = repositorio;
        _mapper = mapper;
        _unitOfWorkCategoria =  unitOfWorkCategoria;
    }

    private async Task IsValidDespesa(Despesa dto)
    {
        var despesa = await UnitOfWork!.Repository.Get(dto.Id);
        if (despesa == null || despesa.UsuarioId != dto.UsuarioId)
            throw new DespesaUsuarioInvalidaException();
    }

    private async Task IsValidCategoria(Despesa dto)
    {
        var categoria = await _unitOfWorkCategoria.Repository.Get(dto.CategoriaId);
        if (categoria == null
            || categoria.UsuarioId != dto.UsuarioId
            || categoria == null
            || categoria.TipoCategoria != TipoCategoria.CategoriaType.Despesa
            || categoria.Id != dto.CategoriaId)
            throw new CategoriaUsuarioInvalidaException();
    }

    public override async Task<Dto> Create(Dto dto)
    {
        var despesa = _mapper.Map<Despesa>(dto);
        await IsValidCategoria(despesa);
        await UnitOfWork.Repository.Insert(despesa);
        await UnitOfWork.CommitAsync();
        despesa = await UnitOfWork.Repository.Get(despesa.Id);
        if (despesa is null)
            throw new CustomException("Não foi possível realizar o cadastro da despesa.");
        return _mapper.Map<Dto>(despesa);
    }

    public override async Task<List<Dto>> FindAll(Guid idUsuario)
    {
        var result = await UnitOfWork.Repository.Find(repo => repo.UsuarioId == idUsuario && repo.Categoria.TipoCategoria.Id == (int)TipoCategoria.CategoriaType.Despesa);
        var despesas = result.ToList();
        var dtos = _mapper.Map<List<Dto>>(despesas);
        return dtos;
    }

    public override async Task<Dto> FindById(Guid id, Guid idUsuario)
    {
        var result = await UnitOfWork.Repository.Find(d => d.Id == id && d.UsuarioId == idUsuario);
        if (result is null) return null;
        var despesa = result.FirstOrDefault();        
        despesa.UsuarioId = idUsuario;
        await IsValidDespesa(despesa);
        var despesaDto = _mapper.Map<Dto>(despesa);
        return despesaDto;
    }

    public override async Task<Dto> Update(Dto dto)
    {
        var despesa = _mapper.Map<Despesa>(dto);
        await IsValidDespesa(despesa);
        await IsValidCategoria(despesa);
        await UnitOfWork.Repository.Update(despesa);
        await UnitOfWork.CommitAsync();
        despesa = await UnitOfWork.Repository.Get(dto.Id.Value) 
            ?? throw new CustomException("Não foi possível atualizar o cadastro da despesa.");
        return _mapper.Map<Dto>(despesa);
    }

    public override async Task<bool> Delete(Dto dto)
    {
        var result = await UnitOfWork.Repository.Find(d => d.Id == dto.Id && d.UsuarioId == dto.UsuarioId);
        if (result is null)
            throw new UsuarioNaoAutorizadoException();
        var despesa = result.FirstOrDefault();
        await IsValidDespesa(despesa);
        await UnitOfWork.Repository.Delete(despesa.Id);
        await UnitOfWork.CommitAsync();
        return true;
    }
}