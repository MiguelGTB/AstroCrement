using UnityEngine;

public class CreditosScroll : MonoBehaviour
{
    public float velocidad = 50f;
    public float puntoDeReinicioY = 2000f; // Altura a la que desaparece por arriba

    private RectTransform rectTexto;
    private Vector2 posicionInicial;

    void Awake()
    {
        // Cogemos el componente de UI
        rectTexto = GetComponent<RectTransform>();
        posicionInicial = rectTexto.anchoredPosition;
    }

    void OnEnable()
    {
        // Cada vez que se abra el panel, el texto vuelve a su sitio
        if (rectTexto != null) rectTexto.anchoredPosition = posicionInicial;
    }

    void Update()
    {
        if (rectTexto == null) return;

        // Movemos el texto hacia arriba
        rectTexto.anchoredPosition += Vector2.up * velocidad * Time.deltaTime;

        // Si el texto ya ha subido demasiado, vuelve abajo (bucle)
        if (rectTexto.anchoredPosition.y > puntoDeReinicioY)
        {
            rectTexto.anchoredPosition = posicionInicial;
        }
    }
}