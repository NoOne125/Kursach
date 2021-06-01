using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace API
{
    public class MapsClient
    {
        private readonly HttpClient _client;
        private static string _adress;
        private static string _apikey;
        public List_Read_Models current_list;

        public MapsClient()
        {
            _adress = Constants.adress;
            _apikey = Constants.apikey;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_adress);
        }

        public async Task Search_Nearby(double x, double y, int radius, string type, string language, bool open_now)
        {
            string str = "";
            if (open_now)
                str = "&opennow=true";
            var response = await _client.GetAsync($"place/nearbysearch/json?location={x.ToString().Replace(",", ".")},{y.ToString().Replace(",", ".")}&language={language}&radius={radius}&type={type}&key={_apikey}{str}");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            current_list = JsonConvert.DeserializeObject<List_Read_Models>(content);
        }

        public void Sort(bool sort_by_rating)
        {
            List<Reading_Model> str = new List<Reading_Model>();
            for (int i = 0; i < current_list.results.Count; i++)
            {
                Reading_Model local = new Reading_Model();
                if (sort_by_rating)
                {
                    int i_max = 0;
                    double max = 0;
                    for (int j = 0; j < current_list.results.Count; j++)
                    {
                        local = current_list.results[j];
                        if (max < local.rating)
                        {
                            i_max = j;
                            max = local.rating;
                        }
                    }
                    local = current_list.results[i_max];
                    str.Add(local);
                    current_list.results.RemoveAt(i_max);
                }
                else
                {
                    local = current_list.results[i];
                    str.Add(local);

                }
            }
            current_list.results = str;
        }

        public async Task<Full_Model> More_Details(string place_id)
        {
            var response = await _client.GetAsync($"place/details/json?place_id={place_id}&language=ru&fields=name,vicinity,rating,opening_hours,international_phone_number,website,review&key={_apikey}");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Full_Model>(content);
        }
    }
}
