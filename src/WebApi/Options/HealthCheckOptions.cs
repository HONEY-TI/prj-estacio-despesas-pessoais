namespace WebApi.Options;
public sealed class HealthCheckItem
{
    public string? EndPoint { get; set; }
    public string? Name { get; set; }
    public int? Port { get; set; }
    public string? Database { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
}

public class HealthCheckOptions
{
    public string? EndPoint { get; set; }

    public class HealthCheckConfig
    {
        public IList<HealthCheckItem> Endpoints { get; set; } = new List<HealthCheckItem>();
    }
}