using Azure;
using Azure.Data.Tables;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contoso.MCP.Order.Models;

public record Order : ITableEntity
{
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set;  }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; } = ETag.All;

    // Internal property for Table Storage
    public string? ItemsIds { get; set; }

    // Public property for your application logic (not stored in Table Storage)
    [IgnoreDataMember]
    [JsonIgnore]
    public List<string> Items
    {
        get => string.IsNullOrEmpty(ItemsIds)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(ItemsIds) ?? new List<string>();
        set => ItemsIds = JsonSerializer.Serialize(value);
    }
};
