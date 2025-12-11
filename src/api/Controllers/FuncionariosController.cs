using Microsoft.AspNetCore.Mvc;
using api.Context;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionariosController : ControllerBase
{
    private readonly ClinicaDbContext _context;

    public FuncionariosController(ClinicaDbContext context)
    {
        _context = context;
    }

    // Endpoints de funcionários foram movidos para ClinicasController
    // Os funcionários são geridos pelas clínicas através de /api/clinicas/{clinicaId}/funcionarios
}
