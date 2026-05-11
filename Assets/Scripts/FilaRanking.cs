using UnityEngine;
using TMPro;

public class FilaRanking : MonoBehaviour
{
    public TextMeshProUGUI textoPuesto;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoPuntos;

    // Esta función la llama el RankingManager
    public void Configurar(int puesto, string nombre, string puntos)
    {
        textoPuesto.text = puesto.ToString();
        textoNombre.text = nombre;
        textoPuntos.text = puntos;
    }
}