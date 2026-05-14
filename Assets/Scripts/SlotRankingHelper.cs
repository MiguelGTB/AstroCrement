using Firebase.Database;
using System.Collections.Generic;

public static class SlotRankingHelper
{
    public static bool TryGetMejorSlotRanking(DataSnapshot usuarioSnapshot, out UsuarioRanking ranking)
    {
        ranking = null;

        var slots = usuarioSnapshot.Child("slots");
        if (!slots.Exists) return false;

        double mejorTotal = -1;
        string mejorNombre = "Sin Nombre";

        foreach (var slot in slots.Children)
        {
            var datos = slot.Child("datos");
            if (!datos.Exists) continue;

            double total = 0;
            if (datos.HasChild("dineroTotal"))
                double.TryParse(datos.Child("dineroTotal").Value.ToString(), out total);

            if (total <= 0) continue;

            string nombre = "Sin Nombre";
            if (datos.HasChild("nombreUsuario"))
                nombre = datos.Child("nombreUsuario").Value.ToString();

            if (total > mejorTotal)
            {
                mejorTotal = total;
                mejorNombre = nombre;
            }
        }

        if (mejorTotal >= 0)
        {
            ranking = new UsuarioRanking(mejorNombre, mejorTotal);
            return true;
        }

        return false;
    }
}