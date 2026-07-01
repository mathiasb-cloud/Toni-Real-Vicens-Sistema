using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class FichaService : FirebaseService
    {
        public FichaService(IConfiguration config) : base(config) { }

        public async Task<string> AddAsync(FichaDiagnostica ficha)
        {
            var resultado = await _firebase.Child("Fichas").PostAsync(ficha);
            return resultado.Key;
        }

        public async Task UpdateAsync(FichaDiagnostica ficha)
        {
            await _firebase
                .Child("Fichas")
                .Child(ficha.Id)
                .PutAsync(ficha);
        }

        // Método para obtener todas las fichas de un alumno
        public async Task<List<FichaDiagnostica>> GetByAlumnoAsync(string alumnoId)
        {
            var data = await _firebase
                .Child("Fichas")
                .OrderBy("AlumnoId")
                .EqualTo(alumnoId)
                .OnceAsync<FichaDiagnostica>();

            return data.Select(x =>
            {
                var ficha = x.Object;
                ficha.Id = x.Key;
                return ficha;
            }).OrderByDescending(f => f.Fecha).ToList();
        }

        // Método para obtener una ficha por su ID
        public async Task<FichaDiagnostica> GetByIdAsync(string id)
        {
            var result = await _firebase
                .Child("Fichas")
                .Child(id)
                .OnceSingleAsync<FichaDiagnostica>();

            if (result != null)
            {
                result.Id = id;
            }
            return result;
        }

        // Obtener todas las fichas
        public async Task<List<FichaDiagnostica>> GetAllAsync()
        {
            var data = await _firebase.Child("Fichas").OnceAsync<FichaDiagnostica>();
            return data.Select(item =>
            {
                var ficha = item.Object;
                ficha.Id = item.Key;
                return ficha;
            }).ToList();
        }
    }
}
