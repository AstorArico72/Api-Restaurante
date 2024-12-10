using Microsoft.EntityFrameworkCore;

namespace Api_Restaurante.Modelos;

[Keyless]
public class FilaOrden {
    public int? Orden {get; set;}
    public int Artículo {get; set;}
    public byte Cantidad {get; set;}

    public FilaOrden () {
        //Constructor vacío.
    }
}