using AutoMapper;
using Despesas.Application.Abstractions;
using Despesas.Application.Dtos.Abstractions;
using Domain.Entities;
using Domain.Entities.ValueObjects;
using Repository.Persistency.Generic;
using Repository.Persistency.UnitOfWork.Abstractions;

namespace Despesas.Application.Implementations;
public class UsuarioBusinessImpl<Dto> : BusinessBase<Dto, Usuario>, IUsuarioBusiness<Dto> where Dto : BaseUsuarioDto, new()
{
    private readonly IRepositorio<Usuario> _repositorio;
    private readonly IMapper _mapper;

    public UsuarioBusinessImpl(IMapper mapper, IRepositorio<Usuario> repositorio, IUnitOfWork<Usuario>? unitOfWork = null) : base(mapper, repositorio, unitOfWork)
    {
        _mapper = mapper;
        _repositorio = repositorio;
    }

    public override Dto Create(Dto dto)
    {
        IsValidPrefilAdministratdor(dto);
        var usuario = _mapper.Map<Usuario>(dto);
        usuario = usuario.CreateUsuario(usuario);
        _repositorio.Insert(usuario);
        return _mapper.Map<Dto>(usuario);
    }

    public override List<Dto> FindAll(Guid idUsuario)
    {
        var usuario = _repositorio?.Find(u => u.Id == idUsuario)?.FirstOrDefault();
        IsValidPrefilAdministratdor(usuario);
        return _mapper.Map<List<Dto>>(_repositorio?.GetAll());
    }

    public override Dto Update(Dto dto)
    {
        var usuario = _mapper.Map<Usuario>(dto);
        _repositorio.Update(usuario);
        if (usuario is null) throw new ArgumentException("Usuário não encontrado!");
        return _mapper.Map<Dto>(usuario);
    }

    public override Dto FindById(Guid id)
    {
        var usuario = _repositorio?.Find(u => u.Id == id)?.FirstOrDefault();
        return this.Mapper.Map<Dto>(usuario);
    }

    public override bool Delete(Dto dto)
    {
        IsValidPrefilAdministratdor(dto);
        var usuario = _mapper.Map<Usuario>(dto);
        return _repositorio.Delete(usuario);
    }

    private void IsValidPrefilAdministratdor(Dto dto)
    {
        var adm = _repositorio.Get(dto.UsuarioId);
        if (adm.PerfilUsuario != PerfilUsuario.Perfil.Admin)
            throw new ArgumentException("Usuário não permitido a realizar operação!");
    }

    private void IsValidPrefilAdministratdor(Usuario usuario)
    {
        var adm = _repositorio.Get(usuario.Id);
        if (adm.PerfilUsuario != PerfilUsuario.Perfil.Admin)
            throw new ArgumentException("Usuário não permitido a realizar operação!");
    }
}