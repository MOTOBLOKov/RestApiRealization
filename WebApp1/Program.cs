using Microsoft.AspNetCore.Mvc;

namespace MainApplication;

class Program
{
    static List<User> users { get; set; } = new List<User>(){ new User("Teacher" , "30" , 1),
                                                              new User("Programmer" , "22" , 2),
                                                              new User("Builder" , "29" , 3)};
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(pol => pol.AddPolicy("AllowPolicy" , settings => settings.AllowAnyMethod()
                                                                                         .AllowAnyHeader()
                                                                                         .AllowAnyOrigin()
                                                                                         .AllowCredentials()));

        var app = builder.Build();

        app.MapGet("/" , async (context) => {
            context.Response.Headers.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/index.html");
        });

        app.MapGet("/getUsers" , async (context) => {
            context.Response.Headers.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync<List<User>>(users);
        });

        app.MapPost("/addUser" , (HttpContext context , [FromBody] User user) => {
            app.Logger.LogInformation(user.ToString());
            users.Add(user);
        });
        
        app.MapDelete("/removeUser" , (HttpContext context , [FromBody] int id)=>{
            app.Logger.LogInformation($"{id}"); 
            
            for(int i = 0 ; i < users.Count() ; ++i)
            {
                if(users[i].id == id)
                users.Remove(users[i]);
            }
        });

        app.MapPatch("/changeUser" , (HttpContext context , [FromBody] User user)=>{
            app.Logger.LogInformation(user.ToString());

            for(int i = 0 ; i < users.Count() ; ++i)
            {
                if(users[i].id == user.id){
                   users[i] = new User(user.work , user.age , user.id);
                }
            }
        });
        app.Run();
    }

    record User(string work , string age , int id);
}
