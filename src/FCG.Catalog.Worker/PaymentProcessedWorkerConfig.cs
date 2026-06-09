namespace FCG.Catalog.Worker;

public class PaymentProcessedWorkerConfig
{
    public bool Ativo { get; set; } = true;
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}
