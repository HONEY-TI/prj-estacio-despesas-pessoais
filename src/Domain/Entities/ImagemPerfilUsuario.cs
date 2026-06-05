using Domain.Core.Aggreggates;

namespace Domain.Entities;
public class ImagemPerfilUsuario : BaseDomain
{
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; } = new();
}