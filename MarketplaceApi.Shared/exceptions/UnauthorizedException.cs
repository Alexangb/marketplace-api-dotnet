namespace MarketplaceApi.Shared.exceptions;

/// <summary>
/// Excepción para errores de autorización
/// </summary>
public class UnauthorizedException : Exception
{
    public string? RequiredRole { get; set; }
    public string? CurrentUser { get; set; }
    
    public UnauthorizedException() : base()
    {
    }
    
    public UnauthorizedException(string message) : base(message)
    {
    }
    
    public UnauthorizedException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
    public UnauthorizedException(string message, string requiredRole) 
        : base(message)
    {
        RequiredRole = requiredRole;
    }
    
    public UnauthorizedException(string message, string requiredRole, string currentUser) 
        : base(message)
    {
        RequiredRole = requiredRole;
        CurrentUser = currentUser;
    }
}

/// <summary>
/// Excepción para errores de validación
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; set; }
    
    public ValidationException() : base("Error de validación")
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Error de validación")
    {
        Errors = errors;
    }
    
    public ValidationException(string message, Dictionary<string, string[]> errors) 
        : base(message)
    {
        Errors = errors;
    }
}