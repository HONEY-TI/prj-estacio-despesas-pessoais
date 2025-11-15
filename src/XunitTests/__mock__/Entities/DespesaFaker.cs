using Bogus;
using Application.Dtos;
using Application.Dtos.Abstractions;
using Application.Dtos.Core;
using Domain.Core.ValueObject;

namespace __mock__.Entities;
public sealed class DespesaFaker
{
    static int counter = 1;
    static int counterVM = 1;

    private static DespesaFaker? _instance;
    private static readonly object LockObject = new object();
    public static DespesaFaker Instance
    {
        get
        {
            lock (LockObject)
            {
                return _instance ??= new DespesaFaker();
            }
        }
    }

    public Despesa GetNewFaker(Usuario usuario, Categoria categoria)
    {

        var despesaFaker = new Faker<Despesa>()
        .RuleFor(r => r.Id, f => Guid.NewGuid())
        .RuleFor(r => r.Data, new DateTime(DateTime.Now.Year, new Random().Next(1, 13), 1))
        .RuleFor(
            r => r.DataVencimento,
            new DateTime(DateTime.Now.Year, new Random().Next(1, 13), 1)
        )
        .RuleFor(r => r.Descricao, f => f.Commerce.ProductName())
        .RuleFor(r => r.Valor, f => f.Random.Decimal(1, 900000))
        .RuleFor(r => r.UsuarioId, usuario.Id)
        .RuleFor(r => r.Usuario, usuario)
        .RuleFor(r => r.Categoria, CategoriaFaker.Instance.GetNewFaker(usuario, (int)TipoCategoria.CategoriaType.Despesa, usuario.Id))
        .Generate();
        despesaFaker.Categoria = despesaFaker.Categoria ?? new();
        despesaFaker.CategoriaId = despesaFaker.Categoria.Id;
        counter++;
        return despesaFaker;

    }

    public DespesaDto GetNewFakerVM(Guid idUsuario, Guid idCategoria)
    {

        var despesaFaker = new Faker<DespesaDto>()
        .RuleFor(r => r.Id, f => Guid.NewGuid())
        .RuleFor(r => r.Data, new DateTime(DateTime.Now.Year, new Random().Next(1, 13), 1))
        .RuleFor(
            r => r.DataVencimento,
            new DateTime(DateTime.Now.Year, new Random().Next(1, 13), 1)
        )
        .RuleFor(r => r.Descricao, f => f.Commerce.ProductName())
        .RuleFor(r => r.Valor, f => f.Random.Decimal(1, 900000))
        .RuleFor(r => r.UsuarioId, idUsuario)
        .RuleFor(r => r.CategoriaId, CategoriaFaker.Instance.GetNewFakerVM(UsuarioFaker.Instance.GetNewFakerVM(idUsuario), TipoCategoriaDto.Despesa, idUsuario).Id)
        .Generate();
        counterVM++;
        return despesaFaker;
    }

    public List<DespesaDto> DespesasVMs(UsuarioDto? usuarioDto = null, Guid? idUsuario = null)
    {
        var listDespesaDto = new List<DespesaDto>();
        for (int i = 0; i < 10; i++)
        {
            if (idUsuario == null)
                usuarioDto = UsuarioFaker.Instance.GetNewFakerVM(Guid.NewGuid());

            usuarioDto = usuarioDto ?? new UsuarioDto();
            var categoriaDto = CategoriaFaker.Instance.GetNewFakerVM(usuarioDto, TipoCategoriaDto.Despesa);

            var despesaDto = GetNewFakerVM(usuarioDto.Id.Value, categoriaDto.Id.Value);
            listDespesaDto.Add(despesaDto);
        }

        return listDespesaDto;
    }

    public List<Despesa> Despesas(Usuario? usuario = null, int? idUsurio = null, int count = 10)
    {
        var listDespesa = new List<Despesa>();
        for (int i = 0; i < count; i++)
        {
            if (idUsurio == null)
                usuario = UsuarioFaker.Instance.GetNewFaker(Guid.NewGuid());

            usuario = usuario ?? new();
            var categoria = CategoriaFaker.Instance.GetNewFaker(usuario, (int)TipoCategoria.CategoriaType.Despesa, usuario.Id);
            var despesa = GetNewFaker(usuario, categoria);
            listDespesa.Add(despesa);
        }
        return listDespesa;
    }
}
