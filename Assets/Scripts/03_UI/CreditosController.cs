using UnityEngine;

public class CreditosController : MonoBehaviour
{
    // Referencias a componentes de UI para gestionar la opacidad y el desplazamiento.
    public CanvasGroup fondoNegro;
    public RectTransform panelCreditos;
    
    // Parámetros de control para la animación de los créditos.
    public float velocidad = 50f;
    public float tiempoFade = 0.5f;
    public float limiteSuperiorY = 1200f;

    // Estado interno para la gestión de la animación y posición original.
    private Vector2 posicionInicial;
    private bool activo = false;

    // Inicializa el estado del componente ocultando la UI y guardando la posición inicial.
    void Awake()
    {
        posicionInicial = panelCreditos.anchoredPosition;
        fondoNegro.alpha = 0;
        gameObject.SetActive(false);
    }

    // Activa la visualización de los créditos y lanza la rutina de entrada.
    public void Mostrar()
    {
        gameObject.SetActive(true);
        panelCreditos.anchoredPosition = posicionInicial;
        activo = true;
        StartCoroutine(FadeIn());
    }

    // Inicia el proceso de ocultación de los créditos.
    public void Ocultar()
    {
        activo = false;
        StartCoroutine(FadeOut());
    }

    // Actualiza la posición del panel de créditos en cada frame durante la fase activa.
    void Update()
    {
        if (!activo) return;

        // Desplaza el panel verticalmente.
        panelCreditos.anchoredPosition += Vector2.up * velocidad * Time.deltaTime;

        // Finaliza el desplazamiento si se alcanza el límite superior definido.
        if (panelCreditos.anchoredPosition.y >= limiteSuperiorY)
            activo = false;
    }

    // Rutina para realizar la transición de opacidad de entrada (Fade In).
    private System.Collections.IEnumerator FadeIn()
    {
        float t = 0;
        while (t < tiempoFade)
        {
            t += Time.deltaTime;
            fondoNegro.alpha = Mathf.Lerp(0, 1, t / tiempoFade);
            yield return null;
        }
    }

    // Rutina para realizar la transición de opacidad de salida (Fade Out) y resetear el objeto.
    private System.Collections.IEnumerator FadeOut()
    {
        float t = 0;
        while (t < tiempoFade)
        {
            t += Time.deltaTime;
            fondoNegro.alpha = Mathf.Lerp(1, 0, t / tiempoFade);
            yield return null;
        }

        panelCreditos.anchoredPosition = posicionInicial;
        gameObject.SetActive(false);
    }
}