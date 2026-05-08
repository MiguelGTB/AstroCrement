using UnityEngine;
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
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public TextMeshProUGUI textoFeedback;

    // Variables de Firebase
    private FirebaseAuth auth;
    public FirebaseUser UserActual; // Aquí guardaremos quién está jugando

    void Awake() 
    {
        Instance = this;
    }

    async void Start()
    {
        textoFeedback.text = "Conectando con el servidor...";
        
        // 1. Comprobamos que el SDK de Firebase está listo para funcionar
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            textoFeedback.text = "Servidor online. Inicia sesión o regístrate.";
        }
        else
        {
            textoFeedback.text = "Error crítico de conexión.";
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

        textoFeedback.text = "Registrando en la base de datos...";
        try
        {
            // Esperamos a que Google cree la cuenta
            AuthResult resultado = await auth.CreateUserWithEmailAndPasswordAsync(inputEmail.text, inputPassword.text);
            UserActual = resultado.User;
            textoFeedback.text = "¡Cuenta creada con éxito!";
            EmpezarJuego();
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
            textoFeedback.text = "¡Sesión iniciada correctamente!";
            EmpezarJuego();
        }
        catch (Exception e)
        {
            textoFeedback.text = "Error. Comprueba tu correo y contraseña.";
            Debug.LogWarning(e);
        }
    }

    private void EmpezarJuego()
    {
        // El jugador ha entrado con éxito, así que apagamos la pantalla de login
        panelLogin.SetActive(false);
        
        Debug.Log("¡El jugador con ID: " + UserActual.UserId + " ha entrado al juego!");
        
        // ¡EN EL PRÓXIMO PASO PONDREMOS AQUÍ LA DESCARGA DE LA PARTIDA!
    }
}