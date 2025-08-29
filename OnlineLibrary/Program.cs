using OnlineLibrary.DBContext;
using OnlineLibrary.Logic;
using System;
using  static OnlineLibrary.Logic.BookMapEndpointsExtensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Sourse=OnlineLibrary.db";
builder.Services.AddSqlite<OnlineLibraryContext>(connectionString);

var app = builder.Build();

app.BookMapEndpointsExt();

app.Run();    
