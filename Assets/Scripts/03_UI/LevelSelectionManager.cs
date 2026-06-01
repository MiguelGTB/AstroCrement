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
        // Al arrancar la escena, por defecto la Luna SIEMPRE tiene que estar visible y jugable.
        // El resto los apago por seguridad hasta que Firebase me confirme qué tengo desbloqueado.
        if (modelosPlanetas != null)
        {
            for (int i = 0; i < modelosPlanetas.Length; i++)
            {
                if (modelosPlanetas[i] == null) continue;

                if (i == 0) 
                {
                    // La Luna Siempre encendida, sin candado y con el clic activado
                    modelosPlanetas[i].material.color = Color.white; 
                    if (scriptsClic.Length > i && scriptsClic[i] != null) scriptsClic[i].enabled = true;
                    if (iconosCandado.Length > i && iconosCandado[i] != null) iconosCandado[i].SetActive(false);
                }
                else 
                {
                    // El resto: Oscuros, bloqueados y con el candado puesto de momento
                    modelosPlanetas[i].material.color = new Color(0.1f, 0.1f, 0.1f, 1f); 
                    if (scriptsClic.Length > i && scriptsClic[i] != null) scriptsClic[i].enabled = false;
                    if (iconosCandado.Length > i && iconosCandado[i] != null) iconosCandado[i].SetActive(true);
                }
            }
        }
    }

    void Update()
    {
        // En cada frame vigilo si la base de datos ya me ha traído mis datos de la nube.
        // Si ya han llegado y aún no he repintado los planetas con mi progreso real, lo hago ahora mismo.
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.partidaCargadaConExito && !yaSeHanPintadoLosPlanetas)
        {
            int maxDesbloqueado = DatabaseManager.Instance.datosCargados.planetasDesbloqueados;

            for (int i = 0; i < modelosPlanetas.Length; i++)
            {
                if (modelosPlanetas[i] == null) continue;

                if (i <= maxDesbloqueado)
                {
                    // Planeta desbloqueado: Lo pinto normal y le quito el candado
                    modelosPlanetas[i].material.color = Color.white; 
                    if (scriptsClic.Length > i && scriptsClic[i] != null) scriptsClic[i].enabled = true;
                    if (iconosCandado.Length > i && iconosCandado[i] != null) iconosCandado[i].SetActive(false);
                }
                else
                {
                    // Planeta bloqueado: Lo dejo apagado para no hacer spoilers
                    modelosPlanetas[i].material.color = new Color(0.1f, 0.1f, 0.1f, 1f); 
                    if (scriptsClic.Length > i && scriptsClic[i] != null) scriptsClic[i].enabled = false;
                    if (iconosCandado.Length > i && iconosCandado[i] != null) iconosCandado[i].SetActive(true);
                }
            }
            
            // Marco que ya he hecho este trabajo para que no se repita 60 veces por segundo y ahorremos rendimiento.
            yaSeHanPintadoLosPlanetas = true;
        }
    }

    public void ViajarAlPlaneta(int indicePlaneta)
    {
        // Antes de viajar, me aseguro de que el jugador no esté haciendo trampas intentando ir a un planeta bloqueado.
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

        // Si todo es legal, configuro a qué planeta vamos en la variable estática y cargo la escena
        if (indicePlaneta < nombresEscenasPlanetas.Length)
        {
            if (nombresClavePlanetas != null && nombresClavePlanetas.Length > indicePlaneta)
            {
                PartidaActual.MundoActual = nombresClavePlanetas[indicePlaneta];
            }
            
            Debug.Log("Viajando a: " + nombresEscenasPlanetas[indicePlaneta]);
            SceneManager.LoadScene(nombresEscenasPlanetas[indicePlaneta]);
        }
    }
}