using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class AlumnoService : FirebaseService
    {
        public AlumnoService(IConfiguration config) : base(config) { }

        public async Task AddAsync(Alumno alumno)
        {
            await _firebase
                .Child("Alumnos")
                .PostAsync(alumno);
        }

        public async Task<List<Alumno>> GetAllAsync()
        {
            var data = await _firebase
                .Child("Alumnos")
                .OnceAsync<Alumno>();

            return data.Select(x =>
            {
                x.Object.Id = x.Key;
                return x.Object;
            }).ToList();
        }

        public async Task<Alumno?> GetByDniAsync(string dni)
        {
            var data = await _firebase
                .Child("Alumnos")
                .OnceAsync<Alumno>();

            return data
                .Select(x =>
                {
                    x.Object.Id = x.Key;
                    return x.Object;
                })
                .FirstOrDefault(a => a.DNI == dni);
        }


        public async Task<Alumno> GetByIdAsync(string id)
        {
            return await _firebase
                .Child("Alumnos")
                .Child(id)
                .OnceSingleAsync<Alumno>();
        }
    }

}
