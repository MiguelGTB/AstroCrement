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
        // Esta línea obliga a que siempre se baje lo más nuevo de internet
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);

        // Apuntamos a "usuarios"
        dbRef = FirebaseDatabase.DefaultInstance.GetReference("usuarios");
        CargarRankingHistorico();
    }

    public void CargarRankingHistorico()
    {
        // Usamos ContinueWithOnMainThread en lugar de ContinueWith
        // Importante: Asegúrate de tener "using Firebase.Extensions;" arriba del todo
        dbRef.OrderByChild("dineroTotal").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {

            if (task.IsFaulted)
            {
                Debug.LogError("Fallo: " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("¡DATOS RECIBIDOS! Cantidad: " + snapshot.ChildrenCount);

                List<DataSnapshot> listaMejores = snapshot.Children.Reverse().ToList();

                // Llamamos directamente a Dibujar sin el Dispatcher manual
                DibujarRanking(listaMejores);
            }
        });
    }

    void DibujarRanking(List<DataSnapshot> usuarios)
    {
        // 1. Limpiamos lo que hubiera antes
        foreach (Transform hijo in contenedorFila) Destroy(hijo.gameObject);

        if (usuarios.Count == 0) Debug.LogWarning("La lista de usuarios está vacía.");

        int pos = 1;
        foreach (var user in usuarios)
        {
            // 2. Creamos (Instantiate) una copia del prefab dentro del contenedor
            GameObject nuevaFila = Instantiate(filaPrefab, contenedorFila);

            // Forzamos la escala a 1 y la Z a 0 para evitar que el prefab "desaparezca"
            nuevaFila.transform.localScale = Vector3.one;
            nuevaFila.transform.localPosition = new Vector3(nuevaFila.transform.localPosition.x, nuevaFila.transform.localPosition.y, 0);

            // 3. Extraemos los datos con seguridad
            string nombre = user.HasChild("nombreUsuario") ? user.Child("nombreUsuario").Value.ToString() : "Sin Nombre";

            double total = 0;
            // IMPORTANTE: Primero intentamos leer dineroTotal, si no existe, usamos dineroActual
            if (user.HasChild("dineroTotal"))
            {
                total = double.Parse(user.Child("dineroTotal").Value.ToString());
            }
            else if (user.HasChild("dineroActual"))
            {
                total = double.Parse(user.Child("dineroActual").Value.ToString());
            }

            // 4. Pasamos los datos al componente de la fila
            FilaRanking scriptFila = nuevaFila.GetComponent<FilaRanking>();
            if (scriptFila != null)
            {
                scriptFila.Configurar(pos, nombre, FormatearNumero(total));
            }
            else
            {
                Debug.LogError("¡El Prefab no tiene el script FilaRanking pegado!");
            }

            pos++;
        }
    }

    string FormatearNumero(double n)
{
    if (n >= 1000000000000) return (n / 1000000000000).ToString("F2") + "T"; // Trillones
    if (n >= 1000000000) return (n / 1000000000).ToString("F2") + "B";    // Billones
    if (n >= 1000000) return (n / 1000000).ToString("F2") + "M";       // Millones
    if (n >= 1000) return (n / 1000).ToString("F1") + "K";          // Miles
    return n.ToString("N0"); // Números normales
}
}