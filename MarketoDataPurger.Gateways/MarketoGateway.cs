using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MarketoDataPurger.Gateways.Exceptions;
using MarketoDataPurger.Gateways.Models;
using Newtonsoft.Json;

namespace MarketoDataPurger.Gateways
{
    public class MarketoGateway : IMarketoGateway
    {
        private readonly string _marketoInstanceUrl;
        private readonly string _marketoClientId;
        private readonly string _marketoClientSecret;
        private AuthenticationToken _authenticationToken;

        public MarketoGateway(string marketoInstanceUrl, string marketoClientId, string marketoClientSecret)
        {
            _marketoInstanceUrl = marketoInstanceUrl;
            _marketoClientId = marketoClientId;
            _marketoClientSecret = marketoClientSecret;
            _authenticationToken = new AuthenticationToken();
        }

        public async Task<DeleteOpportunityResponse> DeleteOpportunity(DeleteOpportunityRequest deleteOpportunityRequest)
        {
            DeleteOpportunityResponse deleteOpportunityResponse = null;
            string path = "rest/v1/opportunities/delete.json";
            string body = JsonConvert.SerializeObject(deleteOpportunityRequest);

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(_marketoInstanceUrl)
            };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", GetToken());

            HttpContent httpContent = new StringContent(body);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(path, httpContent);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                Task<string> asyncString = response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                string result = asyncString.Result;
                deleteOpportunityResponse = JsonConvert.DeserializeObject<DeleteOpportunityResponse>(result);

                if (deleteOpportunityResponse.Success)
                {
                    foreach (DeleteOpportunityResponseResult deleteOpportunityResponseResult in deleteOpportunityResponse.Result)
                    {
                        if (!deleteOpportunityResponseResult.status.Equals("Deleted",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine("Record failed to delete. Status {0} for reason {1}", deleteOpportunityResponseResult.status, string.Join(",", deleteOpportunityResponseResult.reasons.Select(e => e.message)));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("API request failed. Detail: {0}", string.Join(",", deleteOpportunityResponse.Errors.Select(x => x.Message)));
                }
            }
            else if (response.StatusCode.Equals(606))
            {
                throw new MarketoMaxRateLimitException();
            }
            else if (response.StatusCode.Equals(615))
            {
                throw new MarketoConcurrentAccessLimitException();
            }
            else if (response.StatusCode.Equals(607))
            {
                throw new MarketoDailyQuotaReachedException();
            }
            else
            {
                throw new Exception("Unhandled exception has occured when calling DeleteOpportunity on Marketo API.");
            }

            client.Dispose();
            return deleteOpportunityResponse;
        }

        public async Task<DeleteOpportunityResponse> DeleteOpportunityRole(DeleteOpportunityRequest deleteOpportunityRequest)
        {
            DeleteOpportunityResponse deleteOpportunityResponse = null;
            string path = "rest/v1/opportunities/roles/delete.json";
            string body = JsonConvert.SerializeObject(deleteOpportunityRequest);

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(_marketoInstanceUrl)
            };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/json"));

            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetToken());

            HttpContent httpContent = new StringContent(body);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(path, httpContent);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                Task<string> asyncString = response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                string result = asyncString.Result;
                deleteOpportunityResponse = JsonConvert.DeserializeObject<DeleteOpportunityResponse>(result);

