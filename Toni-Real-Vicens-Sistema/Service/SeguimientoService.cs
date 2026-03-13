using Firebase.Database;
using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class SeguimientoService
    {
        private readonly FirebaseClient _firebase;

        public SeguimientoService(IConfiguration config)
        {
            _firebase = new FirebaseClient(config["Firebase:DatabaseUrl"]);
        }


        public async Task<string> AddAsync(FichaSeguimiento ficha)
        {
            var resultado = await _firebase.Child("FichasSeguimiento").PostAsync(ficha);
            return resultado.Key; // Esto es el string que necesita el controlador
        }


        public async Task<List<FichaSeguimiento>> GetByAlumnoAsync(string alumnoId)
        {
            var data = await _firebase
                .Child("FichasSeguimiento")
                .OrderBy("AlumnoId")
                .EqualTo(alumnoId)
                .OnceAsync<FichaSeguimiento>();

            return data.Select(item => {
                item.Object.Id = item.Key;
                return item.Object;
            }).ToList();
        }

        
        public async Task<FichaSeguimiento> GetByIdAsync(string id)
        {
            return await _firebase.Child("FichasSeguimiento").Child(id).OnceSingleAsync<FichaSeguimiento>();
        }

        
        public async Task UpdateAsync(string id, FichaSeguimiento ficha)
        {
            await _firebase.Child("FichasSeguimiento").Child(id).PutAsync(ficha);
        }



        public async Task<List<FichaSeguimiento>> GetAllAsync()
        {
            
            var data = await _firebase.Child("FichasSeguimiento").OnceAsync<FichaSeguimiento>();

            return data.Select(item => {
                var seguimiento = item.Object;
                seguimiento.Id = item.Key; 
                return seguimiento;
            }).ToList();
        }


    }
}