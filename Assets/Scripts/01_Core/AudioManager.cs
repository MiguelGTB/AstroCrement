using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Instancia estática para implementar el patrón Singleton y mantener persistencia entre escenas.
    public static AudioManager Instance;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource musicaSource;
    [SerializeField] public AudioSource sfxSource;

    [Header("Canción de Fondo")]
    public AudioClip cancionJuego;

    [Header("Efectos de Sonido")]
    public AudioClip efectoLogin;
    public AudioClip efectoPlaneta;
    public AudioClip efectoBoton;

    // Propiedades públicas para acceso externo a las fuentes de audio.
    public AudioSource MusicaSource => musicaSource;
    public AudioSource SfxSource => sfxSource;

    void Awake()
    {
        // Verifica la existencia de una instancia previa para evitar duplicidad y mantener el objeto tras cambios de escena.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            AsegurarFuentes();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Recupera y aplica las preferencias de volumen almacenadas en el sistema local.
        float volGeneral = PlayerPrefs.GetFloat("VolumenGeneral", 100f);
        float volEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 100f);

        if (musicaSource != null) musicaSource.volume = volGeneral / 100f;
        if (sfxSource != null) sfxSource.volume = volEfectos / 100f;

        // Inicia la reproducción de la música de fondo si el componente está configurado.
        if (cancionJuego != null && musicaSource != null && !musicaSource.isPlaying)
        {
            musicaSource.clip = cancionJuego;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    // Reproduce el efecto sonoro asociado al evento de inicio de sesión.
    public void PlayEfectoLogin()
    {
        if (efectoLogin != null && sfxSource != null)
            sfxSource.PlayOneShot(efectoLogin);
    }

    // Reproduce el efecto sonoro asociado a la interacción con planetas.
    public void PlayEfectoPlaneta()
    {
        if (efectoPlaneta != null && sfxSource != null)
            sfxSource.PlayOneShot(efectoPlaneta);
    }

    // Reproduce el efecto sonoro asociado a clics en botones generales.
    public void PlayEfectoBotonGeneral()
    {
        if (efectoBoton != null && sfxSource != null)
            sfxSource.PlayOneShot(efectoBoton);
    }

    // Valida y asigna los componentes AudioSource necesarios, creándolos si no existen.
    private void AsegurarFuentes()
    {
        AudioSource[] fuentes = GetComponents<AudioSource>();

        if (fuentes.Length == 0)
        {
            musicaSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
            return;
        }
        if (fuentes.Length == 1)
        {
            musicaSource = fuentes[0];
            sfxSource = gameObject.AddComponent<AudioSource>();
            return;
        }

        musicaSource = fuentes[0];
        sfxSource = fuentes[1];
    }
}