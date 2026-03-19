using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class UsuarioService : FirebaseService
    {
        public UsuarioService(IConfiguration config) : base(config) { }

        public async Task<Usuario?> LoginAsync(string correo, string pass)
        {
            var usuarios = await GetAllAsync();
            // Buscamos el usuario ignorando mayúsculas en el correo por comodidad del usuario
            return usuarios.FirstOrDefault(u => u.Correo?.ToLower() == correo.ToLower() && u.Contrasena == pass);
        }

        public async Task<bool> AddAsync(Usuario usuario)
        {
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
                var obj = x.Object;
                obj.Id = x.Key; // Asignamos la clave de Firebase al ID del modelo
                return obj;
            }).ToList();
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            // Traer directamente el nodo es más rápido que listar todos
            var user = await _firebase.Child("Usuarios").Child(id).OnceSingleAsync<Usuario>();
            if (user != null) user.Id = id;
            return user;
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            // PutAsync sobrescribe el nodo con el ID específico
            await _firebase.Child("Usuarios").Child(usuario.Id).PutAsync(usuario);
        }

        public async Task DeleteAsync(string id)
        {
            await _firebase.Child("Usuarios").Child(id).DeleteAsync();
        }
    }
}