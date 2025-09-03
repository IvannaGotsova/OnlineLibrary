using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DBContext;
using OnlineLibrary.Entities;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Reflection.Metadata.BlobBuilder;

namespace OnlineLibrary.Logic
{
    public static class AuthorMapEndpointsExtensions
    {
        public static WebApplication AuthorMapEndpointsExt(this WebApplication app)
        {

            app.MapGet("/authors", async (OnlineLibraryContext onlineLibraryContext) => {

                var authors = onlineLibraryContext.Authors;

                if (authors == null)
                {
                    return Results.NotFound("No authors found.");
                }

                var html = new System.Text.StringBuilder();
                html.Append("<!DOCTYPE html><html><head><title>Authors</title></head><body>");
                html.Append("<button onclick =\"window.location.href='/books'\"> Go to BOOKS</button >");
                html.Append("<button onclick=\"window.location.href='/users'\">Go to USERS</button>");
                html.Append("<button onclick=\"window.location.href='/authors/create'\">CREATE AUTHOR</button>");
                html.Append("<h1>Authors List</h1><table border='1'><tr><th>No.</th><th>Name</th><th>Biography</th><th>Books</th></tr>");

                foreach (var author in authors)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{author.AuthorId}</td>");
                    html.Append($"<td>{author.Name}</td>");
                    html.Append($"<td>{author.Biography}</td>");
                    html.Append($"<td>{string.Join(", ", onlineLibraryContext.Books.Where(b => b.AuthorId == author.AuthorId).Select(b => $"<a href='/books/details/{b.BookId}'>{b.Title}</a>"))}</td>");
                    html.Append("<td>");
                    html.Append($"<button onclick=\"window.location.href='/authors/details/{author.AuthorId}/'\">Details</button>");
                    html.Append("</td>");
                    html.Append("<td>");
                    html.Append($"<button onclick=\"window.location.href='/authors/delete/{author.AuthorId}/'\">Delete</button>");
                    html.Append("</td>");
                    html.Append("</tr>");
                }

                return Results.Content(html.ToString(), "text/html");
            }).WithName("author");

            app.MapGet("/authors/details/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var authors = onlineLibraryContext.Authors;

                if (authors == null)
                {
                    return Results.NotFound("No authors found.");
                }

                var author = authors.FirstOrDefault(author => author.AuthorId == id);

                if (author == null)
                {
                    return Results.NotFound($"Author with ID {id} not found.");
                }

                var html = new System.Text.StringBuilder();
                html.Append("<!DOCTYPE html><html><head><title>Author</title></head><body>");
                html.Append("<button onclick =\"window.location.href='/books'\"> Go to BOOKS</button >");
                html.Append("<button onclick =\"window.location.href='/authors'\"> Go to AUTHORS</button >");
                html.Append("<button onclick=\"window.location.href='/users'\">Go to USERS</button>");
                html.Append("<h1>Author</h1>");
                html.Append($"<h2>Number: {author.AuthorId}</h2>");
                html.Append($"<h2>Name: {author.Name}</h2>");
                html.Append($"<h3>Biography: {author.Biography}</h3>");
                html.Append($"<td>{string.Join(", ", onlineLibraryContext.Books.Where(b => b.AuthorId == author.AuthorId).Select(b => $"<a href='/books/details/{b.BookId}'>{b.Title}</a>"))}</td>");
                html.Append("<br/ >");
                html.Append("<br/ >");
                html.Append($"<button onclick=\"window.location.href='/authors/update/{author.AuthorId}/'\">Update</button>");
                html.Append($"<button onclick=\"window.location.href='/authors/delete/{author.AuthorId}/'\">Delete</button>");

                return Results.Content(html.ToString(), "text/html");
            });

            app.MapGet("/authors/create/", () =>
            {
                var html = @"
                 <!DOCTYPE html>
                 <html>
                 <head>
                     <title>Create Author</title>
                     <style>
                         body { font-family: Arial; margin: 20px; }
                         label { display: block; margin-top: 10px; }
                         input, textarea { width: 100%; padding: 8px; margin-top: 5px; }
                         .book-input { margin-bottom: 5px; }
                         button { margin-top: 15px; padding: 10px 15px; }
                     </style>
                 </head>
                 <body>
                     <button onclick=""window.location.href='/books'"">Go to BOOKS</button>
                     <button onclick=""window.location.href='/authors'"">Go to AUTHORS</button>
                     <button onclick=""window.location.href='/users'"">Go to USERS</button>
                     <h1>Create a New Author</h1>
                     <form method='post' action='/authors/create'>
                         <label>Name:</label>
                         <input type='text' name='Name' minlength='2' maxlength ='50' required />
                 
                         <label>Biography:</label>
                         <textarea name='Biography' minlength='2' maxlength ='1000' rows='4' required></textarea>
                         <button type='submit'>Create Author</button>
                     </form>
                 </body>
                 </html>";

                return Results.Content(html, "text/html");
            });

