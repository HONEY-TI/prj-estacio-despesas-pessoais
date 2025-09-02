using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos;
using Despesas.Repository.UnitOfWork.Abstractions;
using Domain.Core.ValueObject;
using Domain.Entities;
using Repository.Persistency.Generic;

namespace Despesas.Application.Implementations;
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

    public override async Task<Dto> Create(Dto dto)
    {
        var despesa = _mapper.Map<Despesa>(dto);
        await IsValidCategoria(despesa);
        await UnitOfWork.Repository.Insert(despesa);
        await UnitOfWork.CommitAsync();
        despesa = await UnitOfWork.Repository.Get(despesa.Id);
        return _mapper.Map<Dto>(despesa);
    }

    public override async Task<List<Dto>> FindAll(Guid idUsuario)
    {
        var result = await UnitOfWork.Repository.Find(repo => repo.UsuarioId == idUsuario);
        var despesas = result.ToList();
        var dtos = _mapper.Map<List<Dto>>(despesas);
        return dtos;
    }

    public override async Task<Dto> FindById(Guid id, Guid idUsuario)
    {
        var despesa = await UnitOfWork.Repository.Get(id);
        if (despesa is null) return null;
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
        despesa = await UnitOfWork.Repository.Get(dto.Id.Value);
        return _mapper.Map<Dto>(despesa);
    }

    public override async Task<bool> Delete(Dto dto)
    {
        Despesa despesa = _mapper.Map<Despesa>(dto);
        await IsValidDespesa(despesa);
        await UnitOfWork.Repository.Delete(despesa.Id);
        await UnitOfWork.CommitAsync();
        return true;
    }

    private async Task IsValidCategoria(Despesa dto)
    {
        var categoria = await _unitOfWorkCategoria.Repository.Get(dto.CategoriaId);
        if (categoria == null 
            || categoria.UsuarioId != dto.UsuarioId 
            || categoria == null 
            || categoria.TipoCategoria != TipoCategoria.CategoriaType.Despesa 
            || categoria.Id != dto.CategoriaId) 
            throw new ArgumentException("Categoria inválida para este usuário!");
    }

    private async Task IsValidDespesa(Despesa dto)
    {
        var despesa = await UnitOfWork!.Repository.Get(dto.Id);
        if (despesa == null || despesa.UsuarioId != dto.UsuarioId)
            throw new ArgumentException("Despesa inválida para este usuário!");
    }
}
