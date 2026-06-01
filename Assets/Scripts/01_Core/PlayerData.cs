using System.Collections.Generic;

[System.Serializable]
public class DatosPlaneta
{
    // Aquí dentro solo va lo que pertenece a UN solo planeta
    public double dineroActual;
    public double dineroPorClic = 1;
    public double dineroPorSeg;
    public double dineroTotal;
    public int[] nivelesCompras;
    public bool[] mejorasCompradas;
}

[System.Serializable]
public class PlayerData
{
    public string nombreUsuario;
    
    // PROGRESO POR PLANETA 
    public DatosPlaneta progresoLuna = new DatosPlaneta();
    public DatosPlaneta progresoMarte = new DatosPlaneta();
    public DatosPlaneta progresoEuropa = new DatosPlaneta();
    public DatosPlaneta progresoTitan = new DatosPlaneta();
    public DatosPlaneta progresoKepler = new DatosPlaneta();
    public DatosPlaneta progresoDyson = new DatosPlaneta();
    public DatosPlaneta progresoColapso = new DatosPlaneta();

    //  DATOS DE PRESTIGIO Y PROGRESO GLOBAL
    public double monedasPrestigio; 
    public List<string> mejorasPrestigioCompradas = new List<string>(); 
    public int planetasDesbloqueados = 0; 

    // DATOS DE LOGROS
    public int totalReencarnaciones = 0;
    public List<string> logrosCompletados = new List<string>();

}