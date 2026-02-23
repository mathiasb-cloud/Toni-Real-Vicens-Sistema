using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class CitaService : FirebaseService
    {
        public CitaService(IConfiguration config) : base(config) { }

        public async Task AddAsync(Cita cita)
        {
            await _firebase
                .Child("Citas")
                .PostAsync(cita);
        }




        public async Task<bool> DeleteCitaCompletaAsync(string id)
        {
            try
            {
                
                await _firebase
                    .Child("Citas")
                    .Child(id)
                    .DeleteAsync();

               
                await _firebase
                    .Child("FichasPsicopedagogicas")
                    .Child(id)
                    .DeleteAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<List<Cita>> GetAllAsync()
        {
            var data = await _firebase
                .Child("Citas")
                .OnceAsync<Cita>();

            return data.Select(x =>
            {
                x.Object.Id = x.Key;
                return x.Object;
            }).ToList();
        }

        public async Task<List<Cita>> GetByAlumnoAsync(string alumnoId)
        {
            var data = await _firebase
                .Child("Citas")
                .OnceAsync<Cita>();

            return data
                .Select(x =>
                {
                    x.Object.Id = x.Key;
                    return x.Object;
                })
                .Where(c => c.AlumnoId == alumnoId)
                .OrderByDescending(c => c.FechaHora)
                .ToList();
        }

        public async Task<Cita?> GetByIdAsync(string id)
        {
            var cita = await _firebase
                .Child("Citas")
                .Child(id)
                .OnceSingleAsync<Cita>();

            if (cita == null) return null;

            cita.Id = id;
            return cita;
        }

        public async Task UpdateEstadoAsync(string id, string estado)
        {
            await _firebase
                .Child("Citas")
                .Child(id)
                .PatchAsync(new { Estado = estado });
        }
    }
}