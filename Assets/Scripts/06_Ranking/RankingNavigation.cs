using UnityEngine;
using UnityEngine.SceneManagement;

public class RankingNavigation : MonoBehaviour
{
    public void VolverAlMenu()
    {
        Debug.Log("Regresando al menú principal...");
        SceneManager.LoadScene("MenuPrincipal");
    }
}