            app.MapPost("/authors/create/", async (HttpRequest request, OnlineLibraryContext onlineLibraryContext) =>
            {
                var authors = onlineLibraryContext.Authors;

                int newId = await authors.MaxAsync(a => (int?)a.AuthorId) ?? 0;
                newId += 1;

                var formAuthor = await request.ReadFormAsync();
                string name = formAuthor["Name"];
                string biography = formAuthor["Biography"];
                var books = formAuthor["Books"].ToList();

                Author author = new(
                    newId,
                    name = name,
                    biography = biography
                    );

                using var transaction = await onlineLibraryContext.Database.BeginTransactionAsync();

                await onlineLibraryContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Authors ON");

                onlineLibraryContext.Authors.Add(author);
                await onlineLibraryContext.SaveChangesAsync();

                await onlineLibraryContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Authors OFF");

                await transaction.CommitAsync();

                return Results.Redirect($"/authors/details/{newId}");
            });

            app.MapGet("/authors/update/{id}", async (OnlineLibraryContext onlineLibraryContext, int id) =>
            {
                var author = await onlineLibraryContext.Authors.FindAsync(id);

                if (author is null)
                {
                    return Results.NotFound($"No author with ID {id}");
                }

                var html = $@"
                     <!DOCTYPE html>
                     <html>
                     <head>
                         <title>Update Author</title>
                         <style>
                             body {{ font-family: Arial; margin: 20px; }}
                             label {{ display: block; margin-top: 10px; }}
                             input, textarea {{ width: 100%; padding: 8px; margin-top: 5px; }}
                             button {{ margin-top: 15px; padding: 10px 15px; }}
                         </style>
                     </head>
                     <body>
                         <button onclick=""window.location.href='/books'"">Go to BOOKS</button>
                         <button onclick=""window.location.href='/authors'"">Go to AUTHORS</button>
                         <button onclick=""window.location.href='/users'"">Go to USERS</button>
                         <h1>Update Author</h1>
                         <form method='post' action='/authors/update/{id}'>
                             <label>Name:</label>
                             <input type='text' name='Name' minlength='2' maxlength ='50' value='{System.Net.WebUtility.HtmlEncode (author.Name)}'required / >
                  
                             <label>Biography:</label>
                             <textarea name='Biography' minlength='2' maxlength ='1000' rows='4' required>
                 {System.Net.WebUtility.HtmlEncode(author.Biography)}
                             </textarea>
                 
                             <button type='submit'>Update Author</button>
                         </form>
                     </body>
                     </html>";

                return Results.Content(html, "text/html");
            });

            app.MapPost("/authors/update/{id}", async (HttpRequest request, OnlineLibraryContext onlineLibraryContext, int id) =>
            {
                var authors = onlineLibraryContext.Authors;

                var formAuthor = await request.ReadFormAsync();
                var name = formAuthor["Name"];
                var biography = formAuthor["Biography"];

                Author foundAuthor = authors.FirstOrDefault(a => a.AuthorId == id);

                if (foundAuthor.AuthorId == -1)
                {
                    return Results.NotFound($"Author with ID {id} not found.");
                }

                foundAuthor.Name = name;
                foundAuthor.Biography = biography;

                await onlineLibraryContext.SaveChangesAsync();

                return Results.Redirect($"/authors/details/{id}");
            });

            app.MapGet("/authors/delete/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var author = await onlineLibraryContext.Authors.FindAsync(id);

                if (author is null)
                {
                    return Results.NotFound($"Author with ID {id} not found");
                }

                var html = $@"
                 <!DOCTYPE html>
                 <html>
                 <head>
                     <title>Delete Author</title>
                     <style>
                         body {{ font-family: Arial; margin: 20px; }}
                         .warning {{ color: red; font-weight: bold; }}
                         button {{ margin: 5px; padding: 10px 15px; }}
                     </style>
                 </head>
                 <body>
                      <button onclick=""window.location.href='/books'"">Go to BOOKS</button>
                      <button onclick=""window.location.href='/authors'"">Go to AUTHORS</button>
                      <button onclick=""window.location.href='/users'"">Go to USERS</button>
                     <h1 class='warning'>Delete Author</h1>
                     <p>Are you sure you want to delete <strong>{System.Net.WebUtility.HtmlEncode (author.Name)} </   strong>?</p>
                     <form method='post' action='/authors/delete/{id}'>
                         <button type='submit'>Yes, Delete</button>
                         <a href='/authors/details/{id}'><button type='button'>Cancel</button></a>
                     </form>
                 </body>
                 </html>";

                return Results.Content(html, "text/html");
            });

            app.MapPost("/authors/delete/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var authors = onlineLibraryContext.Authors;

                Author foundAuthor = await authors.FirstOrDefaultAsync(a => a.AuthorId == id);

                if (foundAuthor is null)
                {
                    return Results.NotFound($"Author with ID {id} not found.");
                }

                onlineLibraryContext.Authors.Remove(foundAuthor);
                await onlineLibraryContext.SaveChangesAsync();

                return Results.Redirect($"/authors");
            });

            return app;
        }
    }
}
