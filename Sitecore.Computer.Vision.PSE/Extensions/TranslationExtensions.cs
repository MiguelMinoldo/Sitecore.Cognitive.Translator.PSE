using System.Linq;
using System.Threading.Tasks;
using Sitecore.Cognitive.Translator.PSE.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Sitecore.Cognitive.Translator.PSE.Extensions
{
    public class TranslationExtensions
    {
        private readonly ITranslatorService _translatorService;

        public TranslationExtensions(ITranslatorService translatorServices)
        {
            _translatorService = translatorServices;
        }

        public TranslationExtensions()
        {
            _translatorService = ServiceLocator.ServiceProvider.GetService<ITranslatorService>();
        }

        public async Task<string> TranslateText(string input, string fromLang, string destLang, string textType)
        {
            var res = await _translatorService.GetTranslatation(input, fromLang, destLang, textType);

            if (res != null && res.Any())
            {
                return res[0].Translations[0].Text;
            }

            return string.Empty;
        }
    }
}