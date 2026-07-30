using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FCG.Catalog.Infrastructure.Data.Mongo;

public class AvaliacaoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid JogoId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    public int Nota { get; set; }

    public string Comentario { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}
