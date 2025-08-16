using Despesas.Business.Dtos.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Despesas.Business.Dtos;
public class ImagemPerfilDto : BaseImagemPerfilDto
{
    [Url(ErrorMessage = "Url inválida.")]
    public override string? Url { get; set; }

    [JsonIgnore]
    public override string? Name { get; set; }
    [JsonIgnore]
    public override string? Type { get; set; }
    [JsonIgnore]
    public override string? ContentType { get; set; }

    [JsonIgnore]
    public override byte[]? Arquivo { get; set; }    
}