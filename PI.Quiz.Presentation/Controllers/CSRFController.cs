using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace PI.Quiz.Presentation.Controllers
{
    [Route("api/csrf")]
    [ApiController]
    public class CSRFController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public CSRFController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
        }

        [HttpPost("generator")]
        public async Task<IActionResult> Generator()
        {
            // Generate CSRF token
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

            return await Task.FromResult(Ok(new
            {
                Message = "Generate CSRF token successfully.",
                CsrfToken = tokens.RequestToken
            }));
        }
    }
}
