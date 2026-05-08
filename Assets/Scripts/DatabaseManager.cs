using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;

public class DatabaseManager : MonoBehaviour
{
    [Header("Conexión con el Juego")]
    public EconomyManager economy;

    public TextMeshProUGUI textoNombreJugador;
    private string nombreDelJugadorCargado;

    private string userId;
    private DatabaseReference dbReference;


    void Start()
    {
        // 1. Buscamos quién inició sesión en el Menú Principal
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        
        if (user != null) 
        {
            userId = user.UserId;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Jugador detectado: " + userId);

            CargarPartidaDeNube();
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

        // 1. Metemos los datos en la "caja"
        PlayerData data = new PlayerData();
        data.nombreUsuario = nombreDelJugadorCargado;
        data.dineroActual = economy.dineroActual;
        data.dineroPorClic = economy.dineroPorClic;
        data.dineroPorSeg = economy.dineroPorSeg;
        data.nivelesCompras = economy.nivelesCompras;

        // 2. Convertimos la caja a texto JSON
        string json = JsonUtility.ToJson(data);

        // 3. Lo subimos a Firebase bajo el ID del jugador
        dbReference.Child("usuarios").Child(userId).SetRawJsonValueAsync(json);
        
        Debug.Log("¡Partida guardada en la nube con éxito!");
    }

    // --- FUNCIÓN PARA CARGAR ---
    public async void CargarPartidaDeNube()
    {
        if (userId == null) return;

        // 1. Pedimos los datos a Firebase
        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).GetValueAsync();

        if (snapshot.Exists)
        {
            // 2. Leemos el texto JSON y lo sacamos de la caja
            string json = snapshot.GetRawJsonValue();
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            nombreDelJugadorCargado = data.nombreUsuario;

            if (textoNombreJugador != null && !string.IsNullOrEmpty(data.nombreUsuario))
            {
                textoNombreJugador.text = data.nombreUsuario;
            }

            // 3. Se lo aplicamos a tu economía actual
            economy.dineroActual = data.dineroActual;
            economy.dineroPorClic = data.dineroPorClic;
            economy.dineroPorSeg = data.dineroPorSeg;
            economy.nivelesCompras = data.nivelesCompras;

            // Actualizamos la pantalla para ver los números
            if (economy.ui != null) economy.ui.ActualizarInterfaz();
            
            Debug.Log("¡Partida cargada! Bienvenido/a " + data.nombreUsuario);
        }
        else
        {
            Debug.Log("No hay partida guardada para este jugador. Es una partida nueva.");
        }
    }
}