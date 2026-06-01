using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    // Patrón Singleton para mantener el gestor de autenticación accesible globalmente.
    public static AuthManager Instance;

    [Header("UI del Login")]
    public GameObject panelLogin;
    public TMP_InputField inputUsername;
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public TextMeshProUGUI textoFeedback;

    // Instancia de autenticación de Firebase y objeto de usuario autenticado.
    private FirebaseAuth auth;
    public FirebaseUser UserActual; 

    // Almacena localmente el nombre de usuario para referencia en el juego.
    public static string NombreUsuario;

    void Awake() 
    {
        Instance = this;
    }

    // Inicializa la conexión con los servicios de Firebase de forma asíncrona.
    async void Start()
    {
        if (textoFeedback != null)
        {
            textoFeedback.text = "Conectando con el servidor...";
        }
        else
        {
            Debug.LogWarning("AuthManager: textoFeedback no está asignado en el inspector.");
        }
        
        // Verifica y resuelve las dependencias necesarias de Firebase SDK.
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            if (textoFeedback != null)
            {
                textoFeedback.text = "Servidor online. Inicia sesión o regístrate.";
            }
        }
        else
        {
            if (textoFeedback != null)
            {
                textoFeedback.text = "Error crítico de conexión.";
            }
            Debug.LogError("No se pudo resolver Firebase: " + dependencyStatus);
        }
    }

    // Procesa el registro de nuevos usuarios en Firebase Auth y crea el nodo en Realtime Database.
    public async void RegistrarUsuario()
    {
        if (string.IsNullOrEmpty(inputEmail.text) || string.IsNullOrEmpty(inputPassword.text))
        {
            textoFeedback.text = "Por favor, rellena todos los campos.";
            return;
        }

        textoFeedback.text = "Creando perfil de comandante...";
        try
        {
            // Ejecuta la creación de cuenta mediante correo y contraseña.
            AuthResult resultado = await auth.CreateUserWithEmailAndPasswordAsync(inputEmail.text, inputPassword.text);
            UserActual = resultado.User;

            // Persiste el nombre de usuario en la base de datos vinculada al UID del usuario.
            NombreUsuario = inputUsername.text;
            await Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference.
                Child("usuarios").Child(UserActual.UserId).Child("nombreUsuario").SetValueAsync(NombreUsuario);

            textoFeedback.text = "¡Bienvenido, Comandante " + inputUsername.text + "!";

            // Retrasa la transición a la escena de menú principal tras el éxito.
            Invoke("IrAlMenuPrincipal", 1.5f);
        }
        catch (Exception e)
        {
            textoFeedback.text = "Error al registrar. (La contraseña debe tener 6+ caracteres)";
            Debug.LogWarning(e);
        }
    }

    // Valida las credenciales del usuario y recupera su perfil desde Firebase.
    public async void IniciarSesion()
    {
        if (string.IsNullOrEmpty(inputEmail.text) || string.IsNullOrEmpty(inputPassword.text))
        {
            textoFeedback.text = "Por favor, rellena todos los campos.";
            return;
        }

        textoFeedback.text = "Comprobando credenciales...";
        try
        {
            // Valida el inicio de sesión contra los servicios de Google Firebase.
            AuthResult resultado = await auth.SignInWithEmailAndPasswordAsync(inputEmail.text, inputPassword.text);
            UserActual = resultado.User;

            // Consulta el nombre de usuario asociado al UID en la base de datos.
            var snapshot = await Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference.
                Child("usuarios").Child(UserActual.UserId).Child("nombreUsuario").GetValueAsync();
            NombreUsuario = snapshot.Exists ? snapshot.Value.ToString() : "Comandante Desconocido";

            textoFeedback.text = "¡Sesión iniciada correctamente!";
            Invoke("IrAlMenuPrincipal", 1.5f);
        }
        catch (Exception e)
        {
            textoFeedback.text = "Error. Comprueba tu correo y contraseña.";
            Debug.LogWarning(e);
        }
    }

    // Gestiona el cambio de escena hacia el menú principal tras la autenticación.
    private void IrAlMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


    
    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego desde la pantalla de Login...");
        Application.Quit(); // Cierra el juego compilado

        // Esto hace que el botón también funcione mientras pruebas en el editor de Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}