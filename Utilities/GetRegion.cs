using CountryRegion.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryRegion.Utilities
{
    internal static class GetRegion
    {
        private static RestClient? _client;
        //private static string CountriesFileName { get; set; } = "v1657709078/ez3kg2u8l5uqs3tiluy5.json";
        private static string CountriesFileName { get; set; } = "Countries.json";
        //private static string NGSatesAndLGAsFileName { get; set; } = "v1663328828/dy1aiwk8vnsgop9v607y.json";
        private static string NGSatesAndLGAsFileName { get; set; } = "NigerianStates.json";
               
        internal static async Task<IEnumerable<Response?>> Countries()
        {
            dynamic? objs = await GetObject(CountriesFileName);

            IList<Response> responses = new List<Response>();

            if (objs == null) return responses;

            foreach (var obj in objs)
            {
                string country = Convert.ToString(obj.country);

                Response? countryResponse = JsonConvert.DeserializeObject<Response>(country);

                if (countryResponse != null) responses.Add(countryResponse);
            }

            return responses;
        }

        internal static async Task<IEnumerable<Response?>> States(int countryId)
        {
            dynamic? objs = await GetObject(CountriesFileName);

            if (objs == null) return Array.Empty<Response>();

            foreach (var obj in objs)
            {
                string country = Convert.ToString(obj.country);

                Response? countryResponse = JsonConvert.DeserializeObject<Response>(country);

                if (countryResponse?.Id == countryId)
                {
                    string state = Convert.ToString(obj.country.state);

                    return JsonConvert.DeserializeObject<IEnumerable<Response>>(state) ??
                           Array.Empty<Response>();
                }
            }

            return Array.Empty<Response>();
        }        

        internal static async Task<IEnumerable<Response?>> LGAs(int countryId, int stateId)
        {
            if (countryId != 160) return Array.Empty<Response>();

            dynamic? objs = await GetObject(NGSatesAndLGAsFileName);

            if (objs == null) return Array.Empty<Response>();

            foreach (var obj in objs)
            {
                int _stateId = obj.state.id;


                if (_stateId == stateId)
                {
                    string LGAs = Convert.ToString(obj.state.locals);

                    return JsonConvert.DeserializeObject<IEnumerable<Response>>(LGAs) ??
                           Array.Empty<Response>();
                }
            }

            return Array.Empty<Response>();
        }

        internal static async Task<Response?> Country(int id)
        {
            dynamic? objs = await GetObject(CountriesFileName);

            if (objs == null) return null;

            foreach (var obj in objs)
            {
                int countryId = obj.country.id;
                string country = Convert.ToString(obj.country);

                if (countryId == id)
                {
                    return JsonConvert.DeserializeObject<Response>(country);
                }
            }

            return null;
        }


        internal static async Task<Response?> State(int countryId, int statedId)
        {
            dynamic objs = await GetObject(CountriesFileName);

            if (objs == null) return null;

            foreach (var obj in objs)
            {
                if (obj.country.id != countryId) continue;

                dynamic state = obj.country.state;

                foreach (var stateObj in state)
                {
                    if (stateObj.id != statedId) continue;

                    string _state = Convert.ToString(stateObj);

                    return JsonConvert.DeserializeObject<Response>(_state);
                }
            }

            return null;
        }


        internal static async Task<Response?> LGA(int statedId, int? lgaId)
        {
            dynamic? objs = await GetObject(NGSatesAndLGAsFileName);

            if (objs == null) return null;

            foreach (var obj in objs)
            {
                if (obj.state.id != statedId) continue;

                dynamic local = obj.state.locals;

                foreach (var localObj in local)
                {
                    if (localObj.id != lgaId) continue;

                    string _local = Convert.ToString(localObj);

                    return JsonConvert.DeserializeObject<Response>(_local);
                }
            }

            return null;
        }

        private static async Task<dynamic?> GetObject(string fileName)
        {
            var client = GetClient();

            var request = new RestRequest(fileName);

            var response = await client.ExecuteGetAsync(request);

            if (!response.IsSuccessful)
                throw new Exception("Api call was not successful, check your network");

            string content = response.Content;

            return JsonConvert.DeserializeObject<dynamic>(content);
        }

        private static RestClient GetClient()
        {
            _client = null ?? new RestClient("https://smcore.blob.core.windows.net/countryregion/");

            //_client = null ?? new RestClient("https://res.cloudinary.com/https-tenece-com/raw/upload/");

            return _client;
        }
    }
}
