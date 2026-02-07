using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class FichaService : FirebaseService
    {
        public FichaService(IConfiguration config) : base(config) { }

        public async Task AddAsync(FichaDiagnostica ficha)
        {
            await _firebase
                .Child("Fichas")
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
    }
}