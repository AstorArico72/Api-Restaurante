using Api_Restaurante.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Restaurante.Api;

[ApiController]
[Route("/Api/Articulos")]
[Authorize(policy:"Cliente")]
public class ArticulosController : ControllerBase {
    private ContextoDb Contexto;
    private readonly IConfiguration Config;

    public ArticulosController (ContextoDb contexto, IConfiguration ajustes) {
        this.Contexto = contexto;
        this.Config = ajustes;
    }

    [HttpGet("Menu")]
    public IActionResult VerMenu () {
        return Ok (Contexto.Artículos.ToList ());
    }

    [HttpGet("Detalles/{id}")]
    public IActionResult DetallesArticulo (int id) {
        Artículo? ArticuloEncontrado = Contexto.Artículos.Find (id);
        if (ArticuloEncontrado == null) {
            return NotFound ("No existe un artículo con ése ID.");
        } else {
            return Ok (ArticuloEncontrado);
        }
    }
}