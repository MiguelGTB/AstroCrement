using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource musicaSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Canción de Fondo (Todo el Juego)")]
    public AudioClip cancionJuego;

    [Header("Efectos de Sonido (SFX)")]
    public AudioClip efectoLogin;
    public AudioClip efectoPlaneta;
    public AudioClip efectoBoton;

    void Awake()
    {
        // Hacer que el AudioManager sea inmortal entre escenas
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
        // Reproducir la canción en bucle e impedir que se reinicie al cambiar de escena
        if (cancionJuego != null && musicaSource != null && !musicaSource.isPlaying)
        {
            musicaSource.clip = cancionJuego;
            musicaSource.loop = true;
            musicaSource.Play();
        }
    }

    // --- Métodos públicos para activar los sonidos ---
    public void PlayEfectoLogin()
    {
        if (efectoLogin != null) sfxSource.PlayOneShot(efectoLogin);
    }

    public void PlayEfectoPlaneta()
    {
        if (efectoPlaneta != null) sfxSource.PlayOneShot(efectoPlaneta);
    }

    public void PlayEfectoBotonGeneral()
    {
        if (efectoBoton != null) sfxSource.PlayOneShot(efectoBoton);
    }

}
