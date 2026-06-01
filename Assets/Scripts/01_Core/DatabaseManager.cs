using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    // Instancia única del script (Patrón Singleton) para que no se duplique
    public static DatabaseManager Instance;
    // Contenedor principal de todos los datos del jugador
    public PlayerData datosCargados = new PlayerData(); 

    [Header("Conexión con el Juego")]
    public EconomyManager economy;
    public MejorasManager mejoras; 
    private string userId; // ID único del jugador en Firebase
    private DatabaseReference dbReference; // Referencia a la base de datos

    // Variables para el autoguardado periódico
    private float tiempoParaGuardar = 60f;
    private float cronometro = 0f;

    [HideInInspector] public bool enModoPrestigio = false;
    public bool partidaCargadaConExito = false;

    // Función que devuelve los datos guardados según el planeta en el que estés jugando
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

        return datosCargados.progresoLuna; // Planeta por defecto
    }

    void Awake()
    {
        // Configuración del Patrón Singleton: sobrevive al cambio de escenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Si ya existe otro DatabaseManager, destruye este nuevo
            Destroy(gameObject);
            return; 
        }
    }

    void Start()
    {
        // Al iniciar, comprueba si el usuario ha iniciado sesión
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null) 
        {
            userId = user.UserId;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            
            // Determina en qué ranura de guardado (slot) estamos jugando
            string slot = PartidaActual.SlotSeleccionado;
            if (string.IsNullOrEmpty(slot)) slot = "slot1";

            // Inicia la descarga de datos e información de IA
            CargarPartidaDeNube();
            CargarConfiguracionIA();
        } 
    }

    void Update()
    {
        // Si estamos en el menú principal, este script no hace falta y se destruye
        if (SceneManager.GetActiveScene().name == "MenuPrincipal")
        {
            Destroy(gameObject);
            return;
        }

        DatosPlaneta planeta = ObtenerDatosPlanetaActual();

        // Actualiza constantemente la clase de datos (datosCargados) con lo que pasa en el juego
        if (economy != null && mejoras != null && planeta != null)
        {
            if (!enModoPrestigio)
            {
                // Copia el dinero y las estadísticas actuales
                planeta.dineroActual = economy.dineroActual;
                planeta.dineroTotal = economy.dineroTotal;
                planeta.dineroPorClic = economy.dineroPorClic;
                planeta.dineroPorSeg = economy.dineroPorSeg;

                // Copia los niveles de los edificios/compras
                if (economy.nivelesCompras != null)
                {
                    if (planeta.nivelesCompras == null || planeta.nivelesCompras.Length != economy.nivelesCompras.Length)
                        planeta.nivelesCompras = new int[economy.nivelesCompras.Length];
                        
                    for (int i = 0; i < economy.nivelesCompras.Length; i++)
                        planeta.nivelesCompras[i] = economy.nivelesCompras[i];
                }

                // Copia el estado de las mejoras (comprada o no)
                if (mejoras.listaMejoras != null)
                {
                    if (planeta.mejorasCompradas == null || planeta.mejorasCompradas.Length != mejoras.listaMejoras.Length)
                        planeta.mejorasCompradas = new bool[mejoras.listaMejoras.Length];

                    for (int i = 0; i < mejoras.listaMejoras.Length; i++)
                        planeta.mejorasCompradas[i] = mejoras.listaMejoras[i].comprada;
                }
            }

            // Sistema de autoguardado (cada 60 segundos por defecto)
            cronometro += Time.deltaTime;
            if (cronometro >= tiempoParaGuardar)
            {
                cronometro = 0f;
                GuardarPartidaEnNube();
            }
        }
    }

    // Se llama al cambiar de escena/planeta para volver a conectar las variables con los nuevos scripts
    public void ReconectarEscenaActual(EconomyManager nuevaEconomia, MejorasManager nuevasMejoras)
    {
        enModoPrestigio = false; 

        economy = nuevaEconomia;
        mejoras = nuevasMejoras;
        
        DatosPlaneta planeta = ObtenerDatosPlanetaActual();

        // Vuelca los datos guardados sobre los managers de la nueva escena
        if (economy != null && planeta != null)
        {
            if (planeta.dineroPorClic <= 0) planeta.dineroPorClic = 1; // Prevención de errores de clic a 0

            economy.dineroActual = planeta.dineroActual;
            economy.dineroTotal = planeta.dineroTotal;
            economy.dineroPorClic = planeta.dineroPorClic;
            economy.dineroPorSeg = planeta.dineroPorSeg;
            
            // Restaura los niveles de compras
            if (planeta.nivelesCompras != null && economy.nivelesCompras != null)
            {
                int limite = Mathf.Min(planeta.nivelesCompras.Length, economy.nivelesCompras.Length);
                for (int i = 0; i < limite; i++)
                    economy.nivelesCompras[i] = planeta.nivelesCompras[i];
            }

            // Restaura las mejoras y desactiva los botones de las que ya están compradas
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
            // Actualiza los textos de la interfaz
            if (economy.ui != null) economy.ui.ActualizarInterfaz();
        }
    }

    // Convierte todos tus datos en JSON y los sube a la base de datos de Firebase
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
            
            // Guarda la lista de mejoras de prestigio en Firebase (diccionario)
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

            // Guarda los logros completados
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

    // Guarda automáticamente la partida si cierras la app o la pones en segundo plano
    private void OnApplicationQuit() { GuardarPartidaEnNube(); }
    private void OnApplicationPause(bool pausa) { if (pausa) GuardarPartidaEnNube(); }
    
    // Descarga el JSON desde Firebase y lo asigna a las variables de juego
    public async void CargarPartidaDeNube()
    {
        if (userId == null) return;
        string slot = PartidaActual.SlotSeleccionado;
        if (string.IsNullOrEmpty(slot)) slot = "slot1";

        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).Child("slots").Child(slot).Child("datos").GetValueAsync();

        if (snapshot.Exists)
        {
            // Convierte el texto descargado en tu objeto 'PlayerData'
            string json = snapshot.GetRawJsonValue();
            datosCargados = JsonUtility.FromJson<PlayerData>(json);

            // Medidas de seguridad por si es una partida nueva y algunos planetas están vacíos
            if (datosCargados.progresoLuna == null) datosCargados.progresoLuna = new DatosPlaneta();
            if (datosCargados.progresoMarte == null) datosCargados.progresoMarte = new DatosPlaneta();
            if (datosCargados.progresoEuropa == null) datosCargados.progresoEuropa = new DatosPlaneta();

            if (datosCargados.progresoLuna.dineroPorClic <= 0) datosCargados.progresoLuna.dineroPorClic = 1;
            if (datosCargados.progresoMarte.dineroPorClic <= 0) datosCargados.progresoMarte.dineroPorClic = 1;

            // Lee y carga el diccionario de mejoras de prestigio
            datosCargados.mejorasPrestigioCompradas = new List<string>();
            DataSnapshot snapPrestigio = snapshot.Child("mejorasPrestigioCompradas");
            if (snapPrestigio.Exists)
            {
                foreach (var child in snapPrestigio.Children)
                    datosCargados.mejorasPrestigioCompradas.Add(child.Value.ToString());
            }

            // Lee y carga el diccionario de logros completados
            datosCargados.logrosCompletados = new List<string>();
            DataSnapshot snapLogros = snapshot.Child("logrosCompletados");
            if (snapLogros.Exists)
            {
                foreach (var child in snapLogros.Children)
                    datosCargados.logrosCompletados.Add(child.Value.ToString());
            }

            DatosPlaneta planeta = ObtenerDatosPlanetaActual();

            // Sincroniza la información descargada con la UI y managers actuales de la escena
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
            
            // Actualiza el árbol de habilidades visualmente
            ArbolManager arbol = FindObjectOfType<ArbolManager>();
            if (arbol != null) arbol.ActualizarTodoElArbol();

            // Bandera (flag) que indica que ya podemos empezar a jugar
            partidaCargadaConExito = true;
            Debug.Log("¡Firebase ha terminado de descargar los datos de los planetas!");
        }
    }

    // Descarga las credenciales de la IA (ElevenLabs) de una ruta segura en Firebase
    public void CargarConfiguracionIA()
    {
        // Este es mi puente secreto con la IA. Voy a leer la API Key directamente desde mi Firebase
        // para que nadie pueda robármela si intentan descompilar el juego. ¡Seguridad ante todo!
        FirebaseDatabase.DefaultInstance.GetReference("configuracion_ia").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                // Extraigo la llave de ElevenLabs y el estado (activo/inactivo) que puse en la web
                string llave = task.Result.Child("elevenlabs_api_key").Value.ToString();
                string estado = task.Result.Child("estado").Value.ToString();

                // Si mi TextToSpeechManager ya nació y está listo en la escena, le inyecto los datos de golpe
                if (TextToSpeechManager.Instance != null)
                {
                    TextToSpeechManager.Instance.apiKey = llave;
                    TextToSpeechManager.Instance.sistemaActivo = (estado == "activo");
                    
                    Debug.Log("¡He cargado la configuración de la IA desde la nube! Estado actual: " + estado);
                }
            }
            else
            {
                // Aviso en consola si olvidaste crear el nodo en Firebase Database
                Debug.LogWarning("Vaya, parece que no he encontrado la carpeta 'configuracion_ia' en mi base de datos.");
            }
        });
    }
}