using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    DatabaseReference dbRef;
    public Transform contenedorFila;
    public GameObject filaPrefab;

    void Start()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        dbRef = FirebaseDatabase.DefaultInstance.GetReference("usuarios");
        CargarRankingHistorico();
    }

    public void CargarRankingHistorico()
{
    Debug.Log("Intentando descargar ranking..."); // Mensaje de control
    dbRef.OrderByChild("total").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Error al descargar ranking: " + task.Exception);
            return;
        }

        DataSnapshot snapshot = task.Result;
        Debug.Log("Datos recibidos. Número de hijos: " + snapshot.ChildrenCount); // ¿Recibes datos?

        if (!snapshot.Exists)
        {
            Debug.LogWarning("La ruta 'usuarios' está vacía o no existe.");
            return;
        }

        List<UsuarioRanking> lista = new List<UsuarioRanking>();
        foreach (var usuario in snapshot.Children)
        {
            if (SlotRankingHelper.TryGetMejorSlotRanking(usuario, out UsuarioRanking ranking))
            {
                lista.Add(ranking);
            }
            else
            {
                Debug.LogWarning("No se pudo parsear al usuario: " + usuario.Key);
            }
        }
        
        lista = lista.OrderByDescending(u => u.total).ToList();
        DibujarRanking(lista);
    });
}

    void DibujarRanking(List<UsuarioRanking> usuarios)
    {
        foreach (Transform hijo in contenedorFila)
            Destroy(hijo.gameObject);

        int pos = 1;
        foreach (var u in usuarios)
        {
            GameObject nuevaFila = Instantiate(filaPrefab, contenedorFila);
            nuevaFila.transform.localScale = Vector3.one;

            FilaRanking scriptFila = nuevaFila.GetComponent<FilaRanking>();
            scriptFila.Configurar(pos, u.nombre, FormatearNumero(u.total));

            pos++;
        }
    }

    string FormatearNumero(double n)
    {
        if (n >= 1_000_000_000_000) return (n / 1_000_000_000_000).ToString("F2") + "T";
        if (n >= 1_000_000_000) return (n / 1_000_000_000).ToString("F2") + "B";
        if (n >= 1_000_000) return (n / 1_000_000).ToString("F2") + "M";
        if (n >= 1000) return (n / 1000).ToString("F1") + "K";
        return n.ToString("N0");
    }
}

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
