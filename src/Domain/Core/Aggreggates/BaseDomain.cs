namespace Domain.Core.Aggreggates;
public abstract class BaseDomain
{
    public virtual Guid Id { get; set; } = Guid.NewGuid();
}