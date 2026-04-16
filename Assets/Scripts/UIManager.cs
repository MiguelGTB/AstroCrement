using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public EconomyManager economy;
    public TiendaManager shop;

    public TextMeshProUGUI textoDineroUI;
    public TextMeshProUGUI textoPasivoUI;

    // Nombre de las Compras
    public string[] nombresInstalaciones = new string[15];
    // Aquí arrastrarás los 15 textos de tus botones
    public TextMeshProUGUI[] textosBotones = new TextMeshProUGUI[15];
    public TextMeshProUGUI[] textosNiveles;
    public Button[] botones;

    [Header("Pestañas")]
    public Image pestanaCompras;
    public Image pestanaMejoras;
    public Color colorActivo;
    public Color colorInactivo;


    [Header("Paneles")]
    public GameObject panelCompras;
    public GameObject panelMejoras;

    public void ActualizarInterfaz()
    {
        if (textoDineroUI != null)
            textoDineroUI.text = "Polvo Estelar: " + economy.dineroActual;

        if (textoPasivoUI != null)
            textoPasivoUI.text = economy.dineroPorSeg + " PE/s";

        // Actualizamos los 15 botones
        for (int i = 0; i < 15; i++)
        {
            if (textosBotones[i] != null)
            {
                if (economy.nivelesCompras[i] < 100)
                {
                    int precioActual = shop.preciosBase[i] * (economy.nivelesCompras[i] + 1);
                    textosBotones[i].text = nombresInstalaciones[i] + " (" + economy.nivelesCompras[i] + ")\nCoste: " + precioActual + " PE";

                    botones[i].interactable = true;
                    botones[i].image.color = Color.white;
                }
                else
                {
                    textosBotones[i].text = nombresInstalaciones[i] + " (100)\n<color=#7C7C7C>MAX</color>";

                    botones[i].interactable = false;

                    botones[i].image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }

                if(textosNiveles.Length > i && textosNiveles[i] != null)
                {
                    textosNiveles[i].text = economy.nivelesCompras[i].ToString();
                }
            }
        }
    }

    public void AbrirPestanaCompras()
    { 
        panelCompras.SetActive(true);
        panelMejoras.SetActive(false);

        pestanaCompras.color = colorActivo;
        pestanaMejoras.color = colorInactivo;
    }
    public void AbrirPestanaMejoras()
    {
        panelCompras.SetActive(false);
        panelMejoras.SetActive(true);

        pestanaCompras.color = colorInactivo;
        pestanaMejoras.color = colorActivo;
    }
}