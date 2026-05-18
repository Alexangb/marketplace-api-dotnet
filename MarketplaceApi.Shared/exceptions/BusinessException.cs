namespace MarketplaceApi.Shared.exceptions;

/// <summary>
/// Excepción para errores de lógica de negocio
/// </summary>
public class BusinessException : Exception
{
    public string? ErrorCode { get; set; }
    
    public BusinessException() : base()
    {
    }
    
    public BusinessException(string message) : base(message)
    {
    }
    
    public BusinessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
    public BusinessException(string message, string errorCode) 
        : base(message)
    {
        ErrorCode = errorCode;
    }
    
    public BusinessException(string message, string errorCode, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}