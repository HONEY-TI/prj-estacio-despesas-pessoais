namespace Application.Dtos;
public class SaldoDto
{
    public decimal saldo { get; set; }
    public static implicit operator decimal(SaldoDto d) => d.saldo;
    public static implicit operator SaldoDto(decimal saldo) => new SaldoDto(saldo);
    public SaldoDto(decimal saldo)
    {
        this.saldo = saldo;
    }
}