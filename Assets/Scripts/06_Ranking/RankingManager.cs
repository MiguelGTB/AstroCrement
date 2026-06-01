using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    // Referencia al nodo raíz de usuarios en la base de datos.
    DatabaseReference dbRef;
    
    // Contenedor UI para las filas del ranking y el prefab correspondiente.
    public Transform contenedorFila;
    public GameObject filaPrefab;

    // Inicializa la configuración de Firebase y dispara la carga de datos.
    void Start()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        dbRef = FirebaseDatabase.DefaultInstance.GetReference("usuarios");
        CargarRankingHistorico();
    }

    // Consulta los datos en Firebase, procesa la colección y ordena los resultados para el ranking.
    public void CargarRankingHistorico()
    {
        Debug.Log("Intentando descargar ranking..."); 
        
        // Ejecuta la consulta ordenada por el valor 'total' limitando a los 10 últimos registros.
        dbRef.OrderByChild("total").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al descargar ranking: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            Debug.Log("Datos recibidos. Número de hijos: " + snapshot.ChildrenCount); 

            if (!snapshot.Exists)
            {
                Debug.LogWarning("La ruta 'usuarios' está vacía o no existe.");
                return;
            }

            List<UsuarioRanking> lista = new List<UsuarioRanking>();
            foreach (var usuario in snapshot.Children)
            {
                // Extrae el mejor slot de ranking de cada usuario mediante la clase auxiliar.
                if (SlotRankingHelper.TryGetMejorSlotRanking(usuario, out UsuarioRanking ranking))
                {
                    lista.Add(ranking);
                }
                else
                {
                    Debug.LogWarning("No se pudo parsear al usuario: " + usuario.Key);
                }
            }
            
            // Ordena la lista resultante de forma descendente por el valor total.
            lista = lista.OrderByDescending(u => u.total).ToList();
            DibujarRanking(lista);
        });
    }

    // Instancia las filas de la UI para cada usuario contenido en la lista.
    void DibujarRanking(List<UsuarioRanking> usuarios)
    {
        // Limpia el contenedor de filas previas.
        foreach (Transform hijo in contenedorFila)
            Destroy(hijo.gameObject);

        int pos = 1;
        foreach (var u in usuarios)
        {
            GameObject nuevaFila = Instantiate(filaPrefab, contenedorFila);
            nuevaFila.transform.localScale = Vector3.one;

            // Configura el texto y posición de la fila mediante el script componente.
            FilaRanking scriptFila = nuevaFila.GetComponent<FilaRanking>();
            scriptFila.Configurar(pos, u.nombre, FormatearNumero(u.total));

            pos++;
        }
    }

    // Convierte valores numéricos largos a una representación simplificada con sufijos (K, M, B, T).
    string FormatearNumero(double n)
    {
        if (n >= 1_000_000_000_000) return (n / 1_000_000_000_000).ToString("F2") + "T";
        if (n >= 1_000_000_000) return (n / 1_000_000_000).ToString("F2") + "B";
        if (n >= 1_000_000) return (n / 1_000_000).ToString("F2") + "M";
        if (n >= 1000) return (n / 1000).ToString("F1") + "K";
        return n.ToString("N0");
    }
}

// Clase de datos simplificada para la representación de los usuarios en el ranking.
public class UsuarioRanking
{
    public string nombre;
    public double total;

    public UsuarioRanking(string nombre, double total)
    {
        this.nombre = nombre;
        this.total = total;
    }
}