using Api_Restaurante.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Restaurante.Api;

[ApiController]
[Route("/Api/Articulos")]
[Authorize]
public class ArticulosController : ControllerBase {
    private ContextoDb Contexto;
    private readonly IConfiguration Config;

    public ArticulosController (ContextoDb contexto, IConfiguration ajustes) {
        this.Contexto = contexto;
        this.Config = ajustes;
    }

    [HttpGet("Menu")]
    [Authorize(policy:"Cliente")]
    public IActionResult VerMenu () {
        return Ok (Contexto.Artículos.ToList ());
    }


    [Authorize(policy:"Restaurante")]
    [HttpGet("Detalles/{id}")]
    public async Task<IActionResult> DetallesArticulo (int id) {
        Artículo? ArticuloEncontrado = await Contexto.Artículos.FindAsync (id);
        if (ArticuloEncontrado == null) {
            return NotFound ("No existe un artículo con ése ID.");
        } else {
            return Ok (ArticuloEncontrado);
        }
    }
}