using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    // --- DATOS NORMALES (Los que se pondrán a CERO al reencarnar) ---
    public string nombreUsuario;
    public double dineroActual;
    public double dineroPorClic = 1;
    public double dineroPorSeg;
    public double dineroTotal;
    public int[] nivelesCompras;
    public bool[] mejorasCompradas;

    // --- DATOS DE PRESTIGIO (Los que son INMORTALES y nunca se borran) ---
    public double monedasPrestigio; 
    public List<string> mejorasPrestigioCompradas = new List<string>(); 
}