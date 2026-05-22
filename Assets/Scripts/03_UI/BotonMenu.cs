using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonMenu : MonoBehaviour
{
    public void IrAlMenuPrincipal()
    {
        // Forzamos el guardado de la partido justo antes de salirnos al menu principal
        DatabaseManager db = FindObjectOfType<DatabaseManager>();
        if(db != null)
        {
            db.GuardarPartidaEnNube();
        }

        SceneManager.LoadScene("MenuPrincipal");
    }
}