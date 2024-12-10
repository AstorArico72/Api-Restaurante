using Api_Restaurante.Modelos;
using Microsoft.EntityFrameworkCore;

public class ContextoDb : DbContext {
    public DbSet <Orden> Ordenes {get; set;}
    public DbSet <Artículo> Artículos {get; set;}
    public DbSet <FilaOrden> Filas_Orden {get; set;}

    public ContextoDb (DbContextOptions<ContextoDb> options) : base (options) {
        // Constructor vacío.
    }
}