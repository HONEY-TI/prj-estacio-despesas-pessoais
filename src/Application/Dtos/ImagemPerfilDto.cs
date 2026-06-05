using Application.Dtos.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos;
public class ImagemPerfilDto : BaseDto
{
    [Url(ErrorMessage = "Url inválida.")]
    public string? Url { get; set; }

    [JsonIgnore]
    public string? Name { get; set; }
    [JsonIgnore]
    public string? Type { get; set; }
    [JsonIgnore]
    public string? ContentType { get; set; }

    [JsonIgnore]
    public byte[]? Arquivo { get; set; }    
}