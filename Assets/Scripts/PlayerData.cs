[System.Serializable]
public class PlayerData
{
    public string nombreUsuario;
    public double dineroActual;
    public double dineroPorClic;
    public double dineroPorSeg;

    public double dineroTotal;
    public int[] nivelesCompras;
    
    // Si más adelante quieres guardar las mejoras, añadiríamos:
    // public bool[] mejorasCompradas;
}