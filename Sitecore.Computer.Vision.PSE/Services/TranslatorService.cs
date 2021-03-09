using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Cognitive.Translator.PSE.Caching;
using Sitecore.Cognitive.Translator.PSE.Models;
using Sitecore.Configuration;

namespace Sitecore.Cognitive.Translator.PSE.Services
{
    public class TranslatorService : ITranslatorService
    {
        private readonly string _cognitiveServicesKey = Settings.GetSetting($"Sitecore.Cognitive.Translator.PSE.TranslateService.ApiKey", "");
        private readonly string _cognitiveServicesUrl = Settings.GetSetting($"Sitecore.Cognitive.Translator.PSE.TranslateService.ApiUrl", "");
        private readonly string _cognitiveServicesZone = Settings.GetSetting($"Sitecore.Cognitive.Translator.PSE.TranslateService.ApiZone", "");

        public async Task<TranslationResult[]> GetTranslatation(string textToTranslate, string fromLang, string targetLanguage, string textType)
        {
            return await CacheManager.GetCachedObject(textToTranslate + fromLang + targetLanguage + textType, async () =>
            {
                var route = $"/translate?api-version=3.0&to={targetLanguage}&suggestedFrom=en";

                if (!string.IsNullOrEmpty(fromLang))
                {
                    route += $"&from={fromLang}";
                }

                if (!string.IsNullOrEmpty(textType) && textType.Equals("Rich Text"))
                {
                    route += "&textType=html";
                }

                var requestUri = _cognitiveServicesUrl + route;
                var translationResult = await TranslateText(requestUri, textToTranslate);

                return translationResult;
            });
        }

        async Task<TranslationResult[]> TranslateText(string requestUri, string inputText)
        {
            var body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(requestUri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _cognitiveServicesKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", _cognitiveServicesZone);

                var response = await client.SendAsync(request).ConfigureAwait(false);
                var result = await response.Content.ReadAsStringAsync();
                var deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);

                return deserializedOutput;
            }
        }
    }
}
