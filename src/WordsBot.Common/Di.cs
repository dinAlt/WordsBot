using WordsBot.Common;
using WordsBot.Common.Controllers;
using WordsBot.Common.Views;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class WordsBotDi
  {
    public static IServiceCollection AddTranslator<T>(this IServiceCollection services) where T : class, ITranslator =>
      services.AddSingleton<ITranslator, T>();

    public static IServiceCollection AddWordsBot(this IServiceCollection serices) =>
      serices
        .AddSingleton<IViewFactory, ViewFactory>()
        .AddSingleton<IRouter, Router>()
        // .AddWordsbotControllers()
        .AddSingleton<IWordsBot, Bot>();

    private static IServiceCollection AddWordsbotControllers(
      this IServiceCollection services) =>
      services
        .AddScoped<MenuController>()
        .AddScoped<TranslateController>()
        .AddScoped<GameController>();
  }
}