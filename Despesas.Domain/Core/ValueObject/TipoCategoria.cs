namespace Domain.Core.ValueObject;
public record TipoCategoria
{
    public static implicit operator CategoriaType(TipoCategoria tc) => (CategoriaType)tc.Id;
    public static implicit operator TipoCategoria(int tipoCategoria) => new TipoCategoria((int)tipoCategoria);
    public static bool operator ==(TipoCategoria tipoCategoria, CategoriaType tipoCategoriaType) => tipoCategoria?.Id == (int)tipoCategoriaType;
    public static bool operator !=(TipoCategoria tipoCategoria, CategoriaType tipoCategoriaType) => !(tipoCategoria?.Id == (int)tipoCategoriaType);

    public enum CategoriaType
    {
        Invalid = 0,
        Despesa = 1,
        Receita = 2
    }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public TipoCategoria() { }

    public TipoCategoria(int id)
    {
        Id = id;
        Name = GetTipoCategoriaName((int)id);
    }

    public TipoCategoria(CategoriaType tipoCategoria)
    {
        Id = (int)tipoCategoria;
        Name = GetTipoCategoriaName((int)tipoCategoria);
    }

    private static string GetTipoCategoriaName(int tipoCategoria)
    {
        if ((int)CategoriaType.Despesa == tipoCategoria)
            return "Despesa";
        else if ((int)CategoriaType.Receita == tipoCategoria)
            return "Receita";
        else if ((int)CategoriaType.Invalid == tipoCategoria)
            return "Invalid";


        throw new ArgumentException("Tipo de Categoria inexistente!");
    }
}

