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
        return Ok (Contexto.Ordenes.ToList ());
    }

    [HttpPost("Nueva")]
    [Authorize(policy:"Cliente")]
    public async Task <IActionResult> NuevaOrden ([FromForm] string filas) {
        List<FilaOrden>? FilasOrden = JsonConvert.DeserializeObject <List<FilaOrden>>(filas);
        Orden nuevaOrden = new Orden ();
        nuevaOrden.Cliente = int.Parse (User.Claims.FirstOrDefault (claim => claim.Type == "IdCliente").Value);
        await Contexto.Ordenes.AddAsync (nuevaOrden);
        if (ModelState.IsValid) {
            int UltimoId = await Contexto.Ordenes.MaxAsync (item => item.ID);
            if (FilasOrden.Count () > 0 && FilasOrden != null) {
                foreach (var fila in FilasOrden) {
                    fila.Orden = UltimoId;
                    await Contexto.Database.ExecuteSqlAsync ($"INSERT INTO Filas_Orden VALUES ({fila.Orden}, {fila.Artículo}, {fila.Cantidad});");
                }
            } else {
                return BadRequest ("No hay artículos en la orden.");
            }
            await Contexto.SaveChangesAsync ();
            return Created ();
        } else {
            return BadRequest (ModelState);
        }
    }

    [HttpDelete("Entregar")]
    [Authorize(policy:"Cocina")]
    public async Task <IActionResult> EntregarOrden (int id) {
        //Éste método hace un borrado lógico.
        Orden? OrdenAEntregar = Contexto.Ordenes.Find (id);
        if (OrdenAEntregar == null) {
            return BadRequest ("Ésa orden no existe");
        } else {
            Contexto.Ordenes.Entry (OrdenAEntregar).State = EntityState.Deleted;
            await Contexto.SaveChangesAsync ();
            return Ok ($"Orden #${id} entregada.");
        }
    }

    [HttpDelete("Descartar")]
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
}