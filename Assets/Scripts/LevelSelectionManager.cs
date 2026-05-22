using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Tus Planetas 3D")]
    public MeshRenderer[] modelosPlanetas; 
    public Planeta3DSelector[] scriptsClic; 
    public GameObject[] iconosCandado;

    [Header("Nombres de Escenas")]
    public string[] nombresEscenasPlanetas; 
    
    [Header("Nombres Clave (Para Base de Datos)")]
    public string[] nombresClavePlanetas;

    private bool yaSeHanPintadoLosPlanetas = false;

    void Start()
    {
        // 1. APAGÓN INICIAL: Mientras esperamos a internet, apagamos todos por seguridad
        if (modelosPlanetas != null)
        {
            for (int i = 0; i < modelosPlanetas.Length; i++)
            {
                if (modelosPlanetas[i] == null) continue;
                modelosPlanetas[i].material.color = new Color(0.02f, 0.02f, 0.02f, 1f); 
                if (scriptsClic.Length > i && scriptsClic[i] != null) scriptsClic[i].enabled = false;
                if (iconosCandado.Length > i && iconosCandado[i] != null) iconosCandado[i].SetActive(true);
            }
        }
    }

    void Update()
    {
        // 2. EL RADAR: Espera pacientemente a que la Base de Datos exista y confirme la descarga
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.partidaCargadaConExito)
        {
            if (!yaSeHanPintadoLosPlanetas)
            {
                yaSeHanPintadoLosPlanetas = true;
                ActualizarEstadoPlanetas(); 
            }
        }
    }

    public void ActualizarEstadoPlanetas()
    {
        // Cogemos el dato real de Firebase
        int nivelMaximoDesbloqueado = DatabaseManager.Instance.datosCargados.planetasDesbloqueados;

        if (modelosPlanetas != null)
        {
            for (int i = 0; i < modelosPlanetas.Length; i++)
            {
                if (modelosPlanetas[i] == null) continue;

                if (i <= nivelMaximoDesbloqueado)
                {
                    // --- PLANETA DESBLOQUEADO ---
                    modelosPlanetas[i].material.color = Color.white; 
                    if (scriptsClic.Length > i && scriptsClic[i] != null) scriptsClic[i].enabled = true; 
                    if (iconosCandado.Length > i && iconosCandado[i] != null) iconosCandado[i].SetActive(false); 
                }
                else
                {
                    // --- PLANETA BLOQUEADO ---
                    modelosPlanetas[i].material.color = new Color(0.1f, 0.1f, 0.1f, 1f); 
                    if (scriptsClic.Length > i && scriptsClic[i] != null) scriptsClic[i].enabled = false; 
                    if (iconosCandado.Length > i && iconosCandado[i] != null) iconosCandado[i].SetActive(true); 
                }
            }
        }
    }

    public void ViajarAlPlaneta(int indicePlaneta)
    {
        int nivelMaximoDesbloqueado = 0;
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.datosCargados != null)
        {
            nivelMaximoDesbloqueado = DatabaseManager.Instance.datosCargados.planetasDesbloqueados;
        }

        if (indicePlaneta > nivelMaximoDesbloqueado)
        {
            Debug.LogWarning("¡Planeta bloqueado! Aún no puedes viajar aquí.");
            return; 
        }

        if (indicePlaneta < nombresEscenasPlanetas.Length)
        {
            if (nombresClavePlanetas != null && nombresClavePlanetas.Length > indicePlaneta)
            {
                PartidaActual.MundoActual = nombresClavePlanetas[indicePlaneta];
            }

            SceneManager.LoadScene(nombresEscenasPlanetas[indicePlaneta]);
        }
    }
}