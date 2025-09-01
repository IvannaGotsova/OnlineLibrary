using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DBContext;
using OnlineLibrary.Entities;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineLibrary.Logic
{
    public static class UserMapEndpointsExtensions
    {
        public static WebApplication UserMapEndpointsExt(this WebApplication app)
        {
            app.MapGet("/users", async (OnlineLibraryContext onlineLibraryContext) => {
            
                var users = onlineLibraryContext.Users;

                if (users == null)
                {
                    return Results.NotFound("No users found.");
                }

                var html = new System.Text.StringBuilder();
                html.Append("<!DOCTYPE html><html><head><title>Users</title></head><body>");
                html.Append("<h1>Users List</h1><table border='1'><tr><th>No.</th><th>Name</th><th>Books</th></tr>");

                foreach (var user in users)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{user.UserId}</td>");
                    html.Append($"<td>{user.Name}</td>");
                    html.Append($"<td>{string.Join(", ", user.Books.Select(b => b.Title))}</td>");
                    html.Append("</tr>");
                }

                html.Append("</table></body></html>");

                return Results.Content(html.ToString(), "text/html");
            }).WithName("user");

            app.MapGet("/users/details/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var users = onlineLibraryContext.Users;

                if (users == null)
                {
                    return Results.NotFound("No users found.");
                }

                var user = users.FirstOrDefault(user => user.UserId == id);

                if (user == null)
                {
                    return Results.NotFound($"User with ID {id} not found.");
                }

                var html = new System.Text.StringBuilder();
                html.Append("<!DOCTYPE html><html><head><title>User</title></head><body>");
                html.Append("<h1>User</h1>");
                html.Append($"<h2>Number: {user.UserId}</h2>");
                html.Append($"<h2>Name: {user.Name}</h2>");
                html.Append($"<h3>Books: {string.Join(", ", user.Books.Select(b => b.Title))}</h3>");

                return Results.Content(html.ToString(), "text/html");
            });

            app.MapGet("/users/create/", () =>
            {
                var html = @"
                 <!DOCTYPE html>
                 <html>
                 <head>
                     <title>Create User</title>
                     <style>
                         body { font-family: Arial; margin: 20px; }
                         label { display: block; margin-top: 10px; }
                         input, textarea { width: 100%; padding: 8px; margin-top: 5px; }
                         .book-input { margin-bottom: 5px; }
                         button { margin-top: 15px; padding: 10px 15px; }
                     </style>
                 </head>
                 <body>
                     <h1>Create a New User</h1>
                     <form method='post' action='/users/create'>
                         <label>Name:</label>
                         <input type='text' name='Name' required />
                 
                         <button type='submit'>Create User</button>
                     </form>
                 </body>
                 </html>";

                return Results.Content(html, "text/html");
            });
         
            app.MapPost("/users/create/", async (HttpRequest request, OnlineLibraryContext onlineLibraryContext) =>
            {
                var users = onlineLibraryContext.Users;

                int newId = await users.MaxAsync(u => (int?)u.UserId) ?? 0;
                newId += 1;

                var formUser = await request.ReadFormAsync();
                string name = formUser["Name"];
                var books = formUser["Books"].ToList();

                User user = new(
                    newId,
                    name = name
                    );

                using var transaction = await onlineLibraryContext.Database.BeginTransactionAsync();

                await onlineLibraryContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users ON");

                onlineLibraryContext.Users.Add(user);
                await onlineLibraryContext.SaveChangesAsync();

                await onlineLibraryContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users OFF");

                await transaction.CommitAsync();

                return Results.Redirect($"/users/details/{newId}");
            });

            app.MapGet("/users/update/{id}", async (OnlineLibraryContext onlineLibraryContext, int id) =>
            {
                var user = await onlineLibraryContext.Users.FindAsync(id);

                if (user is null)
                {
                    return Results.NotFound($"No user with ID {id}");
                }

                var html = $@"
                     <!DOCTYPE html>
                     <html>
                     <head>
                         <title>Update User</title>
                         <style>
                             body {{ font-family: Arial; margin: 20px; }}
                             label {{ display: block; margin-top: 10px; }}
                             input, textarea {{ width: 100%; padding: 8px; margin-top: 5px; }}
                             button {{ margin-top: 15px; padding: 10px 15px; }}
                         </style>
                     </head>
                     <body>
                         <h1>Update User</h1>
                         <form method='post' action='/users/update/{id}'>
                             <label>Name:</label>
                             <input type='text' name='Name' value='{System.Net.WebUtility.HtmlEncode(user.Name)}'required / >
                 
                             <button type='submit'>Update User</button>
                         </form>
                     </body>
                     </html>";

                return Results.Content(html, "text/html");
            });
   
            app.MapPost("/users/update/{id}", async (HttpRequest request, OnlineLibraryContext onlineLibraryContext, int id) =>
            {
                var users = onlineLibraryContext.Users;

                var formUser = await request.ReadFormAsync();
                var name = formUser["Name"];

                User foundUser = users.FirstOrDefault(u => u.UserId == id);

                if (foundUser.UserId == -1)
                {
                    return Results.NotFound($"User with ID {id} not found.");
                }

                foundUser.Name = name;

                await onlineLibraryContext.SaveChangesAsync();

                return Results.Redirect($"/users/details/{id}");
            });

            app.MapGet("/users/delete/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var user = await onlineLibraryContext.Users.FindAsync(id);

                if (user is null)
                {
                    return Results.NotFound($"User with ID {id} not found");
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
                     <h1 class='warning'>Delete Author</h1>
                     <p>Are you sure you want to delete <strong>{System.Net.WebUtility.HtmlEncode(user.Name)} </   strong>?</p>
                     <form method='post' action='/users/delete/{id}'>
                         <button type='submit'>Yes, Delete</button>
                         <a href='/users/details/{id}'><button type='button'>Cancel</button></a>
                     </form>
                 </body>
                 </html>";

                return Results.Content(html, "text/html");
            });

            app.MapPost("/users/delete/{id}", async (int id, OnlineLibraryContext onlineLibraryContext) =>
            {
                var users = onlineLibraryContext.Users;

                User foundUser = await users.FirstOrDefaultAsync(u => u.UserId == id);

                if (foundUser is null)
                {
                    return Results.NotFound($"User with ID {id} not found.");
                }

                onlineLibraryContext.Users.Remove(foundUser);
                await onlineLibraryContext.SaveChangesAsync();

                return Results.Redirect($"/users");
            });

            return app;
        }
    }
}
