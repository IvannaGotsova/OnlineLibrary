using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.DTOs;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace OnlineLibrary.Logic
{
    public static class BookMapEndpointsExtensions
    {
        public static WebApplication BookMapEndpointsExt(this WebApplication app)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(null) }
            };

            app.MapGet("/", () => "Hello World!");

            app.MapGet("/books", async () =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"));
                var books = JsonSerializer.Deserialize<List<Book>>(json, options);

                if (books == null)
                {
                    return Results.NotFound("No books found.");
                }

                return Results.Ok(books);
            }).WithName("book");

            app.MapGet("/books/details/{id}", async (int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"));
                var books = JsonSerializer.Deserialize<List<Book>>(json, options);

                if (books == null)
                {
                    return Results.NotFound("No books found.");
                }

                var book = books.FirstOrDefault(book => book.BookId == id);

                if (book == null)
                {
                    return Results.NotFound($"Book with ID {id} not found.");
                }

                return Results.Ok(book);
            });

            app.MapPost("/books/create/", async ([FromBody] CreateBook createBook) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"));
                var books = JsonSerializer.Deserialize<List<Book>>(json, options);

                int newId = books.Any() ? books.Max(b => b.BookId) + 1 : 1;

                Book book = new(
                    newId,
                    createBook.Title,
                    createBook.Description,
                    createBook.Author,
                    createBook.ReleaseDate,
                    createBook.Genre,
                    createBook.Pages,
                    createBook.Price,
                    createBook.ImageUrl
                    );

                books.Add(book);

                var updatedJson = JsonSerializer.Serialize(books, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"), updatedJson);

                return Results.CreatedAtRoute("book", new { id = book.BookId }, book);
            });

            app.MapPut("/books/update/{id}", async ([FromBody] UpdateBook updateBook, int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"));
                var books = JsonSerializer.Deserialize<List<Book>>(json, options);

                var index = books.FindIndex(book => book.BookId == id);

                if (index == -1)
                {
                    return Results.NotFound($"Book with ID {id} not found.");
                }

                books[index] = new Book
                (
                    id,
                    updateBook.Title,
                    updateBook.Description,
                    updateBook.Author,
                    updateBook.ReleaseDate,
                    updateBook.Genre,
                    updateBook.Pages,
                    updateBook.Price,
                    updateBook.ImageUrl
                );

                var updatedJson = JsonSerializer.Serialize(books, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"), updatedJson);

                return Results.NoContent();
            });

            app.MapDelete("/books/delete/{id}", async (int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"));
                var books = JsonSerializer.Deserialize<List<Book>>(json, options);

                Book foundBook = books.Find(book => book.BookId == id);

                if (foundBook is null)
                {
                    return Results.NotFound($"Book with ID {id} not found.");
                }

                books.Remove(foundBook);

                var updatedJson = JsonSerializer.Serialize(books, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "books.json"), updatedJson);

                return Results.NoContent();
            });

            return app;
        }
    }
}
