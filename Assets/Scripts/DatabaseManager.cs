using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System;
using System.Collections.Generic;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;
    public PlayerData datosCargados = new PlayerData(); 

    [Header("Conexión con el Juego")]
    public EconomyManager economy;
    public MejorasManager mejoras; 
    private string userId;
    private DatabaseReference dbReference;

    private float tiempoParaGuardar = 60f;
    private float cronometro = 0f;

    // ---> ESTA ES LA VARIABLE QUE FALTABA <---
    [HideInInspector] public bool enModoPrestigio = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return; 
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
        } 
        else 
        {
            Debug.LogError("Error: Nadie ha iniciado sesión.");
        }
    }

    void Update()
    {
        if (economy != null && mejoras != null)
        {
            cronometro += Time.deltaTime;
            if (cronometro >= tiempoParaGuardar)
            {
                cronometro = 0f;
                GuardarPartidaEnNube();
            }
        }
    }

    public void ReconectarEscenaActual(EconomyManager nuevaEconomia, MejorasManager nuevasMejoras)
    {
        // ---> APAGAMOS EL MODO PRESTIGIO AL VOLVER AL JUEGO <---
        enModoPrestigio = false; 

        economy = nuevaEconomia;
        mejoras = nuevasMejoras;
        Debug.Log("DatabaseManager: Cables reconectados con éxito a la nueva escena.");
        
        if (economy != null && datosCargados != null)
        {
            if (datosCargados.dineroPorClic <= 0) datosCargados.dineroPorClic = 1;

            economy.dineroActual = datosCargados.dineroActual;
            economy.dineroTotal = datosCargados.dineroTotal;
            economy.dineroPorClic = datosCargados.dineroPorClic;
            economy.dineroPorSeg = datosCargados.dineroPorSeg;
            
            if (datosCargados.nivelesCompras != null && economy.nivelesCompras != null)
            {
                int limite = Mathf.Min(datosCargados.nivelesCompras.Length, economy.nivelesCompras.Length);
                for (int i = 0; i < limite; i++)
                {
                    economy.nivelesCompras[i] = datosCargados.nivelesCompras[i];
                }
            }

            if (mejoras != null && datosCargados.mejorasCompradas != null && mejoras.listaMejoras != null)
            {
                int limiteMej = Mathf.Min(datosCargados.mejorasCompradas.Length, mejoras.listaMejoras.Length);
                for (int i = 0; i < limiteMej; i++)
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
    }

    public async void GuardarPartidaEnNube()
    {
        if (userId == null || dbReference == null) return;

        string slot = PartidaActual.SlotSeleccionado;
        if (string.IsNullOrEmpty(slot)) slot = "slot1";

        // ---> BLOQUEAMOS EL GUARDADO SI ESTAMOS REENCARNANDO <---
        if (!enModoPrestigio && economy != null && mejoras != null && economy.gameObject != null)
        {
            datosCargados.nombreUsuario = AuthManager.NombreUsuario;
            datosCargados.dineroActual = economy.dineroActual;
            datosCargados.dineroTotal = economy.dineroTotal;
            datosCargados.dineroPorClic = economy.dineroPorClic;
            datosCargados.dineroPorSeg = economy.dineroPorSeg;
            
            if (economy.nivelesCompras != null)
            {
                datosCargados.nivelesCompras = new int[economy.nivelesCompras.Length];
                for (int i = 0; i < economy.nivelesCompras.Length; i++)
                    datosCargados.nivelesCompras[i] = economy.nivelesCompras[i];
            }

            if (mejoras.listaMejoras != null)
            {
                datosCargados.mejorasCompradas = new bool[mejoras.listaMejoras.Length];
                for (int i = 0; i < mejoras.listaMejoras.Length; i++)
                {
                    datosCargados.mejorasCompradas[i] = mejoras.listaMejoras[i].comprada;
                }
            }
        }

        try
        {
            string json = JsonUtility.ToJson(datosCargados); 
            await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").SetRawJsonValueAsync(json);
            Debug.Log("¡Partida guardada de forma segura en " + slot + "!");
        }
        catch (Exception e)
        {
            Debug.LogWarning("Guardado en la nube omitido o fallido: " + e.Message);
        }
    }

    private void OnApplicationQuit() { GuardarPartidaEnNube(); }
    private void OnApplicationPause(bool pausa) { if (pausa) GuardarPartidaEnNube(); }
    
    public async void CargarPartidaDeNube()
    {
        if (userId == null) return;

        string slot = PartidaActual.SlotSeleccionado;
        if (string.IsNullOrEmpty(slot)) slot = "slot1";

        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").GetValueAsync();

        if (snapshot.Exists)
        {
            string json = snapshot.GetRawJsonValue();
            datosCargados = JsonUtility.FromJson<PlayerData>(json);

            if (datosCargados.dineroPorClic <= 0)
            {
                datosCargados.dineroPorClic = 1;
            }

            datosCargados.mejorasPrestigioCompradas = new List<string>();
            DataSnapshot snapPrestigio = snapshot.Child("mejorasPrestigioCompradas");
            if (snapPrestigio.Exists)
            {
                foreach (var child in snapPrestigio.Children)
                {
                    datosCargados.mejorasPrestigioCompradas.Add(child.Value.ToString());
                }
            }

            if (economy != null && mejoras != null)
            {
                economy.dineroActual = datosCargados.dineroActual;
                economy.dineroTotal = datosCargados.dineroTotal;
                economy.dineroPorClic = datosCargados.dineroPorClic;
                economy.dineroPorSeg = datosCargados.dineroPorSeg;

                if (datosCargados.nivelesCompras != null && economy.nivelesCompras != null)
                {
                    int limite = Mathf.Min(datosCargados.nivelesCompras.Length, economy.nivelesCompras.Length);
                    for (int i = 0; i < limite; i++)
                    {
                        economy.nivelesCompras[i] = datosCargados.nivelesCompras[i];
                    }
                }

                if (datosCargados.mejorasCompradas != null && mejoras.listaMejoras != null)
                {
                    int limiteMej = Mathf.Min(datosCargados.mejorasCompradas.Length, mejoras.listaMejoras.Length);
                    for (int i = 0; i < limiteMej; i++)
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
            
            ArbolManager arbol = FindObjectOfType<ArbolManager>();
            if (arbol != null) arbol.ActualizarTodoElArbol();
        }
        else
        {
            Debug.Log("Nueva partida en " + slot + ".");
        }
    }
}