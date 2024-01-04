using Microsoft.AspNetCore.Mvc;

namespace PeliculasAPI.ApiBehavior
{
    public static class BehaviorBadRequests
    {
        public static void Parsear(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var respuesta = new List<string>();
                foreach (var llave in actionContext.ModelState.Keys)
                {
                    foreach (var error in actionContext.ModelState[llave].Errors)
                    {
                        respuesta.Add($"{llave}: {error.ErrorMessage}");
                    }
                }

                return new BadRequestObjectResult(respuesta);
            };
        }
    }
}
