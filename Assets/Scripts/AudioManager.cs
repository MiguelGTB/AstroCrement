using UnityEngine;

public class AudioManager : MonoBehaviour
{
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

    public AudioSource MusicaSource => musicaSource;
    public AudioSource SfxSource => sfxSource;

    void Awake()
    {
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
        // Aplicar volúmenes guardados al arrancar
        float volGeneral = PlayerPrefs.GetFloat("VolumenGeneral", 100f);
        float volEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 100f);

        if (musicaSource != null) musicaSource.volume = volGeneral / 100f;
        if (sfxSource != null) sfxSource.volume = volEfectos / 100f;

        // Reproducir música
        if (cancionJuego != null && musicaSource != null && !musicaSource.isPlaying)
        {
            musicaSource.clip = cancionJuego;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    public void PlayEfectoLogin()
    {
        if (efectoLogin != null && sfxSource != null)
            sfxSource.PlayOneShot(efectoLogin);
    }

    public void PlayEfectoPlaneta()
    {
        if (efectoPlaneta != null && sfxSource != null)
            sfxSource.PlayOneShot(efectoPlaneta);
    }

    public void PlayEfectoBotonGeneral()
    {
        if (efectoBoton != null && sfxSource != null)
            sfxSource.PlayOneShot(efectoBoton);
    }

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