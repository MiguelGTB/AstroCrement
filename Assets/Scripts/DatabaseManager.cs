using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System;

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
            
            // Leemos el puente para saber qué botón pulsó en el menú
            string slot = PartidaActual.SlotSeleccionado;
            if (string.IsNullOrEmpty(slot)) slot = "slot1"; // Por si acaso hay un error, va al 1

            Debug.Log("Jugador detectado: " + userId + " | Slot activo: " + slot);

            CargarPartidaDeNube();

            // Tu autoguardado: empieza a los 10s y se repite cada 60s
            InvokeRepeating("GuardarPartidaEnNube", 10f, 60f);
        } 
        else 
        {
            Debug.LogError("Error: Nadie ha iniciado sesión.");
        }
    }

    // --- FUNCIÓN PARA GUARDAR ---
    public void GuardarPartidaEnNube()
    {
        if (userId == null) return;

        string slot = PartidaActual.SlotSeleccionado;
        if (string.IsNullOrEmpty(slot)) slot = "slot1";

        PlayerData data = new PlayerData();
        data.dineroActual = economy.dineroActual;
        data.dineroTotal = economy.dineroTotal;
        data.dineroPorClic = economy.dineroPorClic;
        data.dineroPorSeg = economy.dineroPorSeg;
        data.nivelesCompras = economy.nivelesCompras;

        // Guardar mejoras
        data.mejorasCompradas = new bool[mejoras.listaMejoras.Length];
        for (int i = 0; i < mejoras.listaMejoras.Length; i++)
        {
            data.mejorasCompradas[i] = mejoras.listaMejoras[i].comprada;
        }

        string json = JsonUtility.ToJson(data);
        
        // CAMBIO VITAL: Ahora la ruta incluye "slots" y la variable de tu slot actual, y lo mete en "datos"
        dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").SetRawJsonValueAsync(json);
        
        Debug.Log("¡Partida y Mejoras guardadas automáticamente en " + slot + "!");
    }

    // Se ejecuta cuando el jugador cierra el juego
    private void OnApplicationQuit()
    {
        GuardarPartidaEnNube();
    }

    // Se ejecuta cuando el jugador minimiza el juego
    private void OnApplicationPause(bool pausa)
    {
        if (pausa)
        {
            GuardarPartidaEnNube();
        }
    }
    
    // --- FUNCIÓN PARA CARGAR ---
    public async void CargarPartidaDeNube()
    {
        if (userId == null) return;

        string slot = PartidaActual.SlotSeleccionado;
        if (string.IsNullOrEmpty(slot)) slot = "slot1";

        // CAMBIO VITAL: Va a buscar los datos a la carpeta específica de este slot
        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").GetValueAsync();

        if (snapshot.Exists)
        {
            string json = snapshot.GetRawJsonValue();
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            // Cargar Economía
            economy.dineroActual = data.dineroActual;
            economy.dineroTotal = data.dineroTotal;
            economy.dineroPorClic = data.dineroPorClic;
            economy.dineroPorSeg = data.dineroPorSeg;
            economy.nivelesCompras = data.nivelesCompras;

            // Cargar Mejoras
            if (data.mejorasCompradas != null && data.mejorasCompradas.Length == mejoras.listaMejoras.Length)
            {
                for (int i = 0; i < mejoras.listaMejoras.Length; i++)
                {
                    mejoras.listaMejoras[i].comprada = data.mejorasCompradas[i];
                    if (data.mejorasCompradas[i] == true && mejoras.listaMejoras[i].botonAsociado != null)
                    {
                        mejoras.listaMejoras[i].botonAsociado.SetActive(false);
                    }
                }
            }

            if (economy.ui != null) economy.ui.ActualizarInterfaz();
            
            Debug.Log("¡Partida cargada perfectamente desde " + slot + "!");
        }
        else
        {
            Debug.Log("Nueva partida en " + slot + ".");
        }
    }
}