using Firebase.Database;

public static class SlotRankingHelper
{
    private static readonly string[] nombresPlanetas = {
        "progresoLuna", "progresoMarte", "progresoEuropa",
        "progresoTitan", "progresoKepler", "progresoDyson", "progresoColapso"
    };

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

            double totalSlot = 0;
            foreach (var nombrePlaneta in nombresPlanetas)
            {
                var planeta = datos.Child(nombrePlaneta);
                if (!planeta.Exists) continue;

                if (planeta.HasChild("dineroTotal"))
                {
                    double.TryParse(planeta.Child("dineroTotal").Value.ToString(), out double dt);
                    totalSlot += dt;
                }
            }

            if (totalSlot <= 0) continue;

            string nombre = "Sin Nombre";
            if (datos.HasChild("nombreUsuario"))
                nombre = datos.Child("nombreUsuario").Value.ToString();

            if (totalSlot > mejorTotal)
            {
                mejorTotal = totalSlot;
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