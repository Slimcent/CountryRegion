using CountryRegion.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryRegion.Utilities
{
    internal static class GetRegion
    {
        private static RestClient? _client;
        private static string CountriesFileName { get; set; } = "Countries.json";
        // private static string CountriesFileName { get; set; } = "13515537b82fb331222e";
        
        internal static async Task<IEnumerable<Response?>> Countries()
        {
            List<dynamic>? objs = await GetObject(CountriesFileName);


            if (objs == null) return Array.Empty<Response?>();

            IEnumerable<Response?> response = objs.Select(o => new Response
            {
                Name = o.country.name,
                Id = o.country.id,
            }).ToList();

            return response;
        }

        internal static async Task<IEnumerable<Response?>> States(int countryId)
        {
            List<dynamic>? objs = await GetObject(CountriesFileName);

            if (objs == null) return Array.Empty<Response>();

            dynamic? states = objs.SingleOrDefault(o => o.country.id == countryId)?.country.states;

            if (states == null) return Array.Empty<Response>();

            return JsonConvert.DeserializeObject<IEnumerable<Response>>(Convert.ToString(states)) ??
                   Array.Empty<Response>();
        }

        internal static async Task<IEnumerable<Response?>> LGAs(int countryId, int stateId)
        {
            if (countryId != Constants.NigeriaCountryId) return Array.Empty<Response>();

            List<dynamic>? objs = await GetObject(CountriesFileName);

            dynamic? states = objs.SingleOrDefault(o => o.country.id == countryId)?.country.states;

            if (states == null) return Array.Empty<Response>();

            List<dynamic>? statesEnumerable = JsonConvert.DeserializeObject<List<dynamic>>(Convert.ToString(states));

            dynamic? locals = statesEnumerable.SingleOrDefault(s => s.id == stateId)?.locals;

            if (locals == null) return Array.Empty<Response>();

            return JsonConvert.DeserializeObject<IEnumerable<Response>>(Convert.ToString(locals)) ??
                   Array.Empty<Response>();

        }

        internal static async Task<Response?> Country(int id)
        {
            List<dynamic>? objs = await GetObject(CountriesFileName);

            if (objs == null) return null;

            string? countryObj = Convert.ToString(objs.SingleOrDefault(o => o.country.id == id)?.country);

            return countryObj == null ? null : JsonConvert.DeserializeObject<Response?>(countryObj);
        }


        internal static async Task<Response?> State(int countryId, int statedId)
        {
            List<dynamic>? objs = await GetObject(CountriesFileName);

            dynamic? states = objs.SingleOrDefault(o => o.country.id == countryId)?.country.states;

            if (states == null) return null;

            List<dynamic>? statesEnumerable = JsonConvert.DeserializeObject<List<dynamic>>(Convert.ToString(states));

            string? state = Convert.ToString(statesEnumerable.SingleOrDefault(s => s.id == statedId));

            return state == null ? null : JsonConvert.DeserializeObject<Response?>(state);
        }

        internal static Response? State(this List<dynamic> objs, int countryId, int statedId)
        {
            dynamic? states = objs.SingleOrDefault(o => o.country.id == countryId)?.country.states;

            if (states == null) return null;

            List<dynamic>? statesEnumerable = JsonConvert.DeserializeObject<List<dynamic>>(Convert.ToString(states));

            string? state = Convert.ToString(statesEnumerable.SingleOrDefault(s => s.id == statedId));

            return state == null ? null : JsonConvert.DeserializeObject<Response?>(state);
        }

        internal static async Task<Response?> LGA(int stateId, int? lgaId)
        {
            List<dynamic>? objs = await GetObject(CountriesFileName);

            if (objs == null) return null;

            dynamic? states = objs.SingleOrDefault(o => o.country.id == Constants.NigeriaCountryId)?.country.states;

            if (states == null) return null;

            List<dynamic>? statesEnumerable = JsonConvert.DeserializeObject<List<dynamic>>(Convert.ToString(states));

            dynamic? locals = statesEnumerable.SingleOrDefault(s => s.id == stateId)?.locals;

            if (locals == null) return null;

            List<dynamic>? localsEnumerable = JsonConvert.DeserializeObject<List<dynamic>?>(Convert.ToString(locals));

            dynamic? lga = Convert.ToString(localsEnumerable.SingleOrDefault(l => l.id == lgaId));

            return lga == null ? null : (Response?) JsonConvert.DeserializeObject<Response?>(lga);
        }

        internal static Response? LGA(this List<dynamic> objs, int stateId, int? lgaId)
        {
            if (objs == null) return null;

            dynamic? states = objs.SingleOrDefault(o => o.country.id == Constants.NigeriaCountryId)?.country.states;

            if (states == null) return null;

            List<dynamic>? statesEnumerable = JsonConvert.DeserializeObject<List<dynamic>>(Convert.ToString(states));

            dynamic? locals = statesEnumerable.SingleOrDefault(s => s.id == stateId)?.locals;

            if (locals == null) return null;

            List<dynamic>? localsEnumerable = JsonConvert.DeserializeObject<List<dynamic>?>(Convert.ToString(locals));

            dynamic? lga = Convert.ToString(localsEnumerable.SingleOrDefault(l => l.id == lgaId));

            return lga == null ? null : (Response?)JsonConvert.DeserializeObject<Response?>(lga);
        }

        private static async Task<List<object>?> GetObject(string fileName)
        {
            RestClient client = GetClient();

            RestRequest request = new RestRequest(fileName);

            IRestResponse response = await client.ExecuteGetAsync(request);

            if (!response.IsSuccessful)
                throw new Exception("Api call was not successful, check your network");

            string content = response.Content;

            return JsonConvert.DeserializeObject<List<dynamic>>(content);
        }

        internal static async Task<List<object>?> GetDump()
        {
            return await GetObject(CountriesFileName);
        }

        private static RestClient GetClient()
        {
            _client = null ?? new RestClient("https://smcore.blob.core.windows.net/countryregion/");
            // _client = null ?? new RestClient("https://api.npoint.io/");
            return _client;
        }
    }
}
