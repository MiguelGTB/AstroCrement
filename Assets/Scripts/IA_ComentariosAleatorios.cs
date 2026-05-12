using UnityEngine;
using System.Collections.Generic;

public class IA_ComentariosAleatorios : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoMinimo = 60f;
    public float tiempoMaximo = 180f;

    [Header("Posibles frases")]
    public List<string> frasesAleatorias = new List<string>();

    private float proximoComentario;

    void Start()
    {
        CalcularSiguienteTiempo();
    }

    void Update()
    {
        if (Time.time >= proximoComentario)
        {
            SoltarComentario();
            CalcularSiguienteTiempo();
        }
    }

    void CalcularSiguienteTiempo()
    {
        proximoComentario = Time.time + Random.Range(tiempoMinimo, tiempoMaximo);
    }

    void SoltarComentario()
    {
        if (frasesAleatorias.Count > 0 && TextToSpeechManager.Instance != null)
        {
            int indice = Random.Range(0, frasesAleatorias.Count);
            TextToSpeechManager.Instance.Hablar(frasesAleatorias[indice]);
        }
    }
}