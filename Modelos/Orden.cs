using System.ComponentModel.DataAnnotations;

namespace Api_Restaurante.Modelos;

public class Orden {
    [Key]
    public int ID {get; set;}
    public int Cliente {get; set;}

    public Orden () {
        //Constructor vacío.
    }
}