                if (deleteOpportunityResponse.Success)
                {
                    foreach (DeleteOpportunityResponseResult deleteOpportunityResponseResult in deleteOpportunityResponse.Result)
                    {
                        if (!deleteOpportunityResponseResult.status.Equals("Deleted",
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine("Record failed to delete. Status {0} for reason {1}", deleteOpportunityResponseResult.status, string.Join(",", deleteOpportunityResponseResult.reasons.Select(e=> e.message)));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("API request failed. Detail: {0}", string.Join(",",deleteOpportunityResponse.Errors.Select(x=> x.Message)));
                }
            }
            else if (response.StatusCode.Equals(606))
            {
                throw new MarketoMaxRateLimitException();
            }
            else if (response.StatusCode.Equals(615))
            {
                throw new MarketoConcurrentAccessLimitException();
            }
            else if (response.StatusCode.Equals(607))
            {
                throw new MarketoDailyQuotaReachedException();
            }
            else
            {
                throw new Exception("Unhandled exception has occured when calling DeleteOpportunityRole on Marketo API.");
            }

            client.Dispose();
            return deleteOpportunityResponse;
        }

        public async Task<MergeLeadsResponse> MergeLeads(int winningLead, int duplicateLead)
        {
            MergeLeadsResponse mergeLeadsResponse = null;
            string path = $"rest/v1/leads/{winningLead}/merge.json?leadIds={duplicateLead}";

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(_marketoInstanceUrl)
            };

            HttpContent httpContent = new StringContent("");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", GetToken());

            HttpResponseMessage response = await client.PostAsync(path, httpContent);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                Task<string> asyncString = response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                string result = asyncString.Result;
                mergeLeadsResponse = JsonConvert.DeserializeObject<MergeLeadsResponse>(result);

                if (mergeLeadsResponse.Success)
                {
                    Console.WriteLine("Record with winning lead {0} and losing lead {1} successfully merged", winningLead, duplicateLead);
                }
                else
                {
                    Console.WriteLine("API request failed with winning lead {0} and losing lead {1}", winningLead, duplicateLead);
                }
            }
            else if (response.StatusCode.Equals(606))
            {
                throw new MarketoMaxRateLimitException();
            }
            else if (response.StatusCode.Equals(615))
            {
                throw new MarketoConcurrentAccessLimitException();
            }
            else if (response.StatusCode.Equals(607))
            {
                throw new MarketoDailyQuotaReachedException();
            }
            else
            {
                throw new Exception("Unhandled exception has occured when calling DeleteOpportunity on Marketo API.");
            }

            client.Dispose();
            return mergeLeadsResponse;
        }

        public async Task<FindLeadResponse> FindLead(int marketoLeadId)
        {
            FindLeadResponse findLeadResponse = null;
            string path = $"rest/v1/lead/{marketoLeadId}.json?";

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(_marketoInstanceUrl)
            };

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", GetToken());

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                Task<string> asyncString = response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll
                string result = asyncString.Result;
                findLeadResponse = JsonConvert.DeserializeObject<FindLeadResponse>(result);

                if (findLeadResponse.Success)
                {
                    string warnings = "";
                    if (findLeadResponse.Warnings != null && findLeadResponse.Warnings.Any())
                    {
                        warnings = string.Join(",", findLeadResponse.Warnings.Select(x => x.Message));
                    }

                    if (findLeadResponse.Result.Any())
                    {
                        Console.WriteLine("Record with leadID {0} found. Warning detail: {1}", marketoLeadId, warnings);
                    }
                    else
                    {
                        Console.WriteLine("Record with leadID {0} not found. Warning detail: {1}", marketoLeadId, warnings);
                    }
                }
                else
                {
                    string errors = "";
                    if (findLeadResponse.Errors != null && findLeadResponse.Errors.Any())
                    {
                        errors = string.Join(",", findLeadResponse.Errors.Select(x => x.Message));
                    }
                    Console.WriteLine("Unable to Find Lead with leadID {0}. Error detail {1}", marketoLeadId, errors);
                }
            }
            else if (response.StatusCode.Equals(606))
            {
                throw new MarketoMaxRateLimitException();
            }
            else if (response.StatusCode.Equals(615))
            {
                throw new MarketoConcurrentAccessLimitException();
            }
            else if (response.StatusCode.Equals(607))
            {
                throw new MarketoDailyQuotaReachedException();
            }
            else
            {
                throw new Exception("Unhandled exception has occured when calling DeleteOpportunity on Marketo API.");
            }

            client.Dispose();
            return findLeadResponse;
        }

        public string GetToken()
        {
            if (!_authenticationToken.HasToken() || _authenticationToken.HasExpired())
            {
                string url = _marketoInstanceUrl + "identity/oauth/token?grant_type=client_credentials&client_id=" + _marketoClientId + "&client_secret=" + _marketoClientSecret;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(resStream);
                string json = reader.ReadToEnd();
                AuthenticationTokenResponse authenticationTokenResponse = JsonConvert.DeserializeObject<AuthenticationTokenResponse>(json);

                _authenticationToken = new AuthenticationToken()
                {
                    AccessToken = authenticationTokenResponse.Access_token,
                    Expires = DateTime.Now.AddSeconds(authenticationTokenResponse.Expires_in - 500)
                };
            }

            return _authenticationToken.AccessToken;
        }
    }
}
