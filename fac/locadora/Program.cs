//criar projeto:
//	dotnet new webabi -minimal -o NomeDoProjeto
//entrar na pasta:
//	cd NomeDoProjeto
//adicionar entity framework no console:
//	dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 6.0
//	dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 6.0
//	dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0
//incluir namespace do entity framework:
//	using Microsoft.EntityFrameworkCore;
//antes de rodar o dotnet run pela primeira vez, rodar os seguintes comandos para iniciar a base de dados:
//	dotnet ef migrations add InitialCreate
//	dotnet ef database update

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace locadora
{
	class Usuario
    {
    	public int id { get; set; }
		public string? nome { get; set; }
    	public string? email { get; set; }
    }
	
	class BaseUsuarios : DbContext
	{
		public BaseUsuarios(DbContextOptions options) : base(options)
		{
		}
		
		public DbSet<Usuario> Usuarios { get; set; } = null!;
	}

	
	class Program
	{
		static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			
			var connectionString = builder.Configuration.GetConnectionString("Usuarios") ?? "Data Source=Usuarios.db";
			builder.Services.AddSqlite<BaseUsuarios>(connectionString);
			
			var app = builder.Build();
			
			//listar todos os usuarios
			app.MapGet("/usuarios", (BaseUsuarios baseUsuarios) => {
				return baseUsuarios.Usuarios.ToList();
			});
			
			//listar usuario especifico (por email)
			app.MapGet("/usuario/{id}", (BaseUsuarios baseUsuarios, int id) => {
				return baseUsuarios.Usuarios.Find(id);
			});
			
			//cadastrar usuario
			app.MapPost("/cadastrar", (BaseUsuarios baseUsuarios, Usuario usuario) =>
			{
				baseUsuarios.Usuarios.Add(usuario);
				baseUsuarios.SaveChanges();
				return "usuario adicionado";
			});
			
			//atualizar usuario
			app.MapPost("/atualizar/{id}", (BaseUsuarios baseUsuarios, Usuario usuarioAtualizado, int id) =>
			{
				var usuario = baseUsuarios.Usuarios.Find(id);
				usuario.nome = usuarioAtualizado.nome;
				usuario.email = usuarioAtualizado.email;
				baseUsuarios.SaveChanges();
				return "usuario atualizado";
			});
						
			//deletar usuario
			app.MapPost("/deletar/{id}", (BaseUsuarios baseUsuarios, int id) =>
			{
				var usuario = baseUsuarios.Usuarios.Find(id);
				baseUsuarios.Remove(usuario);
				baseUsuarios.SaveChanges();
				return "usuario atualizado";
			});
						
			app.Run();
		}
	}
}