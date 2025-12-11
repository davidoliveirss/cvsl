using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilsController : ControllerBase
{
    [HttpPost("hash")]
    public IActionResult GenerateHash([FromBody] HashPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Password é obrigatória" });
        }

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        return Ok(new
        {
            password = request.Password,
            hash = hash,
            message = "Hash gerado com sucesso"
        });
    }

    [HttpPost("verify")]
    public IActionResult VerifyHash([FromBody] VerifyPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Hash))
        {
            return BadRequest(new { message = "Password e Hash são obrigatórios" });
        }

        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, request.Hash);

        return Ok(new
        {
            password = request.Password,
            hash = request.Hash,
            isValid = isValid,
            message = isValid ? "Password válida!" : "Password inválida!"
        });
    }
}

public class HashPasswordRequest
{
    public string Password { get; set; } = string.Empty;
}

public class VerifyPasswordRequest
{
    public string Password { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
}
