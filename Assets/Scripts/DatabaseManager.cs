using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{
    [Header("Conexión con el Juego")]
    public EconomyManager economy;
    public MejorasManager mejoras; 
    private string userId;
    private DatabaseReference dbReference;

    void Start()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        
        if (user != null) 
        {
            userId = user.UserId;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Jugador detectado: " + userId);
        } 
        else 
        {
            Debug.LogError("Error: Nadie ha iniciado sesión.");
        }
    }

    public void GuardarPartidaEnNube()
    {
        if (userId == null) return;

        PlayerData data = new PlayerData();
        data.dineroActual = economy.dineroActual;
        data.dineroPorClic = economy.dineroPorClic;
        data.dineroPorSeg = economy.dineroPorSeg;
        data.nivelesCompras = economy.nivelesCompras;

        // --- NUEVO: GUARDAR MEJORAS ---
        // Creamos una lista del mismo tamaño que tus mejoras
        data.mejorasCompradas = new bool[mejoras.listaMejoras.Length];
        
        for (int i = 0; i < mejoras.listaMejoras.Length; i++)
        {
            // Apuntamos 'true' si la tienes comprada, o 'false' si no
            data.mejorasCompradas[i] = mejoras.listaMejoras[i].comprada;
        }
        // ------------------------------

        string json = JsonUtility.ToJson(data);
        dbReference.Child("usuarios").Child(userId).SetRawJsonValueAsync(json);
        
        Debug.Log("¡Partida y Mejoras guardadas en la nube!");
    }

    public async void CargarPartidaDeNube()
    {
        if (userId == null) return;

        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).GetValueAsync();

        if (snapshot.Exists)
        {
            string json = snapshot.GetRawJsonValue();
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            // Cargar Economía
            economy.dineroActual = data.dineroActual;
            economy.dineroPorClic = data.dineroPorClic;
            economy.dineroPorSeg = data.dineroPorSeg;
            economy.nivelesCompras = data.nivelesCompras;

            // --- NUEVO: CARGAR MEJORAS ---
            // Nos aseguramos de que el array guardado existe y no da error
            if (data.mejorasCompradas != null && data.mejorasCompradas.Length == mejoras.listaMejoras.Length)
            {
                for (int i = 0; i < mejoras.listaMejoras.Length; i++)
                {
                    // Le decimos a tu script si esta mejora está comprada o no
                    mejoras.listaMejoras[i].comprada = data.mejorasCompradas[i];

                    // Si está comprada, apagamos el botón de inmediato para que no se vea
                    if (data.mejorasCompradas[i] == true && mejoras.listaMejoras[i].botonAsociado != null)
                    {
                        mejoras.listaMejoras[i].botonAsociado.SetActive(false);
                    }
                }
            }
            // ------------------------------

            if (economy.ui != null) economy.ui.ActualizarInterfaz();
            
            Debug.Log("¡Partida cargada perfectamente!");
        }
        else
        {
            Debug.Log("Nueva partida.");
        }
    }
}