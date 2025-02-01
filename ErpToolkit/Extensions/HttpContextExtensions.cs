using ErpToolkit.Helpers;
using Microsoft.AspNetCore.Http;

namespace ErpToolkit.Extensions
{
    public static class HttpContextExtensions
    {
        // verifica se l'utente è già autenticato sulla macchian client e se ha una sessione inizializzata (eg: ErpContext.Instance.UserId valido)
        public static bool IsUserIdentityAuthenticatedAndSessionValid(this HttpContext httpContext)
        {
            string userId = ErpContext.Session(httpContext)?.UserId.Trim() ?? "";
            return ( (httpContext.User.Identity?.IsAuthenticated ?? false) && (userId != "") && (userId == httpContext.User.Identity.Name) ) ;
        }
    }
}
