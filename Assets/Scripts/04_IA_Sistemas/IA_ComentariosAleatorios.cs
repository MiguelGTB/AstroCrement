using UnityEngine;
using System.Collections.Generic;

public class IA_ComentariosAleatorios : MonoBehaviour
{
    // Intervalos definidos para la aleatoriedad en la emisión de mensajes.
    [Header("Configuración")]
    public float tiempoMinimo = 60f;
    public float tiempoMaximo = 180f;

    // Lista de mensajes precargados para la interacción.
    [Header("Posibles frases")]
    public List<string> frasesAleatorias = new List<string>();

    // Marca temporal para el próximo evento de ejecución.
    private float proximoComentario;

    void Start()
    {
        // Inicializa el primer intervalo de tiempo al comenzar la escena.
        CalcularSiguienteTiempo();
    }

    void Update()
    {
        // Verifica si se ha alcanzado el umbral temporal para emitir un comentario.
        if (Time.time >= proximoComentario)
        {
            SoltarComentario();
            CalcularSiguienteTiempo();
        }
    }

    // Calcula de forma aleatoria el siguiente momento de ejecución dentro del rango configurado.
    void CalcularSiguienteTiempo()
    {
        proximoComentario = Time.time + Random.Range(tiempoMinimo, tiempoMaximo);
    }

    // Selecciona y envía una frase aleatoria al gestor de síntesis de voz.
    void SoltarComentario()
    {
        // Valida que existan frases disponibles y que el gestor de voz sea accesible.
        if (frasesAleatorias.Count > 0 && TextToSpeechManager.Instance != null)
        {
            int indice = Random.Range(0, frasesAleatorias.Count);
            TextToSpeechManager.Instance.Hablar(frasesAleatorias[indice]);
        }
    }
}