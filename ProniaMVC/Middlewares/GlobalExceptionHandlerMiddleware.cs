namespace ProniaMVC.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {

               // await Console.Out.WriteLineAsync(e.Message);
               context.Response.Redirect($"/home/error?errorMessage={e.Message}");
            }
            
           
        }
    }
}
