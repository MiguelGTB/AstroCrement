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
        if (user != null)
        {
            userId = user.UserId;
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            panelSelectorPartidas.SetActive(true);
            ActualizarTodosLosSlots();
        }
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

    public void CrearPartida(int num)
    {
        string nombre = "";
        if (num == 1) nombre = inputS1.text;
        if (num == 2) nombre = inputS2.text;
        if (num == 3) nombre = inputS3.text;

        if (string.IsNullOrEmpty(nombre)) nombre = "Comandante " + num;

        // ESTA ES LA RUTA CORRECTA: Guardamos el nombre dentro del slot específico
        // Lo guardamos en dos sitios para que sea fácil de leer:

        // 1. Para que el selector de slots lo encuentre rápido:
        dbReference.Child("usuarios").Child(userId).Child("slots").Child("slot" + num).Child("nombre").SetValueAsync(nombre);

        // 2. IMPORTANTE: Para que el sistema de carga de PlayerData lo encuentre:
        dbReference.Child("usuarios").Child(userId).Child("slots").Child("slot" + num).Child("datos").Child("nombreUsuario").SetValueAsync(nombre);

        PartidaActual.SlotSeleccionado = "slot" + num;
        SceneManager.LoadScene(escenaNuevaPartida);
    }

    public void Jugar(int num)
    {
        // El jugador entra a una partida que ya existía
        PartidaActual.SlotSeleccionado = "slot" + num;
        
        // ¡Nos vamos directos al selector de niveles (o al nivel que quieras)!
        SceneManager.LoadScene(escenaCargarPartida);
    }

    public void Borrar(int num)
    {
        dbReference.Child("usuarios").Child(userId).Child("slots").Child("slot" + num).RemoveValueAsync();
        ActualizarTodosLosSlots();
    }
}