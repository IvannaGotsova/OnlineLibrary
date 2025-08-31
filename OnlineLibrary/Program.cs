using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Data;
using OnlineLibrary.DBContext;
using OnlineLibrary.Logic;
using System;
using  static OnlineLibrary.Logic.BookMapEndpointsExtensions;
using static OnlineLibrary.Logic.AuthorMapEndpointsExtensions;
using static OnlineLibrary.Logic.UserMapEndpointsExtensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OnlineLibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

DatabaseSeed.DatabaseSeeder(app);

app.HomeMapEndpointsExt();
app.AuthorMapEndpointsExt();
app.UserMapEndpointsExt();
app.BookMapEndpointsExt();

app.Run();    
