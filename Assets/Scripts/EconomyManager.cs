using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public double dineroActual = 0;
    public double dineroPorClic = 1;
    public double dineroPorSeg = 0;
    public double dineroTotal = 0;

    public int[] nivelesCompras = new int[15];

    public UIManager ui;
    private float temporizador = 0f;
    private float temporizadorDrones = 0f; // Temporizador para el Auto-clic

    void Start()
    {
        // --- NUEVO: RECONECTAMOS LA BASE DE DATOS INMORTAL A ESTA NUEVA LUNA ---
        if (DatabaseManager.Instance != null)
        {
            MejorasManager mm = FindObjectOfType<MejorasManager>(); 
            DatabaseManager.Instance.ReconectarEscenaActual(this, mm);
        }
    }

    void Update()
    {
        // 1. EL FARMEO PASIVO NORMAL (Dinero por segundo)
        if (dineroPorSeg > 0)
        {
            temporizador += Time.deltaTime;

            if (temporizador >= 1f)
            {
                SumarPasivo();
                temporizador = 0f;
            }
        }

        // 2. MEJORA: ENJAMBRE DE DRONES (Auto-clic de 5 veces por segundo)
        if (DatabaseManager.Instance != null && DatabaseManager.Instance.datosCargados != null)
        {
            if (DatabaseManager.Instance.datosCargados.mejorasPrestigioCompradas.Contains("enjambre_drones"))
            {
                temporizadorDrones += Time.deltaTime;
                
                if (temporizadorDrones >= 0.2f)
                {
                    SumarClick(); // Los drones hacen clic por ti automáticamente
                    temporizadorDrones = 0f;
                }
            }
        }
    }

    // --- EL CLIC Y SUS MULTIPLICADORES ---
    public void SumarClick()
    {
        double clickFinal = dineroPorClic;

        if (DatabaseManager.Instance != null && DatabaseManager.Instance.datosCargados != null)
        {
            PlayerData datos = DatabaseManager.Instance.datosCargados;

            // MEJORA: Pulso Celestial (x2 fijo)
            if (datos.mejorasPrestigioCompradas.Contains("pulso_celestial")) 
            {
                clickFinal *= 2; 
            }

            // MEJORA: Red de Comercio (+50% extra por cada planeta desbloqueado)
            if (datos.mejorasPrestigioCompradas.Contains("red_comercio"))
            {
                double bonusSinergia = 1.0 + (datos.planetasDesbloqueados * 0.50);
                clickFinal *= bonusSinergia;
            }

            // MEJORA: Tormenta de Críticos y Sobrecarga del Núcleo (10% probabilidad de x10)
            if (datos.mejorasPrestigioCompradas.Contains("tormenta_criticos") || datos.mejorasPrestigioCompradas.Contains("sobrecarga_nucleo"))
            {
                if (Random.value <= 0.10f) 
                {
                    clickFinal *= 10;
                    Debug.Log("¡GOLPE CRÍTICO CELESTIAL!");
                }
            }
        }

        AnadirDinero(clickFinal);
        if (ui != null) ui.ActualizarInterfaz();
    }

    // --- EL PASIVO Y SUS MULTIPLICADORES ---
    public void SumarPasivo()
    {
        double pasivoFinal = dineroPorSeg;

        if (DatabaseManager.Instance != null && DatabaseManager.Instance.datosCargados != null)
        {
            PlayerData datos = DatabaseManager.Instance.datosCargados;

            // MEJORA: Armonía / Baterías / Refinería (Multiplicadores al dinero por segundo)
            if (datos.mejorasPrestigioCompradas.Contains("armonia_monetaria") || 
                datos.mejorasPrestigioCompradas.Contains("baterias_eter") ||
                datos.mejorasPrestigioCompradas.Contains("refineria_polvo"))
            {
                pasivoFinal *= 1.5; 
            }
            
            // MEJORA: Red de Comercio (La sinergia de planetas también afecta al pasivo)
            if (datos.mejorasPrestigioCompradas.Contains("red_comercio"))
            {
                double bonusSinergia = 1.0 + (datos.planetasDesbloqueados * 0.50);
                pasivoFinal *= bonusSinergia;
            }
        }

        AnadirDinero(pasivoFinal);
        if (ui != null) ui.ActualizarInterfaz();
    }

    // 4. LA TIENDA
    public bool GastarDinero(double cantidad)
    {
        if (dineroActual >= cantidad)
        {
            dineroActual -= cantidad;
            return true;
        }
        return false;
    }

    public void AnadirDinero(double cantidad)
    {
        dineroActual += cantidad;
        dineroTotal += cantidad;

        // Si el panel de logros existe y está abierto, refrescamos los colores
        if (LogrosManager.instance != null && LogrosManager.instance.achievementsOpen)
        {
            LogrosManager.instance.RefrescarLogros();
        }
    }
}