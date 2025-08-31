using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.DTOs;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineLibrary.Logic
{
    public static class AuthorMapEndpointsExtensions
    {
        public static WebApplication AuthorMapEndpointsExt(this WebApplication app)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(null) }
            };

            app.MapGet("/authors", async () =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"));
                var authors = JsonSerializer.Deserialize<List<Author>>(json, options);

                if (authors == null)
                {
                    return Results.NotFound("No authors found.");
                }

                return Results.Ok(authors);
            }).WithName("author");

            app.MapGet("/author/details/{id}", async (int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"));
                var authors = JsonSerializer.Deserialize<List<Author>>(json, options);

                if (authors == null)
                {
                    return Results.NotFound("No authors found.");
                }

                var author = authors.FirstOrDefault(author => author.AuthorId == id);

                if (author == null)
                {
                    return Results.NotFound($"Author with ID {id} not found.");
                }

                return Results.Ok(author);
            });

            app.MapPost("/authors/create/", async ([FromBody] CreateAuthor createAuthor) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"));
                var authors = JsonSerializer.Deserialize<List<Author>>(json, options);

                int newId = authors.Any() ? authors.Max(a => a.AuthorId) + 1 : 1;

                Author author = new(
                    newId,
                    createAuthor.Name,
                    createAuthor.Biography
                    );

                authors.Add(author);

                var updatedJson = JsonSerializer.Serialize(authors, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"), updatedJson);

                return Results.CreatedAtRoute("author", new { id = author.AuthorId }, author);
            });

            app.MapPut("/authors/update/{id}", async ([FromBody] UpdateAuthor updateAuthor, int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"));
                var authors = JsonSerializer.Deserialize<List<Author>>(json, options);

                var index = authors.FindIndex(author => author.AuthorId == id);

                if (index == -1)
                {
                    return Results.NotFound($"Author with ID {id} not found.");
                }

                authors[index] = new
                (
                    id,
                    updateAuthor.Name,
                    updateAuthor.Biography
                );

                var updatedJson = JsonSerializer.Serialize(authors, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"), updatedJson);

                return Results.NoContent();
            });

            app.MapDelete("/authors/delete/{id}", async (int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"));
                var authors = JsonSerializer.Deserialize<List<Author>>(json, options);

                Author foundAuthor = authors.Find(author => author.AuthorId == id);

                if (foundAuthor is null)
                {
                    return Results.NotFound($"Author with ID {id} not found.");
                }

                authors.Remove(foundAuthor);

                var updatedJson = JsonSerializer.Serialize(authors, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "authors.json"), updatedJson);

                return Results.NoContent();
            });

            return app;
        }
    }
}
