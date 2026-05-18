using Microsoft.AspNetCore.Http;

public class SubirImagenDto
{
    public IFormFile Archivo { get; set; } = null!;
}

