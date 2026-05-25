using FCG.Catalog.Application.Biblioteca.Services;
using FCG.Catalog.Application.Jogo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Catalog.Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CriarJogoService>();
        services.AddScoped<ListarJogosService>();
        services.AddScoped<ListarJogosAtivosService>();
        services.AddScoped<AtualizarJogoService>();
        services.AddScoped<AlterarStatusJogoService>();
        services.AddScoped<ListarBibliotecaService>();
        services.AddScoped<ComprarJogoService>();
        services.AddScoped<ConfirmarPagamentoService>();

        return services;
    }
}
