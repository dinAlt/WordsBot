using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WordsBot.Common;

namespace WordsBot.Translators.YandexTranslate
{
  public class YandexTranslate : ITranslator
  {
    static readonly string _endpoint = "https://translate.api.cloud.yandex.net/translate/v2/translate";

    readonly string _serviceAccountKey;
    readonly string _folderId;

    public YandexTranslate(string serviceAccountKey, string folderId) =>
      (_serviceAccountKey, _folderId) = (serviceAccountKey, folderId);

    public async Task<List<string>> TranslateAsync(string word, string from, string to)
    {
      var cli = new HttpClient();
      using var content = JsonContent.Create(new
      {
        folderId = _folderId,
        sourceLanguageCode = from,
        targetLanguageCode = to,
        texts = new string[] { word }
      });
      cli.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Api-Key", _serviceAccountKey);
      using var res = await cli.PostAsync(_endpoint, content);
      if (res.StatusCode != HttpStatusCode.OK)
      {
        throw new Exception(await res.Content.ReadAsStringAsync());
      }

      var response = (await res.Content.ReadFromJsonAsync(
        typeof(TranslationResponse))) as TranslationResponse;
      if (response == null)
      {
        throw new NullReferenceException("not decoded");
      }

      if (response.translations.First().text == word)
      {
        return new List<string>();
      }

      return response.translations.Select(t => t.text).ToList();
    }

#pragma warning disable IDE1006
    private record TranslationResponse(TranslationResponseItem[] translations);

    private record TranslationResponseItem(string text, string detectedLanguageCode);
#pragma warning restore
  }
}
