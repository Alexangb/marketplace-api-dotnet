namespace MarketplaceApi.Shared.constants;

/// <summary>
/// Mensajes de error estandarizados
/// </summary>
public static class MensajesError
{
    // Errores generales
    public const string ErrorGenerico = "Ha ocurrido un error inesperado";
    public const string ErrorValidacion = "Error de validación";
    public const string ErrorBaseDatos = "Error en la base de datos";
    
    // Errores de entidades
    public const string ServicioNoEncontrado = "El servicio no existe";
    public const string CategoriaNoEncontrada = "La categoría no existe";
    public const string UsuarioNoEncontrado = "El usuario no existe";
    public const string ReservaNoEncontrada = "La reserva no existe";
    public const string HorarioNoEncontrado = "El horario no existe";
    
    // Errores de negocio
    public const string ServicioYaExiste = "Ya existe un servicio con ese nombre";
    public const string CategoriaYaExiste = "Ya existe una categoría con ese nombre";
    public const string EmailYaRegistrado = "El email ya está registrado";
    public const string HorarioDuplicado = "Ya existe un horario configurado para ese día";
    public const string HorarioInvalido = "La hora de inicio debe ser menor a la hora de fin";
    public const string HorarioFueraRango = "El horario debe estar dentro del rango permitido (00:00 - 23:59)";
    
    // Errores de reservas
    public const string ReservaHorarioNoDisponible = "El horario seleccionado no está disponible";
    public const string ReservaFechaInvalida = "La fecha de reserva no puede ser en el pasado";
    public const string ReservaDuplicada = "Ya existe una reserva en ese horario";
    public const string ReservaCancelacionInvalida = "No se puede cancelar esta reserva";
    public const string ReservaConfirmacionInvalida = "No se puede confirmar esta reserva";
    
    // Errores de autenticación
    public const string CredencialesInvalidas = "Email o contraseña incorrectos";
    public const string NoAutorizado = "No tienes autorización para realizar esta acción";
    public const string TokenInvalido = "El token es inválido o ha expirado";
    public const string SesionExpirada = "La sesión ha expirado";
    
    // Errores de validación
    public const string CampoRequerido = "El campo {0} es requerido";
    public const string LongitudInvalida = "El campo {0} debe tener entre {1} y {2} caracteres";
    public const string EmailInvalido = "El formato del email es inválido";
    public const string PasswordInsegura = "La contraseña debe tener al menos 6 caracteres, una mayúscula, una minúscula y un número";
    public const string PrecioInvalido = "El precio debe ser mayor a 0";
    public const string DuracionInvalida = "La duración debe ser mayor a 0 minutos";
    
    // Errores de archivos
    public const string ArchivoNoEncontrado = "No se encontró el archivo";
    public const string ArchivoInvalido = "El formato del archivo no es válido";
    public const string ArchivoDemasiadoGrande = "El archivo excede el tamaño máximo permitido";
}