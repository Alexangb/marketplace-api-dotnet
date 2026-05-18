namespace MarketplaceApi.Shared.helpers;

/// <summary>
/// Helpers para manejo de fechas y horas
/// </summary>
public static class DateTimeHelper
{
    /// <summary>
    /// Zona horaria de Perú (UTC-5)
    /// </summary>
    private static readonly TimeZoneInfo PeruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
    
    /// <summary>
    /// Obtiene la fecha y hora actual en UTC
    /// </summary>
    public static DateTime UtcNow => DateTime.UtcNow;
    
    /// <summary>
    /// Obtiene la fecha y hora actual en hora de Perú
    /// </summary>
    public static DateTime PeruNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PeruTimeZone);
    
    /// <summary>
    /// Convierte una fecha UTC a hora de Perú
    /// </summary>
    public static DateTime ToPeruTime(this DateTime utcDateTime)
        => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, PeruTimeZone);
    
    /// <summary>
    /// Convierte una fecha local a UTC
    /// </summary>
    public static DateTime ToUtcFromPeru(this DateTime peruDateTime)
        => TimeZoneInfo.ConvertTimeToUtc(peruDateTime, PeruTimeZone);
    
    /// <summary>
    /// Valida si una fecha es hoy
    /// </summary>
    public static bool IsToday(this DateTime date)
        => date.Date == PeruNow.Date;
    
    /// <summary>
    /// Valida si una fecha es futura
    /// </summary>
    public static bool IsFuture(this DateTime date)
        => date.Date > PeruNow.Date;
    
    /// <summary>
    /// Valida si una fecha es pasada
    /// </summary>
    public static bool IsPast(this DateTime date)
        => date.Date < PeruNow.Date;
    
    /// <summary>
    /// Formatea una fecha para mostrar
    /// </summary>
    public static string ToFormattedDate(this DateTime date, string format = "dd/MM/yyyy")
        => date.ToString(format);
    
    /// <summary>
    /// Formatea una hora para mostrar
    /// </summary>
    public static string ToFormattedTime(this TimeOnly time, string format = "HH:mm")
        => time.ToString(format);
    
    /// <summary>
    /// Obtiene el rango de fechas para una semana específica
    /// </summary>
    public static (DateTime start, DateTime end) GetWeekRange(DateTime date)
    {
        var start = date.AddDays(-(int)date.DayOfWeek + 1); // Lunes
        var end = start.AddDays(6); // Domingo
        return (start, end);
    }
    
    /// <summary>
    /// Obtiene el rango de fechas para un mes específico
    /// </summary>
    public static (DateTime start, DateTime end) GetMonthRange(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        return (start, end);
    }
    
    /// <summary>
    /// Valida si dos horarios se solapan
    /// </summary>
    public static bool HasOverlap(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
        => start1 < end2 && start2 < end1;
    
    /// <summary>
    /// Calcula la duración en minutos entre dos horas
    /// </summary>
    public static int GetDurationInMinutes(TimeOnly start, TimeOnly end)
        => (int)(end - start).TotalMinutes;
    
    /// <summary>
    /// Agrega minutos a una hora
    /// </summary>
    public static TimeOnly AddMinutes(this TimeOnly time, int minutes)
        => time.AddMinutes(minutes);
    
    /// <summary>
    /// Formatea un TimeSpan a string legible
    /// </summary>
    public static string FormatDuration(int minutes)
    {
        var hours = minutes / 60;
        var remainingMinutes = minutes % 60;
        
        if (hours > 0 && remainingMinutes > 0)
            return $"{hours}h {remainingMinutes}min";
        if (hours > 0)
            return $"{hours}h";
        return $"{minutes}min";
    }
}