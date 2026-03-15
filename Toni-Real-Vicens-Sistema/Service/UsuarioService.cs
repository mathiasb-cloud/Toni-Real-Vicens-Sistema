using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class UsuarioService : FirebaseService
    {
        public UsuarioService(IConfiguration config) : base(config) { }

        // MÉTODO QUE FALTABA
        public async Task<Usuario?> LoginAsync(string correo, string pass)
        {
            var usuarios = await GetAllAsync();
            // Buscamos el usuario que coincida con correo y contraseña
            return usuarios.FirstOrDefault(u => u.Correo == correo && u.Contrasena == pass);
        }

        public async Task<bool> AddAsync(Usuario usuario)
        {
            // Validar si el correo o el teléfono ya existen para evitar duplicados
            var existentes = await GetAllAsync();
            if (existentes.Any(u => u.Telefono == usuario.Telefono || u.Correo == usuario.Correo))
            {
                return false;
            }

            await _firebase.Child("Usuarios").PostAsync(usuario);
            return true;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            var data = await _firebase.Child("Usuarios").OnceAsync<Usuario>();
            return data.Select(x => {
                x.Object.Id = x.Key;
                return x.Object;
            }).ToList();
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            var usuarios = await GetAllAsync();
            return usuarios.FirstOrDefault(u => u.Id == id);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            
            await _firebase.Child("Usuarios").Child(usuario.Id).PutAsync(usuario);
        }

        public async Task DeleteAsync(string id)
        {
            await _firebase.Child("Usuarios").Child(id).DeleteAsync();
        }
    }
}