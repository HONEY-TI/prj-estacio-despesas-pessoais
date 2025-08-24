using Domain.Core.Aggreggates;
using Domain.Core.ValueObject;
using System.Text.Json.Serialization;

namespace Domain.Entities;
public class Categoria : BaseDomain
{
    public string Descricao { get; set; } = String.Empty;
    public Guid UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; }
    public virtual TipoCategoria? TipoCategoria { get; set; }
    public Categoria() { }

    public Categoria(string descricao, Guid usuarioId, Usuario usuario, TipoCategoria tipoCategoria)
    {
        Descricao = descricao;
        UsuarioId = usuarioId;
        Usuario = usuario;
        TipoCategoria = tipoCategoria;
    }

    [JsonConstructor]
    public Categoria(Guid id, string descricao, Guid usuarioId, Usuario usuario, TipoCategoria tipoCategoria)
    {
        Id = id;
        Descricao = descricao;
        UsuarioId = usuarioId;
        Usuario = usuario;
        TipoCategoria = tipoCategoria;
    }
}