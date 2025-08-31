using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.DTOs;
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

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(null) }
            };

            app.MapGet("/users", async () =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"));
                var users = JsonSerializer.Deserialize<List<OnlineLibrary.DTOs.User>>(json, options);

                if (users == null)
                {
                    return Results.NotFound("No users found.");
                }

                return Results.Ok(users);
            }).WithName("user");

            app.MapGet("/user/details/{id}", async (int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"));
                var users = JsonSerializer.Deserialize<List<OnlineLibrary.DTOs.User>>(json, options);

                if (users == null)
                {
                    return Results.NotFound("No users found.");
                }

                var user = users.FirstOrDefault(user => user.UserId == id);

                if (user == null)
                {
                    return Results.NotFound($"User with ID {id} not found.");
                }

                return Results.Ok(user);
            });

            app.MapPost("/users/create/", async ([FromBody] CreateUser createUser) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"));
                var users = JsonSerializer.Deserialize<List<OnlineLibrary.DTOs.User>>(json, options);

                int newId = users.Any() ? users.Max(u => u.UserId) + 1 : 1;

                OnlineLibrary.DTOs.User user = new(
                    newId,
                    createUser.Name
                    );

                users.Add(user);

                var updatedJson = JsonSerializer.Serialize(users, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"), updatedJson);

                return Results.CreatedAtRoute("user", new { id = user.UserId }, user);
            });

            app.MapPut("/users/update/{id}", async ([FromBody] UpdateUser updateUser, int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"));
                var users = JsonSerializer.Deserialize<List<OnlineLibrary.DTOs.User>>(json, options);

                var index = users.FindIndex(user => user.UserId == id);

                if (index == -1)
                {
                    return Results.NotFound($"User with ID {id} not found.");
                }

                users[index] = new
                (
                    id,
                    updateUser.Name
                );

                var updatedJson = JsonSerializer.Serialize(users, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"), updatedJson);

                return Results.NoContent();
            });

            app.MapDelete("/users/delete/{id}", async (int id) =>
            {
                var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"));
                var users = JsonSerializer.Deserialize<List<OnlineLibrary.DTOs.User>>(json, options);

                OnlineLibrary.DTOs.User foundUser = users.Find(user => user.UserId == id);

                if (foundUser is null)
                {
                    return Results.NotFound($"User with ID {id} not found.");
                }

                users.Remove(foundUser);

                var updatedJson = JsonSerializer.Serialize(users, options);
                await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Data", "users.json"), updatedJson);

                return Results.NoContent();
            });

            return app;
        }
    }
}
