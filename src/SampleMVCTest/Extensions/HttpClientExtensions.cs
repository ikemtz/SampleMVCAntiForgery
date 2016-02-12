using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace SampleMVCTest.Extensions
{
    public static class HttpClientExtensions
    {
        public async static Task<HttpResponseMessage> PostFormDataAsync<T>(this HttpClient client, string url, T viewModel, KeyValuePair<string, string>? antiForgeryToken = null)
        {
            var list = viewModel.GetType()
                .GetProperties()
                .Select(t => new KeyValuePair<string, string>(t.Name, (t.GetValue(viewModel) ?? new object()).ToString()))
                .ToList();
            if (antiForgeryToken.HasValue)
                list.Add(antiForgeryToken.Value);
            var formData = new FormUrlEncodedContent(list);
            return await client.PostAsync(url, formData);
        }
    }
}
