using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class GestorSlots : MonoBehaviour
{
    private string userId;
    private DatabaseReference dbReference;

    [Header("Configuración de Escenas")]
    public GameObject panelSelectorPartidas;
    public string escenaNuevaPartida = "IntroJuego"; // Nombre de tu escena del cómic
    public string escenaCargarPartida = "Seleccion_Niveles"; // A donde van al darle a "Jugar"

    [Header("Slot 1")]
    public TextMeshProUGUI txtNombreS1;
    public GameObject btnJugarS1, btnBorrarS1, grupoCrearS1;
    public TMP_InputField inputS1; // El recuadro donde escribes el nombre

    [Header("Slot 2")]
    public TextMeshProUGUI txtNombreS2;
    public GameObject btnJugarS2, btnBorrarS2, grupoCrearS2;
    public TMP_InputField inputS2;

    [Header("Slot 3")]
    public TextMeshProUGUI txtNombreS3;
    public GameObject btnJugarS3, btnBorrarS3, grupoCrearS3;
    public TMP_InputField inputS3;

    public void InicializarSelector()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        userId = user != null ? user.UserId : null;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (panelSelectorPartidas != null)
        {
            panelSelectorPartidas.SetActive(true);
        }
        else
        {
            Debug.LogError("Panel_SelectorPartidas no está asignado en GestorSlots.");
        }

        if (user != null)
        {
            ActualizarTodosLosSlots();
            return;
        }

        Debug.LogWarning("No hay usuario Firebase activo. Se muestra el panel de selección sin cargar slots.");
    }

    public void ActualizarTodosLosSlots()
    {
        ConfigurarUI(1, txtNombreS1, btnJugarS1, btnBorrarS1, grupoCrearS1);
        ConfigurarUI(2, txtNombreS2, btnJugarS2, btnBorrarS2, grupoCrearS2);
        ConfigurarUI(3, txtNombreS3, btnJugarS3, btnBorrarS3, grupoCrearS3);
    }

    private async void ConfigurarUI(int num, TextMeshProUGUI txt, GameObject btnJ, GameObject btnB, GameObject grupoC)
    {
        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).Child("slots").Child("slot" + num).GetValueAsync();

        if (snapshot.Exists && snapshot.Child("nombre").Exists)
        {
            txt.text = snapshot.Child("nombre").Value.ToString();
            btnJ.SetActive(true);
            btnB.SetActive(true);
            grupoC.SetActive(false);
            txt.gameObject.SetActive(true);
        }
        else
        {
            btnJ.SetActive(false);
            btnB.SetActive(false);
            grupoC.SetActive(true);
            txt.gameObject.SetActive(false);
        }
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    public async void CrearPartida(int num)
    {
        // Leemos el nombre de usuario registrado desde Firebase
        DataSnapshot snapshot = await dbReference.Child("usuarios").Child(userId).Child("nombreUsuario").GetValueAsync();
        string nombreUsuario = snapshot.Exists ? snapshot.Value.ToString() : "Comandante";

        // Determinamos el nombre del slot: usamos el input si tiene texto, sino generamos uno único
        string nombreSlot = "";
        TMP_InputField input = null;
        if (num == 1) input = inputS1;
        else if (num == 2) input = inputS2;
        else if (num == 3) input = inputS3;

        if (input != null && !string.IsNullOrEmpty(input.text))
        {
            nombreSlot = input.text;
        }
        else
        {
            nombreSlot = nombreUsuario + " - Slot " + num;
        }

        // Guardamos el nombre del slot y el nombre de usuario en Firebase
        await dbReference.Child("usuarios").Child(userId).Child("slots").Child("slot" + num).Child("nombre").SetValueAsync(nombreSlot);
        
        // Apuntamos en nuestro "Puente" en qué slot estamos jugando
        PartidaActual.SlotSeleccionado = "slot" + num;

        // ¡Nos vamos directos al Cómic!
        SceneManager.LoadScene(escenaNuevaPartida);
    }

    public void Jugar(int num)
    {
        // El jugador entra a una partida que ya existía
        PartidaActual.SlotSeleccionado = "slot" + num;
        
        // ¡Nos vamos directos al selector de niveles (o al nivel que quieras)!
        SceneManager.LoadScene(escenaCargarPartida);
    }

    public void VolverAlMenuPrincipal()
    {
        if (panelSelectorPartidas != null)
        {
            Debug.Log("Cerrando Panel_SelectorPartidas y regresando al menú principal.");
            panelSelectorPartidas.SetActive(false);
        }
        else
        {
            Debug.LogError("GestorSlots: panelSelectorPartidas no está asignado.");
        }
    }

    public void Borrar(int num)
    {
        dbReference.Child("usuarios").Child(userId).Child("slots").Child("slot" + num).RemoveValueAsync();
        ActualizarTodosLosSlots();
    }
}