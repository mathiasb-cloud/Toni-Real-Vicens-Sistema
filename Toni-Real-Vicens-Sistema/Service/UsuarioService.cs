using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class UsuarioService : FirebaseService
    {
        public UsuarioService(IConfiguration config) : base(config) { }

        public async Task AddAsync(Usuario usuario)
        {
            await _firebase.Child("Usuarios").PostAsync(usuario);
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            var data = await _firebase.Child("Usuarios").OnceAsync<Usuario>();
            return data.Select(x => {
                x.Object.Id = x.Key;
                return x.Object;
            }).ToList();
        }

        public async Task<Usuario?> LoginAsync(string correo, string pass)
        {
            var usuarios = await GetAllAsync();
            return usuarios.FirstOrDefault(u => u.Correo == correo && u.Contrasena == pass);
        }
    }
}
