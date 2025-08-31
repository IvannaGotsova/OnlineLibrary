using OnlineLibrary.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineLibrary.Logic
{
    public static class HomeMapEndpointsExtensions
    {
        public static WebApplication HomeMapEndpointsExt(this WebApplication app)
        {

            app.MapGet("/", () => "Hello World!");

            return app;

        }

    }
}
