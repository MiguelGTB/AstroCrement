using UnityEngine;
using System.Collections;

public class RebotePlaneta : MonoBehaviour
{
    // Referencia al sistema de partículas activado al interactuar con el objeto.
    public ParticleSystem particulasClic;

    // Almacena la escala original y los parámetros físicos de la animación de rebote.
    private Vector3 tamanoOriginal;
    private float velocidadRebote = 10f;
    private float cantidadEncogimiento = 0.9f;

    // Inicializa la escala base del objeto al comenzar la escena.
    void Start()
    {
        tamanoOriginal = transform.localScale;
    }

    // Ejecuta la respuesta visual y sonora ante la interacción del usuario.
    public void PlayClick()
    {
        // Aplica el factor de escala reducido al objeto.
        transform.localScale = tamanoOriginal * cantidadEncogimiento;

        // Dispara el sistema de partículas si se encuentra configurado.
        if (particulasClic != null)
        {
            particulasClic.Play();
        }

        // Solicita al gestor de audio la reproducción del efecto sonoro correspondiente.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEfectoPlaneta();
        }
    }

    // Interpola la escala actual de vuelta a la escala original en cada frame.
    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            tamanoOriginal,
            Time.deltaTime * velocidadRebote
        );
    }
}