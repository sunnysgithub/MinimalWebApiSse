using System.Text;

const string data = "data: ";
const string newLines = "\n\n";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/sse", async (HttpContext ctx, CancellationToken ct) =>
    {
        ctx.Response.Headers.TryAdd("Content-Type", "text/event-stream");
        
        var sb = new StringBuilder();
        while (!ct.IsCancellationRequested)
        {
            sb.Append(data);
            sb.Append(DateTime.Now.ToShortTimeString());
            sb.Append(newLines);
            await ctx.Response.WriteAsync(sb.ToString());
            await ctx.Response.Body.FlushAsync();
            
            sb.Clear();
            await Task.Delay(1000);
        }
    })
.WithName("Server-Sent Events")
.WithOpenApi();

app.Run();
