namespace FCG.Catalog.Application.Messaging;

public class OrderPlacedPublisherConfig
{
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}
