using UnityEngine;

public class AbrirLogrosTeclado : MonoBehaviour
{
    // Arrastra el panel aquí en el Inspector una sola vez en el Prefab
    public GameObject panelLogros;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // Si está abierto lo cierra, si está cerrado lo abre
            panelLogros.SetActive(!panelLogros.activeSelf);
        }
    }
}