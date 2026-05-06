using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReencarnacionManager : MonoBehaviour
{
    public EconomyManager economy;
    public Button botonReencarnar;

    [Header("Configuración por Nivel")]
    public GameObject objetoCandado;

    public int indiceNivelActual;
    public double[] requisitosDesbloqueo;

    void Update()
    {
        // Verificamos que el array tenga valores antes de intentar leerlo
        if (requisitosDesbloqueo == null || requisitosDesbloqueo.Length <= indiceNivelActual) return;

        double requerido = requisitosDesbloqueo[indiceNivelActual];

        // Ahora comprobamos el dinero
        if (economy.dineroActual >= requerido)
        {
            if (botonReencarnar != null) botonReencarnar.interactable = true;
            if (objetoCandado != null) objetoCandado.SetActive(false);
        }
        else
        {
            if (botonReencarnar != null) botonReencarnar.interactable = false;
            if (objetoCandado != null) objetoCandado.SetActive(true);
        }
    }

    public void EjectuarReencarnacion()
    {
        Debug.Log("Botón pulsado. Intentando cargar: " + "Seleccion_Niveles");

        // 1. Desbloqueamos el índice del siguiente nivel
        int siguienteNivel = indiceNivelActual + 1;

        // Guardamos el progreso del nivel más alto alcanzado
        int maximoAlcanzado = PlayerPrefs.GetInt("PlanetaActual", 0);
        if(siguienteNivel > maximoAlcanzado)
        {
            PlayerPrefs.SetInt("PlanetaActual", siguienteNivel);
            PlayerPrefs.Save();
        }

        // 2. Cargamos la escena de selección o el siguiente nivel directamente
        SceneManager.LoadScene("Seleccion_Niveles");

        economy.dineroActual = 0;
        for (int i = 0; i < 15; i++)
        {
            economy.nivelesCompras[i] = 0;
        }
    } 
}
