using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HttpsServer.Pages
{
    public class IndexModel : PageModel
    {
        public bool isLoginSuccess = false;
        private readonly ILogger<IndexModel> _logger;
        private readonly IValidateTokenService _tokeValidator;

        public IndexModel(IValidateTokenService tokenService, ILogger<IndexModel> logger)
        {
            _logger = logger;
            _tokeValidator = tokenService;
        }

        public void OnGet()
        {
            var request = HttpContext.Request; // 获取当前的 HttpRequest 对象
            isLoginSuccess = true;
            // Validate Authorization only once
            if (!isLoginSuccess && request.Headers.ContainsKey("Authorization"))
            {
                var authorization = request.Headers["Authorization"];
                if (authorization.Count > 0)
                {
                    var tokenStr = authorization[0];
                    if (tokenStr != null && tokenStr.StartsWith("Bearer"))
                    {
                        isLoginSuccess = _tokeValidator.ValidateToken(tokenStr.Substring(7), _logger, out var _);
                    }
                }
            }
        }
    }
}
