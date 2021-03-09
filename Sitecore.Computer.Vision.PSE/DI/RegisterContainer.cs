using Microsoft.Extensions.DependencyInjection;
using Sitecore.Cognitive.Translator.PSE.Services;
using Sitecore.DependencyInjection;

namespace Sitecore.Cognitive.Translator.PSE.DI
{
    public class RegisterContainer : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITranslatorService, TranslatorService>();
        }
    }
}