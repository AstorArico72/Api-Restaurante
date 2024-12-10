using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Claims;
using Api_Restaurante.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Api_Restaurante.Api;

[ApiController]
[Route("/Api/Conectarse")]
[AllowAnonymous]
public class ConexionController : ControllerBase {
    private readonly IConfiguration Config;

    public ConexionController (IConfiguration ajustes) {
        this.Config = ajustes;
    }

    [HttpGet("ConexionCliente")]
    public IActionResult HabilitarMesa () {
        Claim? UsuarioExistente = User.Claims.FirstOrDefault (claim => claim.Type == "IdCliente", null);
        if (UsuarioExistente == null) {
            var Llave = new SymmetricSecurityKey (System.Text.Encoding.UTF8.GetBytes(Config["TokenAuthentication:SecretKey"]));
            var Credenciales = new SigningCredentials (Llave, SecurityAlgorithms.HmacSha256);
            List <Claim> ClaimList = new List<Claim> {
                new Claim (ClaimTypes.Role, "Cliente"),
                new Claim ("IdCliente", GenerarIdCliente ())
            };

            var Token = new JwtSecurityToken (
                issuer: Config["TokenAuthentication:Issuer"],
                audience: Config["TokenAuthentication:Audience"],
                claims: ClaimList,
                expires: DateTime.Now.AddHours (24),
                signingCredentials: Credenciales
            );

            return Ok (new JwtSecurityTokenHandler ().WriteToken (Token));
        } else {
            return NoContent ();
        }
    }

    [HttpGet("ConexionCocina")]
    public IActionResult HabilitarCocina () {
        var Llave = new SymmetricSecurityKey (System.Text.Encoding.UTF8.GetBytes(Config["TokenAuthentication:SecretKey"]));
        var Credenciales = new SigningCredentials (Llave, SecurityAlgorithms.HmacSha256);
        List <Claim> ClaimList = new List<Claim> {
            new Claim (ClaimTypes.Role, "Cocina")
        };

        var Token = new JwtSecurityToken (
            issuer: Config["TokenAuthentication:Issuer"],
            audience: Config["TokenAuthentication:Audience"],
            claims: ClaimList,
            expires: DateTime.Now.AddHours (24),
            signingCredentials: Credenciales
        );

        return Ok (new JwtSecurityTokenHandler ().WriteToken (Token));
    }

    private string GenerarIdCliente () {
        string IP = HttpContext.Connection.RemoteIpAddress.ToString ();
        Console.WriteLine ("Tu IP es: " + IP);
        string IdCliente = IP.Split (".").Last ();
        Console.WriteLine ("Tu ID de cliente es: " + IdCliente);
        
        return IdCliente;
    }
}