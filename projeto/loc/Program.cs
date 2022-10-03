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
//dotnet tool install --global dotnet-ef
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
    	public string? endereco { get; set; }
    	public string? telefone { get; set; }
        public int idade { get; set; }
    }

	class Filme
    {
    	public int id { get; set; }
    	public int classIndicativa { get; set; }
		public string? nome { get; set; }
    	public string? diretor { get; set; }
        public string? dataLancamento { get; set; }
        public string? genero { get; set; }
    }

	class Alocar
    {
    	public int id { get; set; }
		public int idUsuario { get; set; }
    	public int idFilme { get; set; }
        public string? dataAloc { get; set; }
    }
	
	class BaseDados : DbContext
	{
		public BaseDados(DbContextOptions options) : base(options)
		{
		}
		
		public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Filme> Filmes { get; set; } = null!;
		public DbSet<Alocar> Alocacoes { get; set; } = null!;
		
	}

	class Program
	{
		static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			
			var connectionString = builder.Configuration.GetConnectionString("Usuarios") ?? "Data Source=Usuarios.db";
            connectionString = builder.Configuration.GetConnectionString("Filmes") ?? "Data Source=Filmes.db";
			connectionString = builder.Configuration.GetConnectionString("Alocações") ?? "Data Source=Alocs.db";
			builder.Services.AddSqlite<BaseDados>(connectionString);
			var app = builder.Build();
			
			//listar todos os usuarios
			app.MapGet("/usuarios", (BaseDados baseUsuarios) => {
				return baseUsuarios.Usuarios.ToList();
			});

			//listar todos os filmes
			app.MapGet("/filmes", (BaseDados baseFilmes) => {
				return baseFilmes.Filmes.ToList();
			});

			//listar todos as alocações
			app.MapGet("/alocacoes", (BaseDados baseAlocacoes) => {
				return baseAlocacoes.Alocacoes.ToList();
			});
			
			//listar usuario especifico (por id)
			app.MapGet("/usuario/{id}", (BaseDados baseUsuarios, int id) => {
				return baseUsuarios.Usuarios.Find(id);
			});

			//listar filme especifico (por id)
			app.MapGet("/filme/{id}", (BaseDados baseFilmes, int id) => {
				return baseFilmes.Filmes.Find(id);
			});

			//listar alocação especifica (por id)
			app.MapGet("/alocacao/{id}", (BaseDados baseAlocacoes, int id) => {
				return baseAlocacoes.Alocacoes.Find(id);
			});
			
			//cadastrar usuario
			app.MapPost("/cadastrarusuario", (BaseDados baseUsuarios, Usuario usuario) =>
			{
				baseUsuarios.Usuarios.Add(usuario);
				baseUsuarios.SaveChanges();
				return "Usuário cadastrado!";
			});

			//cadastrar filme
			app.MapPost("/cadastrarfilme", (BaseDados baseFilmes, Filme filme) =>
			{
				baseFilmes.Filmes.Add(filme);
				baseFilmes.SaveChanges();
				return "Filme cadastrado!";
			});

			//cadastrar alocação
			app.MapPost("/cadastraraloc", (BaseDados baseAlocacoes, Alocar alocar) =>
			{
				baseAlocacoes.Alocacoes.Add(alocar);
				baseAlocacoes.SaveChanges();
				return "Alocação cadastrada!";
			});
			
			//atualizar usuario
			app.MapPost("/atualizarusuario/{id}", (BaseDados baseUsuarios, Usuario usuarioAtualizado, int id) =>
			{
				var usuario = baseUsuarios.Usuarios.Find(id);
				usuario.nome = usuarioAtualizado.nome;
				usuario.endereco = usuarioAtualizado.endereco;
				usuario.idade = usuarioAtualizado.idade;
				usuario.telefone = usuarioAtualizado.telefone;
				baseUsuarios.SaveChanges();
				return "Usuario atualizado.";
			});

			//atualizar filme
			app.MapPost("/atualizarfilme/{id}", (BaseDados baseFilmes, Filme filmeAtualizado, int id) =>
			{
				var filme = baseFilmes.Filmes.Find(id);
				filme.nome = filmeAtualizado.nome;
				filme.diretor = filmeAtualizado.diretor;
				filme.dataLancamento = filmeAtualizado.dataLancamento;
				filme.genero = filmeAtualizado.genero;
				filme.classIndicativa = filmeAtualizado.classIndicativa;
				baseFilmes.SaveChanges();
				return "Filme atualizado.";
			});

			//atualizar alocação
			app.MapPost("/atualizaralocacao/{id}", (BaseDados baseAlocacoes, Alocar alocacaoAtualizado, int id) =>
			{
				var alocacao = baseAlocacoes.Alocacoes.Find(id);
				alocacao.idUsuario = alocacaoAtualizado.idUsuario;
				alocacao.idFilme = alocacaoAtualizado.idFilme;
				alocacao.dataAloc = alocacaoAtualizado.dataAloc;
				baseAlocacoes.SaveChanges();
				return "Alocação atualizada.";
			});
						
			//deletar usuario
			app.MapPost("/deletarusuario/{id}", (BaseDados baseUsuarios, int id) =>
			{
				var usuario = baseUsuarios.Usuarios.Find(id);
				baseUsuarios.Remove(usuario);
				baseUsuarios.SaveChanges();
				return "Usuario deletado.";
			});

			//deletar filme
			app.MapPost("/deletarfilme/{id}", (BaseDados baseFilmes, int id) =>
			{
				var filme = baseFilmes.Filmes.Find(id);
				baseFilmes.Remove(filme);
				baseFilmes.SaveChanges();
				return "Filme deletado.";
			});

			//deletar alocação
			app.MapPost("/deletaralocacao/{id}", (BaseDados baseAlocacoes, int id) =>
			{
				var alocacao = baseAlocacoes.Alocacoes.Find(id);
				baseAlocacoes.Remove(alocacao);
				baseAlocacoes.SaveChanges();
				return "Alocação deletada.";
			});
						
			app.Run();
		}
	}
}