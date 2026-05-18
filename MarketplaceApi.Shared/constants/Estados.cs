namespace MarketplaceApi.Shared.constants;

/// <summary>
/// Estados posibles para una reserva
/// </summary>
public static class EstadosReserva
{
    public const string Pendiente = "Pendiente";
    public const string Confirmada = "Confirmada";
    public const string Cancelada = "Cancelada";
    public const string Completada = "Completada";
    
    /// <summary>
    /// Todos los estados válidos
    /// </summary>
    public static readonly string[] AllStates = { Pendiente, Confirmada, Cancelada, Completada };
    
    /// <summary>
    /// Estados que permiten modificación
    /// </summary>
    public static readonly string[] Modificables = { Pendiente, Confirmada };
    
    /// <summary>
    /// Valida si un estado es válido
    /// </summary>
    public static bool IsValidState(string state)
        => AllStates.Contains(state);
    
    /// <summary>
    /// Valida si se puede cancelar la reserva
    /// </summary>
    public static bool CanCancel(string state)
        => state == Pendiente || state == Confirmada;
    
    /// <summary>
    /// Valida si se puede completar la reserva
    /// </summary>
    public static bool CanComplete(string state)
        => state == Confirmada;
}

/// <summary>
/// Estados posibles para un servicio
/// </summary>
public static class EstadosServicio
{
    public const string Activo = "Activo";
    public const string Inactivo = "Inactivo";
    public const string Eliminado = "Eliminado";
    
    public static readonly string[] AllStates = { Activo, Inactivo, Eliminado };
    
    public static bool IsActive(string state) => state == Activo;
}

/// <summary>
/// Días de la semana
/// </summary>
public static class DiasSemana
{
    public const int Lunes = 1;
    public const int Martes = 2;
    public const int Miercoles = 3;
    public const int Jueves = 4;
    public const int Viernes = 5;
    public const int Sabado = 6;
    public const int Domingo = 7;
    
    public static readonly int[] Todos = { Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo };
    public static readonly int[] Laborables = { Lunes, Martes, Miercoles, Jueves, Viernes };
    public static readonly int[] Finde = { Sabado, Domingo };
    
    public static string GetNombreDia(int dia)
    {
        return dia switch
        {
            Lunes => "Lunes",
            Martes => "Martes",
            Miercoles => "Miércoles",
            Jueves => "Jueves",
            Viernes => "Viernes",
            Sabado => "Sábado",
            Domingo => "Domingo",
            _ => throw new ArgumentException($"Día inválido: {dia}")
        };
    }
    
    public static bool IsValidDay(int dia)
        => dia >= 1 && dia <= 7;
}