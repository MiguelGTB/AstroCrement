using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Mejora
{
    public string nombre;
    public double costePE;
    
    [Header("Requisitos para aparecer")]
    [Tooltip("Pon -1 para requisitos de Producción de Polvo Estelar")]
    public int idInstalacionRequisito; 
    public int cantidadRequisito;      

    [HideInInspector] public bool comprada = false;
    public GameObject botonAsociado;   
}

public class MejorasManager : MonoBehaviour
{
    public EconomyManager economy;
    public TiendaManager tienda; 
    public UIManager ui;
    
    public Mejora[] listaMejoras;

    void Start()
    {
        foreach (Mejora m in listaMejoras)
        {
            if (m.botonAsociado != null) 
            {
                m.botonAsociado.SetActive(false);
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < listaMejoras.Length; i++)
        {
            Mejora m = listaMejoras[i];

            if (!m.comprada && m.botonAsociado != null && !m.botonAsociado.activeSelf)
            {
                bool cumpleRequisito = false;

                // Si el ID es 0 o mayor, miramos cuántos edificios ha comprado
                if (m.idInstalacionRequisito >= 0)
                {
                    if (economy.nivelesCompras[m.idInstalacionRequisito] >= m.cantidadRequisito)
                        cumpleRequisito = true;
                }
                // Si el ID es -1, miramos cuánto Polvo Estelar POR SEGUNDO genera
                else if (m.idInstalacionRequisito == -1)
                {
                    if (economy.dineroPorSeg >= m.cantidadRequisito)
                        cumpleRequisito = true;
                }

                // Si cumple la condición, encendemos el botón
                if (cumpleRequisito)
                {
                    m.botonAsociado.SetActive(true);
                }
            }
        }
    }

    public void ComprarMejora(int indiceMejora)
    {
        Mejora m = listaMejoras[indiceMejora];

        if (!m.comprada)
        {
            // CALCULAMOS EL COSTE FINAL APLICANDO INGENIERÍA CUÁNTICA
            double costeFinal = m.costePE;

            if (DatabaseManager.Instance != null && DatabaseManager.Instance.datosCargados != null)
            {
                // Si en la lista global de prestigio está "ingenieria_cuantica", aplicamos el 50% de descuento
                if (DatabaseManager.Instance.datosCargados.mejorasPrestigioCompradas.Contains("ingenieria_cuantica"))
                {
                    costeFinal *= 0.5f; 
                }
            }

            // Pasamos a la economía el coste final ya recalculado con la rebaja
            if (economy.GastarDinero(costeFinal))
            {
                m.comprada = true;
                m.botonAsociado.SetActive(false); 

                TooltipMejorasManager.Instance.Ocultar();

                // APLICAR EL EFECTO DE LA MEJORA 
                int id = m.idInstalacionRequisito;

                if (id == 0)
                {
                    // Mejora del Láser
                    economy.dineroPorClic *= 2; 
                }
                else if (id > 0)
                {
                    // Mejora de Instalaciones automáticas
                    double beneficioAntiguo = tienda.beneficios[id];
                    int cantidadComprada = economy.nivelesCompras[id];
                    
                    tienda.beneficios[id] *= 2; 
                    economy.dineroPorSeg += (beneficioAntiguo * cantidadComprada);
                }
                else if (id == -1)
                {
                    // MEJORA GLOBAL DE POLVO ESTELAR
                    economy.dineroPorClic *= 5;
                    Debug.Log("¡Mejora global comprada!");
                }

                ui.ActualizarInterfaz();
            }
        }
    }
}