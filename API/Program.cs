using Telegram.Bot;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();


var bot = new TelegramBotClient("8335329999:AAFCHSE7KHsAWXQ8rJhgcNE6sarCMqo8ix8");
var me = await bot.GetMe();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

app.Run();