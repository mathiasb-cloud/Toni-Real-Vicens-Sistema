using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class FichaService : FirebaseService
    {
        public FichaService(IConfiguration config) : base(config) { }

        public async Task<string> AddAsync(FichaDiagnostica ficha)
        {
            var result = await _firebase
                .Child("Fichas")
                .PostAsync(ficha);

            return result.Key;
        }

        public async Task UpdateAsync(FichaDiagnostica ficha)
        {
            // Importante: Usamos ficha.Id que es la Key de Firebase
            await _firebase
                .Child("Fichas")
                .Child(ficha.Id)
                .PutAsync(ficha);
        }

        public async Task AddSeguimientoAsync(FichaSeguimiento ficha)
        {
            await _firebase
                .Child("FichasSeguimiento")
                .PostAsync(ficha);
        }

        public async Task<List<FichaDiagnostica>> GetByAlumnoAsync(string alumnoId)
        {
            var data = await _firebase
                .Child("Fichas")
                .OnceAsync<FichaDiagnostica>();

            return data
                .Select(x =>
                {
                    x.Object.Id = x.Key;
                    return x.Object;
                })
                .Where(f => f.AlumnoId == alumnoId)
                .OrderByDescending(f => f.Fecha)
                .ToList();
        }


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
    }
}