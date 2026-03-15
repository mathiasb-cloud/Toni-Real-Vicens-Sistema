namespace Toni_Real_Vicens_Sistema.Models
{
    public class Usuario
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string SegundoApellido { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty; 
        public bool IsActivo { get; set; } = true; 
    }
}