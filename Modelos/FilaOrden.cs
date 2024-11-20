namespace Api_Restaurante.Modelos;

public class FilaOrden {
    public int Orden {get; set;}
    public int Artículo {get; set;}
    public byte Cantidad {get; set;}

    public FilaOrden () {
        //Constructor vacío.
    }
}