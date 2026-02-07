using Firebase.Database;

namespace Toni_Real_Vicens_Sistema.Service
{
    public class FirebaseService
    {
        protected readonly FirebaseClient _firebase;

        public FirebaseService(IConfiguration config)
        {
            _firebase = new FirebaseClient(
                config["Firebase:DatabaseUrl"]
            );
        }
    }

}
