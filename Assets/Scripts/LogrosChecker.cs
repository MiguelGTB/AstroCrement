using UnityEngine;
using System.Collections;

public class LogrosChecker : MonoBehaviour
{
    [Header("Configuración")]
    public float intervaloComprobacion = 1f;

    [System.Serializable]
    public class DatoLogroSimple
    {
        public string nombreLogro;
        public TipoLogro tipoLogro;
        public double metaRequerida;
        public int indiceInstalacion;
        public Sprite icono;
    }

    public DatoLogroSimple[] logros;

    void Start()
    {
        StartCoroutine(ComprobarLogrosLoop());
    }

    IEnumerator ComprobarLogrosLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloComprobacion);
            ComprobarTodos();
        }
    }

    void ComprobarTodos()
    {
        Debug.Log("Comprobando logros...");
        if (DatabaseManager.Instance == null || DatabaseManager.Instance.datosCargados == null)
        {
            Debug.Log("DatabaseManager no listo");
            return;
        }

        PlayerData datos = DatabaseManager.Instance.datosCargados;

        foreach (var logro in logros)
        {
            if (datos.logrosCompletados != null && datos.logrosCompletados.Contains(logro.nombreLogro))
                continue;

            double actual = ObtenerValor(logro, datos);
            if (logro.metaRequerida > 0 && actual >= logro.metaRequerida)
            {
                if (datos.logrosCompletados == null)
                    datos.logrosCompletados = new System.Collections.Generic.List<string>();

                datos.logrosCompletados.Add(logro.nombreLogro);
                Debug.Log("Logro completado: " + logro.nombreLogro);
                DatabaseManager.Instance.GuardarPartidaEnNube();

                if (NotificacionManager.Instance != null)
                    NotificacionManager.Instance.MostrarNotificacion(logro.nombreLogro, logro.icono);
                else
                    Debug.Log("NotificacionManager no encontrado");
            }
        }
    }

    double ObtenerValor(DatoLogroSimple logro, PlayerData datos)
    {
        DatosPlaneta[] planetas = {
            datos.progresoLuna, datos.progresoMarte, datos.progresoEuropa,
            datos.progresoTitan, datos.progresoKepler, datos.progresoDyson, datos.progresoColapso
        };

        switch (logro.tipoLogro)
        {
            case TipoLogro.PorDineroTotal:
                double total = 0;
                foreach (var p in planetas) if (p != null) total += p.dineroTotal;
                return total;

            case TipoLogro.PorPlanetasDesbloqueados:
                return datos.planetasDesbloqueados;

            case TipoLogro.PorCompras:
                double compras = 0;
                foreach (var p in planetas)
                    if (p?.nivelesCompras != null && p.nivelesCompras.Length > logro.indiceInstalacion)
                        compras += p.nivelesCompras[logro.indiceInstalacion];
                return compras;

            case TipoLogro.PorMejoraComprada:
                foreach (var p in planetas)
                    if (p?.mejorasCompradas != null && p.mejorasCompradas.Length > logro.indiceInstalacion && p.mejorasCompradas[logro.indiceInstalacion])
                        return 1;
                return 0;

            case TipoLogro.PorMejoraProduccionComprada:
                foreach (var p in planetas)
                    if (p?.mejorasCompradas != null)
                        for (int i = 1; i < p.mejorasCompradas.Length; i++)
                            if (p.mejorasCompradas[i]) return 1;
                return 0;

            case TipoLogro.PorDineroPorSegundo:
                double dps = 0;
                foreach (var p in planetas) if (p != null) dps += p.dineroPorSeg;
                return dps;

            case TipoLogro.PorTodasInstalaciones:
                for (int i = 1; i <= 14; i++)
                {
                    bool tiene = false;
                    foreach (var p in planetas)
                        if (p?.nivelesCompras != null && p.nivelesCompras.Length > i && p.nivelesCompras[i] >= 1)
                        { tiene = true; break; }
                    if (!tiene) return 0;
                }
                return 1;

            case TipoLogro.PorReencarnaciones:
                return datos.totalReencarnaciones;

            case TipoLogro.PorMejorasPrestigio:
                return datos.mejorasPrestigioCompradas?.Count ?? 0;

            case TipoLogro.PorTodasMejoras:
                int compradas = 0;
                for (int i = 0; i < 34; i++)
                    foreach (var p in planetas)
                        if (p?.mejorasCompradas != null && p.mejorasCompradas.Length > i && p.mejorasCompradas[i])
                        { compradas++; break; }
                return compradas;

            default:
                return 0;
        }
    }
}