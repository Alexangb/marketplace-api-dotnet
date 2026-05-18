using System.Text;
using System.Text.RegularExpressions;

namespace MarketplaceApi.Shared.helpers;

/// <summary>
/// Helpers para manejo de strings
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Valida si un string es null o está vacío
    /// </summary>
    public static bool IsNullOrEmpty(this string? value, TimeOnly? hora)
        => string.IsNullOrEmpty(value);
    
    /// <summary>
    /// Valida si un string es null, vacío o solo espacios
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value);
    
    /// <summary>
    /// Trunca un string a una longitud máxima
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;
        
        return value.Substring(0, maxLength) + suffix;
    }
    
    /// <summary>
    /// Convierte un string a slug (para URLs)
    /// </summary>
    public static string ToSlug(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        
        // Normalizar caracteres
        var normalized = value.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        
        foreach (var c in normalized)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }
        
        // Convertir a minúsculas y reemplazar espacios
        var slug = stringBuilder.ToString()
            .ToLowerInvariant()
            .Trim()
            .Replace(" ", "-")
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("ñ", "n");
        
        // Eliminar caracteres no válidos
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        
        // Eliminar guiones duplicados
        slug = Regex.Replace(slug, @"\-+", "-");
        
        return slug.Trim('-');
    }
    
    /// <summary>
    /// Capitaliza la primera letra de cada palabra
    /// </summary>
    public static string CapitalizeWords(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;
        
        var words = value.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
            {
                var word = words[i];
                words[i] = char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }
        }
        
        return string.Join(" ", words);
    }
    
    /// <summary>
    /// Elimina espacios en blanco al inicio y final
    /// </summary>
    public static string? TrimSafe(this string? value)
        => value?.Trim();
    
    /// <summary>
    /// Valida si un email tiene formato correcto
    /// </summary>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Enmascara un email (ej: u***@domain.com)
    /// </summary>
    public static string MaskEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return email;
        
        var atIndex = email.IndexOf('@');
        if (atIndex <= 2)
            return email;
        
        var username = email[..atIndex];
        var domain = email[atIndex..];
        
        var maskedUsername = username.Length > 2 
            ? username[0] + new string('*', username.Length - 2) + username[^1]
            : username;
        
        return maskedUsername + domain;
    }
    
    /// <summary>
    /// Enmascara un número de teléfono
    /// </summary>
    public static string MaskPhone(this string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 6)
            return phone;
        
        var lastDigits = phone[^4..];
        return new string('*', phone.Length - 4) + lastDigits;
    }
    
    /// <summary>
    /// Genera un código aleatorio de longitud específica
    /// </summary>
    public static string GenerateRandomCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    /// <summary>
    /// Convierte una lista de strings a una cadena separada por comas
    /// </summary>
    public static string JoinStrings(IEnumerable<string> strings, string separator = ", ")
        => string.Join(separator, strings);
    
    /// <summary>
    /// Limpia caracteres especiales de un string
    /// </summary>
    public static string RemoveSpecialCharacters(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;
        
        return Regex.Replace(value, @"[^a-zA-Z0-9\s]", "");
    }
    
    /// <summary>
    /// Valida si un string contiene solo letras y espacios
    /// </summary>
    public static bool IsOnlyLettersAndSpaces(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
        
        return Regex.IsMatch(value, @"^[a-zA-ZáéíóúñÁÉÍÓÚÑ\s]+$");
    }
    
    /// <summary>
    /// Valida si un string es un número telefónico válido (Perú)
    /// </summary>
    public static bool IsValidPhoneNumber(this string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;
        
        // Limpiar formato
        var cleaned = phone.Replace(" ", "").Replace("-", "").Replace("+", "");
        
        // Validar formato: 9 dígitos para celular, o 9 con código de país
        return Regex.IsMatch(cleaned, @"^(9\d{8}|519\d{8})$");
    }
}