using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DBContext;
using OnlineLibrary.Data;
using OnlineLibrary.Entities;
using Microsoft.IdentityModel.Tokens;

namespace OnlineLibrary.Data
{
    public static class DatabaseSeed
    {
        public static async Task DatabaseSeeder(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OnlineLibraryContext>();

            if (!context.Authors.Any())
            {
                var pathAuthors = Path.Combine(AppContext.BaseDirectory, "Data", "authors.json");
                var jsonAuthors = File.ReadAllText(pathAuthors);
                var authors = JsonSerializer.Deserialize<List<Author>>(jsonAuthors);

                using var transaction = await context.Database.BeginTransactionAsync();

                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Authors ON");

                context.Authors.AddRange(authors);
                await context.SaveChangesAsync();

                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Authors OFF");

                await transaction.CommitAsync();

            }

            if (!context.Users.Any())
            {
                var pathUsers = Path.Combine(AppContext.BaseDirectory, "Data", "users.json");
                var jsonUsers = File.ReadAllText(pathUsers);
                var users = JsonSerializer.Deserialize<List<User>>(jsonUsers);

                using var transaction = await context.Database.BeginTransactionAsync();

                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users ON");

                context.Users.AddRange(users);
                await context.SaveChangesAsync();

                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users OFF");

                await transaction.CommitAsync();
            }

            if (!context.Books.Any())
            {
                var pathBooks = Path.Combine(AppContext.BaseDirectory, "Data", "books.json");
                var jsonBooks = File.ReadAllText(pathBooks);
                var books = JsonSerializer.Deserialize<List<Book>>(jsonBooks);

                using var transaction = await context.Database.BeginTransactionAsync();

                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Books ON");

                context.Books.AddRange(books);
                await context.SaveChangesAsync();

                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Books OFF");

                await transaction.CommitAsync();
            }


        }
    }

}

