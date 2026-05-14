using UnityEngine;
using TMPro;

public class ArbolManager : MonoBehaviour
{
    public TextMeshProUGUI textoMonedasPrestigio;
    private MejoraPrestigio[] todasLasMejoras;

    void Start()
    {
        todasLasMejoras = GetComponentsInChildren<MejoraPrestigio>();
        ActualizarTodoElArbol();
    }

    public void ActualizarTodoElArbol()
    {
        PlayerData datos = DatabaseManager.Instance.datosCargados;
        textoMonedasPrestigio.text = "Monedas Celestiales: " + datos.monedasPrestigio.ToString("F0");

        // Actualizamos cada botón
        foreach (var mejora in todasLasMejoras)
        {
            mejora.RefrescarEstadoVisual(datos);
        }
    }

    public void VolverAlJuego()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Selector_Niveles");
    }
}