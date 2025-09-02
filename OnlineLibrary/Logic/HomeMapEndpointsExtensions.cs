using OnlineLibrary.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineLibrary.Logic
{
    public static class HomeMapEndpointsExtensions
    {
        public static WebApplication HomeMapEndpointsExt(this WebApplication app)
        {

            app.MapGet("/", async context =>
            {
                var html = @"
                    <!DOCTYPE html>
                    <html>
                    <body>
                      <h2>ONLINE LIBRARY</h2> 
                      <button onclick=""window.location.href='/books'"">Go to BOOKS</button>
                      <button onclick=""window.location.href='/authors'"">Go to AUTHORS</button>
                      <button onclick=""window.location.href='/users'"">Go to USERS</button>
                    </body>
                    </html>";
                await context.Response.WriteAsync(html);
            });

            app.MapGet("/{*path}", async context =>
            {
                var html = @"
                    <!DOCTYPE html>
                    <html>
                    <body>
                      <h2>ERROR</h2> 
                      <button onclick=""window.location.href='/books'"">Go to BOOKS</button>
                      <button onclick=""window.location.href='/authors'"">Go to AUTHORS</button>
                      <button onclick=""window.location.href='/users'"">Go to USERS</button>
                    </body>
                    </html>";
                await context.Response.WriteAsync(html);
            });
       
            return app;

        }

    }
}
