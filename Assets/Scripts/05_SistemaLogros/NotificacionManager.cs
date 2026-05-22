using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NotificacionManager : MonoBehaviour
{
    public static NotificacionManager Instance;

    [Header("UI")]
    public GameObject panelNotificacion;
    public Image imagenIcono;
    public TextMeshProUGUI textoNombre;

    [Header("Configuración")]
    public float duracion = 3f;
    public float velocidadFade = 1f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = panelNotificacion.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panelNotificacion.AddComponent<CanvasGroup>();

        panelNotificacion.SetActive(false);
    }

    public void MostrarNotificacion(string nombreLogro, Sprite icono)
    {
        StopAllCoroutines();

        if (icono != null) imagenIcono.sprite = icono;
        textoNombre.text = nombreLogro;

        panelNotificacion.SetActive(true);
        StartCoroutine(AnimarNotificacion());
    }

    private IEnumerator AnimarNotificacion()
    {
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * velocidadFade;
            yield return null;
        }

        yield return new WaitForSeconds(duracion);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * velocidadFade;
            yield return null;
        }

        panelNotificacion.SetActive(false);
    }
}