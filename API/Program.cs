using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

var botik = new Bot();
var cancellationTokenSource = new CancellationTokenSource();


var botTask = Task.Run(() => botik.StartBotAsync(cancellationTokenSource.Token));

Console.WriteLine("Запускаем бота и Web API...");

await app.RunAsync();

cancellationTokenSource.Cancel();
