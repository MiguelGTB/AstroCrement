using Firebase.Database;

public static class SlotRankingHelper
{
    // Define la lista de nodos correspondientes a los datos de progreso por planeta.
    private static readonly string[] nombresPlanetas = {
        "progresoLuna", "progresoMarte", "progresoEuropa",
        "progresoTitan", "progresoKepler", "progresoDyson", "progresoColapso"
    };

    // Analiza los slots de guardado de un usuario para determinar su rendimiento total acumulado.
    public static bool TryGetMejorSlotRanking(DataSnapshot usuarioSnapshot, out UsuarioRanking ranking)
    {
        ranking = null;

        // Verifica la existencia del nodo de slots para el usuario.
        var slots = usuarioSnapshot.Child("slots");
        if (!slots.Exists) return false;

        double mejorTotal = -1;
        string mejorNombre = "Sin Nombre";

        // Itera a través de todos los slots disponibles del usuario.
        foreach (var slot in slots.Children)
        {
            var datos = slot.Child("datos");
            if (!datos.Exists) continue;

            double totalSlot = 0;
            // Acumula el 'dineroTotal' de cada planeta registrado en el slot.
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

            // Extrae el nombre de usuario asociado si está disponible en el nodo de datos.
            string nombre = "Sin Nombre";
            if (datos.HasChild("nombreUsuario"))
                nombre = datos.Child("nombreUsuario").Value.ToString();

            // Actualiza los registros si el slot actual supera el total del mejor encontrado hasta el momento.
            if (totalSlot > mejorTotal)
            {
                mejorTotal = totalSlot;
                mejorNombre = nombre;
            }
        }

        // Si se halló un total válido, instancia el objeto de ranking y retorna verdadero.
        if (mejorTotal >= 0)
        {
            ranking = new UsuarioRanking(mejorNombre, mejorTotal);
            return true;
        }

        return false;
    }
}