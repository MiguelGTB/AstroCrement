using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System;

public class DatabaseManager : MonoBehaviour
{
    // --- NUEVA PUERTA DE ACCESO DIRECTO ---
    public static DatabaseManager Instance;
    public PlayerData datosCargados = new PlayerData(); // Mantiene los datos vivos en todo momento

    [Header("Conexión con el Juego")]
    public EconomyManager economy;
    public MejorasManager mejoras; 
    private string userId;
    private DatabaseReference dbReference;

    void Awake()
    {
        // Configuramos la puerta de acceso (Singleton)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        
        if (user != null) 
        {
            userId = user.UserId;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            
            string slot = PartidaActual.SlotSeleccionado;
            if (string.IsNullOrEmpty(slot)) slot = "slot1";

            Debug.Log("Jugador detectado: " + userId + " | Slot activo: " + slot);

            CargarPartidaDeNube();

            InvokeRepeating("GuardarPartidaEnNube", 10f, 60f);
        } 
        else 
        {
            Debug.LogError("Error: Nadie ha iniciado sesión.");
        }
    }

    // --- FUNCIÓN PARA GUARDAR ---
    public async void GuardarPartidaEnNube()
    {
        if (userId == null || dbReference == null) return;

        string slot = PartidaActual.SlotSeleccionado;
        if (string.IsNullOrEmpty(slot)) slot = "slot1";

        // Si hay economía en esta escena (ej: estamos en el juego), actualizamos los datos normales.
        // Si no la hay (ej: estamos en el árbol de prestigio), nos saltamos este paso y solo guardamos el prestigio.
        if (economy != null && mejoras != null)
        {
            datosCargados.nombreUsuario = AuthManager.NombreUsuario;
            datosCargados.dineroActual = economy.dineroActual;
            datosCargados.dineroTotal = economy.dineroTotal;
            datosCargados.dineroPorClic = economy.dineroPorClic;
            datosCargados.dineroPorSeg = economy.dineroPorSeg;
            datosCargados.nivelesCompras = economy.nivelesCompras;

            // Guardar mejoras normales
            datosCargados.mejorasCompradas = new bool[mejoras.listaMejoras.Length];
            for (int i = 0; i < mejoras.listaMejoras.Length; i++)
            {
                datosCargados.mejorasCompradas[i] = mejoras.listaMejoras[i].comprada;
            }
        }

        string json = JsonUtility.ToJson(datosCargados); // Guardamos la variable global
        
        await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").SetRawJsonValueAsync(json);
        
        Debug.Log("¡Partida y Mejoras guardadas automáticamente en " + slot + "!");
    }

    private void OnApplicationQuit() { GuardarPartidaEnNube(); }
    private void OnApplicationPause(bool pausa) { if (pausa) GuardarPartidaEnNube(); }
    
    // --- FUNCIÓN PARA CARGAR ---
    public async void CargarPartidaDeNube()
    {
        if (userId == null) return;

        string slot = PartidaActual.SlotSeleccionado;
        if (string.IsNullOrEmpty(slot)) slot = "slot1";

        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").GetValueAsync();

        if (snapshot.Exists)
        {
            string json = snapshot.GetRawJsonValue();
            // Cargamos TODOS los datos de internet (incluido el prestigio)
            datosCargados = JsonUtility.FromJson<PlayerData>(json);

            // Cargar Economía (Solo si estamos en la escena del juego)
            if (economy != null && mejoras != null)
            {
                economy.dineroActual = datosCargados.dineroActual;
                economy.dineroTotal = datosCargados.dineroTotal;
                economy.dineroPorClic = datosCargados.dineroPorClic;
                economy.dineroPorSeg = datosCargados.dineroPorSeg;
                economy.nivelesCompras = datosCargados.nivelesCompras;

                if (datosCargados.mejorasCompradas != null && datosCargados.mejorasCompradas.Length == mejoras.listaMejoras.Length)
                {
                    for (int i = 0; i < mejoras.listaMejoras.Length; i++)
                    {
                        mejoras.listaMejoras[i].comprada = datosCargados.mejorasCompradas[i];
                        if (datosCargados.mejorasCompradas[i] == true && mejoras.listaMejoras[i].botonAsociado != null)
                        {
                            mejoras.listaMejoras[i].botonAsociado.SetActive(false);
                        }
                    }
                }
                if (economy.ui != null) economy.ui.ActualizarInterfaz();
            }
            
            Debug.Log("¡Partida cargada perfectamente desde " + slot + "!");
            
            // Si hay un gestor del árbol en la escena, le decimos que se actualice
            ArbolManager arbol = FindObjectOfType<ArbolManager>();
            if (arbol != null) arbol.ActualizarTodoElArbol();
        }
        else
        {
            Debug.Log("Nueva partida en " + slot + ".");
        }
    }
}