using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DBContext;
using OnlineLibrary.Data;
using OnlineLibrary.Entities;

namespace OnlineLibrary.Data
{
    public static class DatabaseSeed
    {
        public static void DatabaseSeeder(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OnlineLibraryContext>();

            
            
            if (!context.Authors.Any())
            {
                var pathAuthors = Path.Combine(AppContext.BaseDirectory, "Data", "authors.json");
                var jsonAuthors = File.ReadAllText(pathAuthors);
                var authors = JsonSerializer.Deserialize<List<Author>>(jsonAuthors);

                context.Authors.AddRange(authors);
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var pathUsers = Path.Combine(AppContext.BaseDirectory, "Data", "users.json");
                var jsonUsers = File.ReadAllText(pathUsers);
                var users = JsonSerializer.Deserialize<List<User>>(jsonUsers);

                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Books.Any())
            {
                var pathBooks = Path.Combine(AppContext.BaseDirectory, "Data", "books.json");
                var jsonBooks = File.ReadAllText(pathBooks);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonBooks);

                context.Books.AddRange(books);
                context.SaveChanges();
            }


        }
    }

}

