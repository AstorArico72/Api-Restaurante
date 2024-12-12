using System.ComponentModel.DataAnnotations;

namespace Api_Restaurante.Modelos;

public class Artículo {
    [Key]
    public int ID {get; set;}
    public string Nombre {get; set;}
    public int Precio {get; set;}
    public List<string>? Atributos {get; set;}
    public string Imagen {get; set;}

    public Artículo () {
        //Constructor vacío.
    }
}