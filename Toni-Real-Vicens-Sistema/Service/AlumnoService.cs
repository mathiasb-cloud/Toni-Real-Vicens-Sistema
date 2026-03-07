using Firebase.Database;
using Firebase.Database.Query;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class AlumnoService : FirebaseService
    {
        public AlumnoService(IConfiguration config) : base(config) { }

        public async Task AddAsync(Alumno alumno)
        {
            // Forzamos Id nulo para que Firebase genere la clave única
            alumno.Id = null;
            await _firebase.Child("Alumnos").PostAsync(alumno);
        }

        public async Task<List<Alumno>> GetAllAsync()
        {
            var data = await _firebase.Child("Alumnos").OnceAsync<Alumno>();
            return data.Select(x => {
                x.Object.Id = x.Key;
                return x.Object;
            }).ToList();
        }

        public async Task<List<Alumno>> GetFiltradosAsync(string nivel, string grado, string seccion)
        {
            var todos = await GetAllAsync();
            return todos.Where(a =>
                (string.IsNullOrEmpty(nivel) || a.Nivel == nivel) &&
                (string.IsNullOrEmpty(grado) || a.Grado == grado) &&
                (string.IsNullOrEmpty(seccion) || a.Seccion == seccion)
            ).ToList();
        }

        public async Task<Alumno?> GetByDniAsync(string dni)
        {
            var data = await GetAllAsync();
            return data.FirstOrDefault(a => a.DNI == dni);
        }

        public async Task<Alumno> GetByIdAsync(string id)
        {
            var alumno = await _firebase.Child("Alumnos").Child(id).OnceSingleAsync<Alumno>();
            if (alumno != null) alumno.Id = id;
            return alumno;
        }

        public async Task UpdateAsync(string id, Alumno alumno)
        {
            alumno.Id = id;
            await _firebase.Child("Alumnos").Child(id).PutAsync(alumno);
        }

        public async Task DeleteAsync(string id)
        {
            await _firebase.Child("Alumnos").Child(id).DeleteAsync();
        }

        public async Task PromoverAlumnoAsync(string id, string nuevoGrado, string nuevoNivel, int nuevoAnio)
        {
            var alumno = await GetByIdAsync(id);
            if (alumno == null) return;

            // Actualizamos campos raíz
            alumno.Grado = nuevoGrado;
            alumno.Nivel = nuevoNivel;

            // Actualizamos el objeto de matrícula
            alumno.MatriculaActual = new Matricula
            {
                Anio = nuevoAnio,
                Grado = nuevoGrado,
                Nivel = nuevoNivel,
                Seccion = alumno.Seccion ?? "A",
                EstadoMatricula = "Regular",
                FechaInscripcion = DateTime.Now
            };

            // Usar el nombre correcto de tu cliente Firebase (ej. _client o _firebase)
            await _firebase.Child("Alumnos").Child(id).PutAsync(alumno);
        }
    }
}