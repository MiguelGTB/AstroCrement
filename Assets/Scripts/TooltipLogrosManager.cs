using UnityEngine;
using TMPro;

public class TooltipLogrosManager : MonoBehaviour
{
    public static TooltipLogrosManager Instance;

    [Header("UI del Tooltip")]
    public GameObject panelTooltip;
    public TextMeshProUGUI txtNombre;
    public TextMeshProUGUI txtDescripcion;
    public TextMeshProUGUI txtProgreso;

    private bool visible = false;

    void Awake()
    {
        Instance = this;
        panelTooltip.SetActive(false); // Empezamos ocultos
    }

    void Update()
    {
        if (visible)
        {
            // 1. Obtenemos la posici�n del mouse
            Vector3 mousePos = Input.mousePosition;

            // 2. Le damos un margen (Offset)
            // X: +20 (a la derecha) | Y: -20 (un poco abajo)
            Vector3 offset = new Vector3(20, -20, 0);

            // 3. Aplicamos la posici�n
            panelTooltip.transform.position = mousePos + offset;

            // OPCIONAL: Si el panel se sale por la derecha de la pantalla, 
            // puedes ajustar el pivote del RectTransform a (0, 1) en Unity.
        }
    }

    public void Mostrar(string nombre, string desc, string progreso)
    {
        txtNombre.text = nombre;
        txtDescripcion.text = desc;
        txtProgreso.text = progreso;

        panelTooltip.SetActive(true);
        visible = true;
    }

    public void Ocultar()
    {
        panelTooltip.SetActive(false);
        visible = false;
    }
}