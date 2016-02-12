using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SampleMVCTest.Extensions
{
    public static  class HttpResponseMessageExtentions
    {
        public static async Task<KeyValuePair<string, string>> GetAntiForgeryFormToken(this HttpResponseMessage resp, string antiForgeryFormTokenName)
        {
            const string val = " value=\"";
            var content = await resp.Content.ReadAsStringAsync();
            var aftLine = content.Split('\n')
                .FirstOrDefault(t => (t ?? "").Contains(antiForgeryFormTokenName));
            if (null == aftLine)
            {
                throw new KeyNotFoundException(string.Format("The string {0} was not found in the HttpResponse", antiForgeryFormTokenName));
            }
            var start = aftLine.IndexOf(val, aftLine.IndexOf(antiForgeryFormTokenName)) + val.Length;
            var end = aftLine.IndexOf("\" />", start);
            var token = aftLine.Substring(start, end - start);
            return new KeyValuePair<string, string>(antiForgeryFormTokenName, token);
        }

        public static CookieHeaderValue GetCookie(this HttpResponseMessage resp, string cookieName)
        {
            IEnumerable<string> values;
            if (!resp.Headers.TryGetValues("Set-Cookie", out values))
                throw new KeyNotFoundException("No cookies found in the response.");
            var setCookie = SetCookieHeaderValue
                .ParseList(values.ToList())
                .FirstOrDefault(t => t.Name.Equals(cookieName));
            if (null == setCookie)
                throw new KeyNotFoundException(string.Format("A cookie with the name {0} could not be found in the response", cookieName));
            var cookie = new CookieHeaderValue(setCookie.Name, setCookie.Value);
            return cookie;
        }
    }
}
