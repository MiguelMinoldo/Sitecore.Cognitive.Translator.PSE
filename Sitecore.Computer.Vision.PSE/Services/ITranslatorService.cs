using System.Threading.Tasks;
using Sitecore.Cognitive.Translator.PSE.Models;

namespace Sitecore.Cognitive.Translator.PSE.Services
{
    public interface ITranslatorService
    {
        Task<TranslationResult[]> GetTranslatation(string textToTranslate, string fromLang, string targetLanguage, string textType);
    }
}
