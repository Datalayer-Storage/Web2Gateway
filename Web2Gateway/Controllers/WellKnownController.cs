namespace Web2Gateway;

using Microsoft.AspNetCore.Mvc;

public class WellKnownController : ControllerBase
{
    private readonly G2To3Service _g2To3Service;

    public WellKnownController(G2To3Service g2To3Service) => _g2To3Service = g2To3Service;

    [HttpGet(".well-known")]
    public IActionResult GetWellKnown()
    {
        return Ok(_g2To3Service.GetWellKnown());
    }
}
