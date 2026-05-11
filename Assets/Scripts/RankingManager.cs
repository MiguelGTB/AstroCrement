using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    DatabaseReference dbRef;
    public Transform contenedorFila;
    public GameObject filaPrefab;

    void Start()
    {
        // Apuntamos a "usuarios"
        dbRef = FirebaseDatabase.DefaultInstance.GetReference("usuarios");
        CargarRankingHistorico();
    }

    public void CargarRankingHistorico()
    {
        // CAMBIO CLAVE: Ordenamos por 'dineroTotalLogrado'
        dbRef.OrderByChild("dineroTotalLogrado").LimitToLast(10).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) return;

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<DataSnapshot> listaMejores = snapshot.Children.Reverse().ToList();

                // Volvemos al hilo principal para tocar la UI
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    DibujarRanking(listaMejores);
                });
            }
        });
    }

    // Dentro de RankingManager.cs

    void DibujarRanking(List<DataSnapshot> usuarios)
    {
        // 1. Limpiamos lo que hubiera antes
        foreach (Transform hijo in contenedorFila) Destroy(hijo.gameObject);

        int pos = 1;
        foreach (var user in usuarios)
        {
            // 2. Creamos (Instantiate) una copia del prefab dentro del contenedor
            GameObject nuevaFila = Instantiate(filaPrefab, contenedorFila);

            // 3. Le pasamos los datos de Firebase al script de la fila
            string nombre = user.Child("nombreUsuario").Value.ToString();
            double total = double.Parse(user.Child("dineroTotalLogrado").Value.ToString());

            nuevaFila.GetComponent<FilaRanking>().Configurar(pos, nombre, FormatearNumero(total));
            pos++;
        }
    }

    string FormatearNumero(double n)
    {
        if (n >= 1000000000) return (n / 1000000000).ToString("F2") + "B";
        if (n >= 1000000) return (n / 1000000).ToString("F2") + "M";
        return n.ToString("N0");
    }
}