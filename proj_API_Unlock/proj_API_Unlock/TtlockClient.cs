using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace proj_API_Unlock
{
    public class TtlockClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _accessToken;
        private readonly JavaScriptSerializer _serializer;

        public TtlockClient(HttpClient httpClient, string clientId, string accessToken)
        {
            _httpClient = httpClient;
            _clientId = clientId;
            _accessToken = accessToken;
            _serializer = new JavaScriptSerializer();
        }

        public async Task<bool> UnlockAsync(int lockId)
        {
            var url = "https://euapi.ttlock.com/v3/lock/unlock";

            // Timestamp em milissegundos desde 1970-01-01 UTC
            long date = (long)(DateTime.UtcNow
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds);

            var formContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("clientId", _clientId),
            new KeyValuePair<string, string>("accessToken", _accessToken),
            new KeyValuePair<string, string>("lockId", lockId.ToString()),
            new KeyValuePair<string, string>("date", date.ToString())
        });

            using (var response = await _httpClient.PostAsync(url, formContent))
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        string.Format("HTTP error {0}: {1}", response.StatusCode, responseBody)
                    );
                }

                TtlockResponse ttResp;
                try
                {
                    ttResp = _serializer.Deserialize<TtlockResponse>(responseBody);
                }
                catch (Exception ex)
                {
                    throw new Exception("Falha ao interpretar o JSON de resposta da TTLock.", ex);
                }

                if (ttResp == null)
                {
                    throw new Exception("Resposta da TTLock veio vazia ou em formato inesperado.");
                }

                if (ttResp.errcode != 0)
                {
                    throw new Exception(
                        string.Format("TTLock error {0}: {1}", ttResp.errcode, ttResp.errmsg)
                    );
                }

                return true;
            }
        }
    }
}
