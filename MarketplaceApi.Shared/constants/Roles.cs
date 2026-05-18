namespace MarketplaceApi.Shared.constants;

/// <summary>
/// Roles de usuario en el sistema
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Prestador = "Prestador";
    public const string Cliente = "Cliente";
    
    /// <summary>
    /// Todos los roles válidos
    /// </summary>
    public static readonly string[] AllRoles = { Admin, Prestador, Cliente };
    
    /// <summary>
    /// Valida si un rol es válido
    /// </summary>
    public static bool IsValidRole(string role)
        => AllRoles.Contains(role);
    
    /// <summary>
    /// Roles que pueden administrar contenido
    /// </summary>
    public static readonly string[] AdminRoles = { Admin, Prestador };
}