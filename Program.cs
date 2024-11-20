using PIM_VIII.API.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<UsuarioDbContext>();
builder.Services.AddSingleton<PlaylistDbContext>();
builder.Services.AddSingleton<ConteudoDbContext>();
builder.Services.AddSingleton<CriadorDbContext>();
builder.Services.AddSingleton<TabelaRelacionamentoCriadorConteudoDbContext>();
builder.Services.AddSingleton<TabelaRelacionamentoPlaylistConteudoDbContext>();
builder.Services.AddSingleton<TabelaRelacionamentoPlaylistECriadorPlaylistDbContext>();
builder.Services.AddSingleton<TabelaRelacionamentoPlaylistUsuariosDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
