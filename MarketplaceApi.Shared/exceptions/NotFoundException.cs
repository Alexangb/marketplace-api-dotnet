namespace MarketplaceApi.Shared.exceptions;

/// <summary>
/// Excepción para recursos no encontrados
/// </summary>
public class NotFoundException : Exception
{
    public string EntityName { get; set; }
    public object? EntityId { get; set; }
    
    public NotFoundException() : base()
    {
        EntityName = string.Empty;
    }
    
    public NotFoundException(string message) : base(message)
    {
        EntityName = string.Empty;
    }
    
    public NotFoundException(string entityName, object entityId) 
        : base($"No se encontró {entityName} con ID: {entityId}")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
    
    public NotFoundException(string entityName, object entityId, Exception innerException) 
        : base($"No se encontró {entityName} con ID: {entityId}", innerException)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
    
    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
        EntityName = string.Empty;
    }
}