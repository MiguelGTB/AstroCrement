using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;

    [Header("UI del Login")]
    public GameObject panelLogin;
    public TMP_InputField inputUsername;
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public TextMeshProUGUI textoFeedback;

    // Variables de Firebase
    private FirebaseAuth auth;
    public FirebaseUser UserActual; // Aquí guardaremos quién está jugando

    // Nombre de usuario para referencia en el juego
    public static string NombreUsuario;

    void Awake() 
    {
        Instance = this;
    }

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
        
        // 1. Comprobamos que el SDK de Firebase está listo para funcionar
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

    // --- BOTÓN REGISTRARSE ---
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
            // Esperamos a que Google cree la cuenta
            AuthResult resultado = await auth.CreateUserWithEmailAndPasswordAsync(inputEmail.text, inputPassword.text);
            UserActual = resultado.User;

            // Guardamos el nombre de usuario para referencia
            NombreUsuario = inputUsername.text;
            await Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference.
                Child("usuarios").Child(UserActual.UserId).Child("nombreUsuario").SetValueAsync(NombreUsuario);

            textoFeedback.text = "¡Bienvenido, Comandante " + inputUsername.text + "!";

            Invoke("IrAlMenuPrincipal", 1.5f);

        }
        catch (Exception e)
        {
            textoFeedback.text = "Error al registrar. (La contraseña debe tener 6+ caracteres)";
            Debug.LogWarning(e);
        }
    }

    // --- BOTÓN INICIAR SESIÓN ---
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
            // Esperamos a que Google valide la cuenta
            AuthResult resultado = await auth.SignInWithEmailAndPasswordAsync(inputEmail.text, inputPassword.text);
            UserActual = resultado.User;

            // Leemos el nombre de usuario desde Firebase
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

    private void IrAlMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}