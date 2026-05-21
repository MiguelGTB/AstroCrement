using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [HideInInspector] public bool enModoPrestigio = false;

    public DatosPlaneta ObtenerDatosPlanetaActual()
    {
        if (datosCargados == null) return null;

        if (PartidaActual.MundoActual == "Luna") return datosCargados.progresoLuna;
        if (PartidaActual.MundoActual == "Marte") return datosCargados.progresoMarte;
        if (PartidaActual.MundoActual == "Europa") return datosCargados.progresoEuropa;
        if (PartidaActual.MundoActual == "Titan") return datosCargados.progresoTitan;
        if (PartidaActual.MundoActual == "Kepler") return datosCargados.progresoKepler;
        if (PartidaActual.MundoActual == "Dyson") return datosCargados.progresoDyson;
        if (PartidaActual.MundoActual == "Colapso") return datosCargados.progresoColapso;

        return datosCargados.progresoLuna; 
    }

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

            CargarPartidaDeNube();
        } 
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MenuPrincipal")
        {
            Destroy(gameObject);
            return;
        }

        DatosPlaneta planeta = ObtenerDatosPlanetaActual();

        if (economy != null && mejoras != null && planeta != null)
        {
            if (!enModoPrestigio)
            {
                planeta.dineroActual = economy.dineroActual;
                planeta.dineroTotal = economy.dineroTotal;
                planeta.dineroPorClic = economy.dineroPorClic;
                planeta.dineroPorSeg = economy.dineroPorSeg;

                if (economy.nivelesCompras != null)
                {
                    if (planeta.nivelesCompras == null || planeta.nivelesCompras.Length != economy.nivelesCompras.Length)
                        planeta.nivelesCompras = new int[economy.nivelesCompras.Length];
                        
                    for (int i = 0; i < economy.nivelesCompras.Length; i++)
                        planeta.nivelesCompras[i] = economy.nivelesCompras[i];
                }

                if (mejoras.listaMejoras != null)
                {
                    if (planeta.mejorasCompradas == null || planeta.mejorasCompradas.Length != mejoras.listaMejoras.Length)
                        planeta.mejorasCompradas = new bool[mejoras.listaMejoras.Length];

                    for (int i = 0; i < mejoras.listaMejoras.Length; i++)
                        planeta.mejorasCompradas[i] = mejoras.listaMejoras[i].comprada;
                }
            }

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
        enModoPrestigio = false; 

        economy = nuevaEconomia;
        mejoras = nuevasMejoras;
        
        DatosPlaneta planeta = ObtenerDatosPlanetaActual();

        if (economy != null && planeta != null)
        {
            if (planeta.dineroPorClic <= 0) planeta.dineroPorClic = 1;

            economy.dineroActual = planeta.dineroActual;
            economy.dineroTotal = planeta.dineroTotal;
            economy.dineroPorClic = planeta.dineroPorClic;
            economy.dineroPorSeg = planeta.dineroPorSeg;
            
            if (planeta.nivelesCompras != null && economy.nivelesCompras != null)
            {
                int limite = Mathf.Min(planeta.nivelesCompras.Length, economy.nivelesCompras.Length);
                for (int i = 0; i < limite; i++)
                    economy.nivelesCompras[i] = planeta.nivelesCompras[i];
            }

            if (mejoras != null && planeta.mejorasCompradas != null && mejoras.listaMejoras != null)
            {
                int limiteMej = Mathf.Min(planeta.mejorasCompradas.Length, mejoras.listaMejoras.Length);
                for (int i = 0; i < limiteMej; i++)
                {
                    mejoras.listaMejoras[i].comprada = planeta.mejorasCompradas[i];
                    if (planeta.mejorasCompradas[i] == true && mejoras.listaMejoras[i].botonAsociado != null)
                        mejoras.listaMejoras[i].botonAsociado.SetActive(false);
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

        datosCargados.nombreUsuario = AuthManager.NombreUsuario;

        try
        {
            string json = JsonUtility.ToJson(datosCargados); 
            await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").SetRawJsonValueAsync(json);
            
            // Guardar mejoras de prestigio
            if (datosCargados.mejorasPrestigioCompradas != null && datosCargados.mejorasPrestigioCompradas.Count > 0)
            {
                Dictionary<string, object> dictPrestigio = new Dictionary<string, object>();
                for (int i = 0; i < datosCargados.mejorasPrestigioCompradas.Count; i++)
                    dictPrestigio[i.ToString()] = datosCargados.mejorasPrestigioCompradas[i];
                
                await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").Child("mejorasPrestigioCompradas").SetValueAsync(dictPrestigio);
            }
            else
            {
                await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").Child("mejorasPrestigioCompradas").RemoveValueAsync();
            }

            // NUEVO: Guardar logros completados
            if (datosCargados.logrosCompletados != null && datosCargados.logrosCompletados.Count > 0)
            {
                Dictionary<string, object> dictLogros = new Dictionary<string, object>();
                for (int i = 0; i < datosCargados.logrosCompletados.Count; i++)
                    dictLogros[i.ToString()] = datosCargados.logrosCompletados[i];

                await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot)
                    .Child("datos").Child("logrosCompletados").SetValueAsync(dictLogros);
            }
            else
            {
                await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot)
                    .Child("datos").Child("logrosCompletados").RemoveValueAsync();
            }
        }
        catch (Exception) {}
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

            if (datosCargados.progresoLuna == null) datosCargados.progresoLuna = new DatosPlaneta();
            if (datosCargados.progresoMarte == null) datosCargados.progresoMarte = new DatosPlaneta();
            if (datosCargados.progresoEuropa == null) datosCargados.progresoEuropa = new DatosPlaneta();

            if (datosCargados.progresoLuna.dineroPorClic <= 0) datosCargados.progresoLuna.dineroPorClic = 1;
            if (datosCargados.progresoMarte.dineroPorClic <= 0) datosCargados.progresoMarte.dineroPorClic = 1;

            // Cargar mejoras de prestigio
            datosCargados.mejorasPrestigioCompradas = new List<string>();
            DataSnapshot snapPrestigio = snapshot.Child("mejorasPrestigioCompradas");
            if (snapPrestigio.Exists)
            {
                foreach (var child in snapPrestigio.Children)
                    datosCargados.mejorasPrestigioCompradas.Add(child.Value.ToString());
            }

            // NUEVO: Cargar logros completados
            datosCargados.logrosCompletados = new List<string>();
            DataSnapshot snapLogros = snapshot.Child("logrosCompletados");
            if (snapLogros.Exists)
            {
                foreach (var child in snapLogros.Children)
                    datosCargados.logrosCompletados.Add(child.Value.ToString());
            }

            DatosPlaneta planeta = ObtenerDatosPlanetaActual();

            if (economy != null && mejoras != null && planeta != null)
            {
                economy.dineroActual = planeta.dineroActual;
                economy.dineroTotal = planeta.dineroTotal;
                economy.dineroPorClic = planeta.dineroPorClic;
                economy.dineroPorSeg = planeta.dineroPorSeg;

                if (planeta.nivelesCompras != null && economy.nivelesCompras != null)
                {
                    int limite = Mathf.Min(planeta.nivelesCompras.Length, economy.nivelesCompras.Length);
                    for (int i = 0; i < limite; i++)
                        economy.nivelesCompras[i] = planeta.nivelesCompras[i];
                }

                if (planeta.mejorasCompradas != null && mejoras.listaMejoras != null)
                {
                    int limiteMej = Mathf.Min(planeta.mejorasCompradas.Length, mejoras.listaMejoras.Length);
                    for (int i = 0; i < limiteMej; i++)
                    {
                        mejoras.listaMejoras[i].comprada = planeta.mejorasCompradas[i];
                        if (planeta.mejorasCompradas[i] == true && mejoras.listaMejoras[i].botonAsociado != null)
                            mejoras.listaMejoras[i].botonAsociado.SetActive(false);
                    }
                }
                if (economy.ui != null) economy.ui.ActualizarInterfaz();
            }
            
            ArbolManager arbol = FindObjectOfType<ArbolManager>();
            if (arbol != null) arbol.ActualizarTodoElArbol();
        }
    }
}