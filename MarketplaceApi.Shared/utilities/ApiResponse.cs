namespace MarketplaceApi.Shared.utilities;

/// <summary>
/// Respuesta estándar para toda la API
/// </summary>
public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    List<string>? Errors = null
)
{
    // Factory methods para respuestas comunes
    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
        => new(true, message, data, null);
    
    public static ApiResponse<T> Created(T data, string message = "Recurso creado exitosamente")
        => new(true, message, data, null);
    
    public static ApiResponse<T> Error(string message, List<string>? errors = null)
        => new(false, message, default, errors);
    
    public static ApiResponse<T> NotFound(string message = "Recurso no encontrado")
        => new(false, message, default, null);
    
    public static ApiResponse<T> BadRequest(string message, List<string>? errors = null)
        => new(false, message, default, errors);
    
    public static ApiResponse<T> Unauthorized(string message = "No autorizado")
        => new(false, message, default, null);
    
    public static ApiResponse<T> Forbidden(string message = "Acceso denegado")
        => new(false, message, default, null);
}

// Para respuestas sin datos
public record ApiResponse(
    bool Success,
    string Message,
    List<string>? Errors = null
)
{
    public static ApiResponse Ok(string message = "Operación exitosa")
        => new(true, message, null);
    
    public static ApiResponse Error(string message, List<string>? errors = null)
        => new(false, message, errors);
    
    public static ApiResponse NotFound(string message = "Recurso no encontrado")
        => new(false, message, null);
}