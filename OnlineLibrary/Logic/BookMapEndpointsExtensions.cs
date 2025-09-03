using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using OnlineLibrary.DBContext;
using OnlineLibrary.Entities;
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

            app.MapGet("/books", async (OnlineLibraryContext onlineLibraryContext) => {

                var books = onlineLibraryContext.Books;

                if (books == null)
                {
                    return Results.NotFound("No books found.");
                }

                var html = new System.Text.StringBuilder();
                html.Append("<!DOCTYPE html><html><head><title>Books</title></head><body>");
                html.Append("<button onclick=\"window.location.href='/authors'\">Go to AUTHORS</button>");
                html.Append("<button onclick =\"window.location.href='/users'\"> Go to USERS</button >");
                html.Append("<button onclick=\"window.location.href='/books/create'\">CREATE BOOK</button>");
                html.Append("<h1>Books List</h1><table border='1'><tr><th>No.</th><th>Title</th><th>Description</th><th>Author</th><th>Genre</th><th>Image</th><th>Release Date</th><th>Pages</th><th>Price</th></tr>");

                foreach (var book in books)
                {
                    Author author = onlineLibraryContext.Authors.FirstOrDefault(a => a.AuthorId == book.AuthorId);
                    Genre genre = onlineLibraryContext.Genres.FirstOrDefault(g => g.GenreId == book.GenreId);

                    html.Append("<tr>");
                    html.Append($"<td>{book.BookId}</td>");
                    html.Append($"<td>{book.Title}</td>");
                    html.Append($"<td>{book.Description}</td>");
                    html.Append($"<td><a href='/authors/details/{author.AuthorId}'>{author.Name}</a></td>");
                    html.Append($"<td>{genre.Name}</td>");
                    html.Append("<td>");
                    html.Append($"<img src={book.ImageUrl} width='100' height='100'>");
                    html.Append("</td>");
                    html.Append($"<td>{book.ReleaseDate.ToString("yyyy-MM-dd")}</td>");
                    html.Append($"<td>{book.Pages}</td>");
                    html.Append($"<td>{book.Price}</td>");
                    html.Append("<td>");
                    html.Append($"<button onclick=\"window.location.href='/books/details/{book.BookId}/'\">Details</button>");
                    html.Append("</td>");
                    html.Append("<td>");
                    html.Append($"<button onclick=\"window.location.href='/books/delete/{book.BookId}/'\">Delete</button>");
                    html.Append("</td>");
                    html.Append("</tr>");
                }

                html.Append("</table></body></html>");

                return Results.Content(html.ToString(), "text/html");
            }).WithName("book");

            app.MapGet("/books/details/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var books = onlineLibraryContext.Books;

                if (books == null)
                {
                    return Results.NotFound("No books found.");
                }

                var book = books.FirstOrDefault(book => book.BookId == id);

                if (book == null)
                {
                    return Results.NotFound($"Book with ID {id} not found.");
                }

                Author author = onlineLibraryContext.Authors.FirstOrDefault(a => a.AuthorId == book.AuthorId);
                Genre genre = onlineLibraryContext.Genres.FirstOrDefault(g => g.GenreId == book.GenreId);

                var html = new System.Text.StringBuilder();
                html.Append("<!DOCTYPE html><html><head><title>Books</title></head><body>");
                html.Append("<button onclick =\"window.location.href='/books'\"> Go to BOOKS</button >");
                html.Append("<button onclick =\"window.location.href='/authors'\"> Go to AUTHORS</button >");
                html.Append("<button onclick=\"window.location.href='/users'\">Go to USERS</button>");
                html.Append("<h1>Book</h1>");
                html.Append($"<h2>Number: {book.BookId}</h2>");
                html.Append($"<h2>Title: {book.Title}</h2>");
                html.Append($"<img src={book.ImageUrl} width='100' height='100'>");
                html.Append($"<h3>Description: {book.Description}</h3>");
                html.Append($"<h3>Author: {author.Name}</h3>");
                html.Append($"<h3>Genre: {genre.Name}</h3>");
                html.Append($"<h3>Release Date: {book.ReleaseDate.ToString("yyyy-MM-dd")}</h3>");
                html.Append($"<h3>Pages: {book.Pages}</h3>");
                html.Append($"<h3>Price: {book.Price}</h3>");
                html.Append($"<button onclick=\"window.location.href='/books/update/ {book.BookId}/'\">Update</button>");
                html.Append($"<button onclick=\"window.location.href='/books/delete/ {book.BookId}/'\">Delete</button>");

                return Results.Content(html.ToString(), "text/html");
            });


            app.MapGet("/books/create/", () =>
            {
                var html = @"
                 <!DOCTYPE html>
                 <html>
                 <head>
                     <title>Create Book</title>
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
                     <h1>Create a New Book</h1>
                     <form method='post' action='/books/create'>
                         <label>Title:</label>
                         <input type='text' name='Title' minlength='2' maxlength ='50' required />
                 
                         <label>Description:</label>
                         <textarea name='Description' minlength='2' maxlength ='10000' rows='4' required></textarea>

                         <label for=""author"">Author:</label>
                         <select id=""author"" name=""Author"" required>
                           <option value="""">-- Select an Author --</option>
                         </select>
                         <script>
                           fetch('/api/authors')
                           .then(response => response.json())
                           .then(authors => {
                             const select = document.getElementById(""author"");
                             authors.forEach(author => {
                               const option = document.createElement(""option"");
                               option.value = author.authorId;
                               option.textContent = author.name;
                               select.appendChild(option);
                             });
                           })
                           .catch(error => console.error(""Error loading authors:"", error));
                         </script>


                         <label for=""genre"">Genre:</label>
                         <select id=""genre"" name=""Genre""required>
                           <option value="""">-- Select a Genre --</option>
                         </select>
                         <script>
                           fetch('/api/genres')
                           .then(response => response.json())
                           .then(genres => {
                             const select = document.getElementById(""genre"");
                             genres.forEach(genre => {
                               const option = document.createElement(""option"");
                               option.value = genre.genreId;
                               option.textContent = genre.name;
                               select.appendChild(option);
                             });
                           })
                           .catch(error => console.error(""Error loading genres:"", error));
                         </script>

                         <label>Image:</label>
                         <textarea name='Image'  minlength='5' maxlength ='100' rows='4' required></textarea>

                         <label>Release Date:</label>
                         <input type='date'  name='Release Date' rows='4' required />

                         <label>Price:</label>
                         <input type='number'  name='Price' step='1' min='1' max='5000' rows='4' required />

                         <label>Pages:</label>
                         <input type='number' name='Pages' step='0.01' min='0.00' max='50000.00' rows='4' required />
                         <button type='submit'>Create Book</button>
                     </form>
                 </body>
                 </html>";

                return Results.Content(html, "text/html");
            });
            
            app.MapPost("/books/create/", async (HttpRequest request, OnlineLibraryContext onlineLibraryContext) =>
            {
                var books = onlineLibraryContext.Books;

                int newId = await books.MaxAsync(b => (int?)b.BookId) ?? 0;
                newId += 1;

                var formBook = await request.ReadFormAsync();
                string title = formBook["Title"];
                string description = formBook["Description"];
                var author = Convert.ToInt32(formBook["Author"]);
                var genre = Convert.ToInt32(formBook["Genre"]);
                string image = formBook["Image"];
                var releaseDate = Convert.ToDateTime(formBook["Release Date"]);
                decimal price = Convert.ToDecimal(formBook["Price"]);
                int pages = Convert.ToInt32(formBook["Pages"]);

                Book book = new(
                    newId,
                    title = title,
                    description = description,
                    author = author,
                    releaseDate = releaseDate,
                    genre = genre,
                    pages = pages,
                    price = price,
                    image = image
                );

                using var transaction = await onlineLibraryContext.Database.BeginTransactionAsync();

                await onlineLibraryContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Books ON");

                onlineLibraryContext.Books.Add(book);
                await onlineLibraryContext.SaveChangesAsync();

                await onlineLibraryContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Books OFF");

                await transaction.CommitAsync();

                return Results.Redirect($"/books/details/{newId}");
            });

            app.MapGet("/books/update/{id}", async (OnlineLibraryContext onlineLibraryContext, int id) =>
            {
                var book = await onlineLibraryContext.Books.FindAsync(id);

                if (book is null)
                {
                    return Results.NotFound($"No book with ID {id}");
                }

                var html = $@"
                     <!DOCTYPE html>
                     <html>
                     <head>
                         <title>Update Book</title>
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
                         <h1>Update Book</h1>
                         <form method='post' action='/books/update/{id}'>
                             <label>Title:</label>
                             <input type='text' name='Title' minlength='2' maxlength ='50' value='{System.Net.WebUtility.HtmlEncode(book.Title)}'required / >
                  
                             <label>Description:</label>
                             <textarea name='Description' rows='4' minlength='2' maxlength ='10000' required>
                                {System.Net.WebUtility.HtmlEncode(book.Description)}
                             </textarea>

                             <label for=""author"">Author:</label>
                             <select id=""author"" name=""Author"" required>
                               <option value="""">-- Select an Author --</option>
                             </select>

                             <script>
                               const currentAuthorId = {book.AuthorId};

                               fetch('/api/authors')
                               .then(response => response.json())
                               .then(authors => {{
                                 const select = document.getElementById(""author"");
                                 authors.forEach(author => {{
                                   const option = document.createElement(""option"");
                                   option.value = author.authorId;
                                   option.textContent = author.name;

                                if (author.authorId === currentAuthorId) 
                                   {{
                                     option.selected = true;
                                   }}

                                   select.appendChild(option);
                                 }});
                               }})
                               .catch(error => console.error(""Error loading authors:"", error));
                             </script>
                            
                            
                             <label for=""genre"">Genre:</label>
                             <select id=""genre"" name=""Genre"" required>
                               <option value="""">-- Select a Genre --</option>
                             </select>

                             <script>
                               const currentGenreId = {book.GenreId};

                               fetch('/api/genres')
                               .then(response => response.json())
                               .then(genres => {{
                                 const select = document.getElementById(""genre"");
                                 genres.forEach(genre => {{
                                   const option = document.createElement(""option"");
                                   option.value = genre.genreId;
                                   option.textContent = genre.name;

                                if (genre.genreId === currentGenreId) 
                                   {{
                                     option.selected = true;
                                   }}

                                   select.appendChild(option);
                                 }});
                               }})
                               .catch(error => console.error(""Error loading genres:"", error));
                             </script>

                             <label>Release Date:</label>
                             <input type='date' name='Release Date' value='{System.Net.WebUtility.HtmlEncode(book.ReleaseDate.ToString("yyyy-MM-dd"))}'required / >

                             <label>Price:</label>
                             <input type='number' name='Price' name='Price' step='1' min='1' max='5000' value='{System.Net.WebUtility.HtmlEncode(book.Price.ToString())}'required / >

                             <label>Pages:</label>
                             <input type='number' name='Pages' name='Pages' step='0.01' min='0.00' max='50000.00' value='{System.Net.WebUtility.HtmlEncode(book.Pages.ToString())}'required / >

                             <label>Image:</label>
                             <input type='text' name='Image' minlength='5' maxlength ='100' value='{System.Net.WebUtility.HtmlEncode(book.ImageUrl)}'required / >
                            
                             <button type='submit'>Update Book</button>
                         </form>
                     </body>
                     </html>";

                return Results.Content(html, "text/html");
            });

            app.MapPost("/books/update/{id}", async (HttpRequest request, OnlineLibraryContext onlineLibraryContext, int id) =>
            {
                var books = onlineLibraryContext.Books;

                var formBook = await request.ReadFormAsync();
                string title = formBook["Title"];
                string description = formBook["Description"];
                var author = Convert.ToInt32(formBook["Author"]);
                var genre = Convert.ToInt32(formBook["Genre"]);
                string image = formBook["Image"];
                var releaseDate = Convert.ToDateTime(formBook["Release Date"]);
                decimal price = Convert.ToDecimal(formBook["Price"]);
                int pages = Convert.ToInt32(formBook["Pages"]);

                Book foundBook = books.FirstOrDefault(b => b.BookId == id);

                if (foundBook.BookId == -1)
                {
                    return Results.NotFound($"Book with ID {id} not found.");
                }

                foundBook.Title = title;
                foundBook.Description = description;
                foundBook.AuthorId = author;
                foundBook.GenreId = genre;
                foundBook.ImageUrl = image;
                foundBook.ReleaseDate = releaseDate;
                foundBook.Pages = pages;
                foundBook.Price = price;

                await onlineLibraryContext.SaveChangesAsync();

                return Results.Redirect($"/books/details/{id}");
            });

            app.MapGet("/books/delete/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var book = await onlineLibraryContext.Books.FindAsync(id);

                if (book is null)
                {
                    return Results.NotFound($"Book with ID {id} not found");
                }

                var html = $@"
                 <!DOCTYPE html>
                 <html>
                 <head>
                     <title>Delete Book</title>
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
                     <h1 class='warning'>Delete Book</h1>
                     <p>Are you sure you want to delete <strong>{System.Net.WebUtility.HtmlEncode(book.Title)} </   strong>?</p>
                     <form method='post' action='/books/delete/{id}'>
                         <button type='submit'>Yes, Delete</button>
                         <a href='/books/details/{id}'><button type='button'>Cancel</button></a>
                     </form>
                 </body>
                 </html>";

                return Results.Content(html, "text/html");
            });

            app.MapPost("/books/delete/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var books = onlineLibraryContext.Books;

                Book foundBook = await books.FirstOrDefaultAsync(b => b.BookId == id);

                if (foundBook is null)
                {
                    return Results.NotFound($"Book with ID {id} not found.");
                }

                onlineLibraryContext.Books.Remove(foundBook);
                await onlineLibraryContext.SaveChangesAsync();

                return Results.Redirect($"/books");
            });

            app.MapGet("/api/authors", (OnlineLibraryContext context) =>
            {
                var authors = context.Authors
                    .Select(a => new { AuthorId = a.AuthorId, Name = a.Name })
                    .ToList();

                return Results.Ok(authors);
            });

            app.MapGet("/api/genres", (OnlineLibraryContext context) =>
            {
                var genres = context.Genres
                    .Select(g => new { GenreId = g.GenreId, Name = g.Name })
                    .ToList();

                return Results.Ok(genres);
            });

            return app;
        }
    }
}
