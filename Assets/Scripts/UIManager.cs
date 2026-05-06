using JetBrains.Annotations;
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


    [Header("Paneles")]
    public GameObject panelCompras;
    public GameObject panelMejoras;

    public void ActualizarInterfaz()
    {
        if (textoDineroUI != null)
            textoDineroUI.text = "Polvo Estelar: " + FormatearNumero(economy.dineroActual);

        if (textoPasivoUI != null)
            textoPasivoUI.text = FormatearNumero(economy.dineroPorSeg) + " PE/s";

        // Actualizamos los 15 botones
        for (int i = 0; i < 15; i++)
        {
            if (textosBotones[i] != null)
            {
                if (economy.nivelesCompras[i] < 100)
                {
                    double precioActual = shop.preciosBase[i] * Mathf.Pow(shop.multiplicadorPrecio, economy.nivelesCompras[i]);
                    textosBotones[i].text = nombresInstalaciones[i] + " (" + economy.nivelesCompras[i] + ")\nCoste: " + FormatearNumero(precioActual) + " PE";

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
    // Función para que no aparezcan los número gigantescos, y en vez de 1.000.000.000, aparezca 1MM
    public string FormatearNumero(double numero)
    {
        if (numero < 1000000)
        {
            return numero.ToString("N0").Replace(',', '.');
        }

        if (numero >= 1000000000000) return (numero / 1000000000000).ToString("F2") + "T";
        if (numero >= 1000000000) return (numero / 1000000000).ToString("F2") + "B";
        if (numero >= 1000000) return (numero / 1000000).ToString("F2") + "M";
        return numero.ToString("F0");
    }
}