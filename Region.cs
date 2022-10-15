using CountryRegion.Models;
using CountryRegion.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryRegion
{
    public static class Region
    {

        public static async Task<List<dynamic>?> GetObject()
        {
            return await GetRegion.GetDump();
        }
        public static async Task<IEnumerable<Response?>> GetCountries()
        {
            return await GetRegion.Countries();
        }

        public static async Task<IEnumerable<Response?>> GetStates(int countryId)
        {
            return await GetRegion.States(countryId);
        }

        public static async Task<IEnumerable<Response?>> GetLGAs(int countryId, int stateId)
        {
            return await GetRegion.LGAs(countryId, stateId);
        }

        public static async Task<Response?> GetLGA(int statedId, int? lgaId)
        {
            return await GetRegion.LGA(statedId, lgaId);
        }

        public static async Task<Response?> GetState(int countryId, int stateId)
        {
            return await GetRegion.State(countryId, stateId);
        }

        public static async Task<Response?> GetCountry(int id)
        {
            return await GetRegion.Country(id);
        }
    }
}
