using UnityEngine;

public class CreditosController : MonoBehaviour
{
    public CanvasGroup fondoNegro;
    public RectTransform panelCreditos;
    public float velocidad = 50f;
    public float tiempoFade = 0.5f;
    public float limiteSuperiorY = 1200f;

    private Vector2 posicionInicial;
    private bool activo = false;

    void Awake()
    {
        posicionInicial = panelCreditos.anchoredPosition;
        fondoNegro.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Mostrar()
    {
        gameObject.SetActive(true);
        panelCreditos.anchoredPosition = posicionInicial;
        activo = true;
        StartCoroutine(FadeIn());
    }

    public void Ocultar()
    {
        activo = false;
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        if (!activo) return;

        panelCreditos.anchoredPosition += Vector2.up * velocidad * Time.deltaTime;

        if (panelCreditos.anchoredPosition.y >= limiteSuperiorY)
            activo = false;
    }

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
