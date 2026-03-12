using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class FichaService : FirebaseService
    {
        public FichaService(IConfiguration config) : base(config) { }

        // DENTRO DE FichaService.cs
        public async Task<string> AddAsync(FichaDiagnostica ficha) // <-- Cambiado de FichaSeguimiento a FichaDiagnostica
        {
            // Este método debe guardar en "Fichas" (que son las diagnósticas)
            var resultado = await _firebase.Child("Fichas").PostAsync(ficha);
            return resultado.Key;
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
                .OrderBy("AlumnoId")
                .EqualTo(alumnoId)
                .OnceAsync<FichaDiagnostica>();

            Console.WriteLine($"Fichas encontradas para {alumnoId}: {data.Count}");

            return data
                .Select(x => {
                    var ficha = x.Object;
                    ficha.Id = x.Key; 
                    return ficha;
                })
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