using System.Text.Json;
using System.Text.Json.Nodes;
using Api_Restaurante.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace Api_Restaurante.Api;

[ApiController]
[Route("/Api/Ordenes")]
public class OrdenesController : ControllerBase {
    private ContextoDb Contexto;
    private readonly IConfiguration Config;

    public OrdenesController (ContextoDb contexto, IConfiguration ajustes) {
        this.Contexto = contexto;
        this.Config = ajustes;
    }
    
    [HttpGet("Todas")]
    [Authorize(policy:"Cocina")]
    public IActionResult LeerOrdenes () {
        List <Orden> ordenes = new List<Orden>();
        List<Orden> lista = Contexto.Ordenes.ToList ();
        foreach (Orden item in lista) {
            if (Contexto.Ordenes.Entry (item).State != EntityState.Deleted) {
                ordenes.Add (item);
            }
        }
        return Ok (ordenes);
    }

    [HttpPost("Nueva")]
    [Authorize(policy:"Cliente")]
    public async Task <IActionResult> NuevaOrden ([FromForm] string filas) {
        List<FilaOrden>? FilasOrden = JsonConvert.DeserializeObject <List<FilaOrden>>(filas);
        Orden nuevaOrden = new Orden ();
        nuevaOrden.Cliente = int.Parse (User.Claims.FirstOrDefault (claim => claim.Type == "IdCliente").Value);
        if (ModelState.IsValid) {
            await Contexto.Ordenes.AddAsync (nuevaOrden);
            await Contexto.SaveChangesAsync ();
            int UltimoId = Contexto.Ordenes.OrderBy (item => item.ID).Last ().ID;
            if (FilasOrden.Count () > 0 && FilasOrden != null) {
                foreach (var fila in FilasOrden) {
                    fila.Orden = UltimoId;
                    await Contexto.Database.ExecuteSqlAsync ($"INSERT INTO Filas_Orden VALUES ({fila.Orden}, {fila.Artículo}, {fila.Cantidad});");
                }
            } else {
                return BadRequest ("No hay artículos en la orden.");
            }
            return Created ();
        } else {
            return BadRequest (ModelState);
        }
    }

    [HttpDelete("Entregar/{id}")]
    [Authorize(policy:"Cocina")]
    public async Task <IActionResult> EntregarOrden (int id) {
        //Éste método hace un borrado lógico.
        Orden? OrdenAEntregar = Contexto.Ordenes.Find (id);
        if (OrdenAEntregar == null) {
            return BadRequest ("Ésa orden no existe");
        } else {
            Contexto.Ordenes.Entry (OrdenAEntregar).State = EntityState.Deleted;
            await Contexto.SaveChangesAsync ();
            return Ok ($"Orden #{id} entregada.");
        }
    }

    [HttpDelete("Descartar/{id}")]
    [Authorize(policy:"Cocina")]
    public async Task <IActionResult> DescartarOrden (int id) {
        //Éste método hace un borrado duro.
        Orden? OrdenAEntregar = Contexto.Ordenes.Find (id);
        if (OrdenAEntregar == null) {
            return BadRequest ("Ésa orden no existe");
        } else {
            Contexto.Ordenes.Remove (OrdenAEntregar);
            await Contexto.SaveChangesAsync ();
            return Ok ("Orden borrada.");
        }
    }

    [HttpGet("Detalles/{id}")]
    [Authorize(policy:"Cocina")]
    public IActionResult LeerFilas (int id) {
        Orden? OrdenSeleccionada = Contexto.Ordenes.Find (id);

        if (OrdenSeleccionada == null) {
            return NotFound ("Ésa orden no existe.");
        } else {
            List<FilaOrden> filas = Contexto.Filas_Orden.Where (fila => fila.Orden == id).ToList ();
            return Ok (filas);
        }
    }
}