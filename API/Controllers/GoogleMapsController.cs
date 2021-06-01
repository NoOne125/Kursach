using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API
{

    [ApiController]
    [Route("[controller]")]
    public class GoogleMapsController : ControllerBase
    {

        private readonly ILogger<GoogleMapsController> _logger;
        private readonly MapsClient _weatherClient;
        private readonly IDynamoDbClient _dynamoDbClient;

        public GoogleMapsController(ILogger<GoogleMapsController> logger, MapsClient mapsClient, IDynamoDbClient dynamoDbClient)
        {
            _logger = logger;
            _weatherClient = mapsClient;
            _dynamoDbClient = dynamoDbClient;

        }

        [HttpGet ("find_nearby")]
        public async Task<string> Nearby(double x, double y, int radius, string type, bool sort_by_rating, bool open_now)
        {
            MapsClient Current_client = new MapsClient();
            await Current_client.Search_Nearby(x, y, radius, type, "ru", open_now);
            Current_client.Sort(sort_by_rating);
            return JsonConvert.SerializeObject(Current_client.current_list);
        }

        [HttpGet("details")]
        public async Task<string> Details(string place_id)
        {
            Full_Model local = await new MapsClient().More_Details(place_id);
            return JsonConvert.SerializeObject(local);
        }

        [HttpPost("add_place")]
        public async Task<IActionResult> AddToPlaces(string place_id, string name, double rating, string vicinity, double x, double y)
        {
            var data = new PlaceDbRepository
            {
                place_id = place_id,
                name = name,
                rating = rating,
                vicinity = vicinity,
                location = new List<double>() { x, y }
            };

            var result = await _dynamoDbClient.PostDataToDbPlace(data);

            if (result == false)
            {
                return BadRequest("Cannot insert value to database. Please see console log");
            }

            return Ok("Value has been successfully added to DB");
        }

        [HttpPost("add_user")]
        public async Task<IActionResult> AddToUsers(string chat_id)
        {
            var result = await _dynamoDbClient.PostDataToDbUser(chat_id);

            if (result == false)
            {
                return BadRequest("Cannot insert value to database. Please see console log");
            }

            return Ok("Value has been successfully added to DB");
        }

        [HttpGet("all_places")]
        public async Task<List<PlaceDbRepository>> GetAllPlace(string chat_id, string name_pl, bool route)
        {
            var list_of_id = await _dynamoDbClient.GetDataByPlaylist(chat_id, name_pl, route);

            if (list_of_id == null)
                return null;

            List<PlaceDbRepository> result = new List<PlaceDbRepository>();
            for (int i = 0; i < list_of_id.Count; i++)
            {
                var local = await _dynamoDbClient.GetDataByPlace(list_of_id[i]);
                result.Add(local);
            }
            return result;
        }

        [HttpGet("all_playlists")]
        public async Task<List<string>> GetAllPlaylists(string chat_id, bool route)
        {
            var list_of_names = await _dynamoDbClient.GetDataByUser(chat_id, route);

            if (list_of_names == null)
                return null;

            return list_of_names;
        }

        [HttpPut("add_or_delete_place")]
        public async Task<IActionResult> Remove_or_add_place(string chat_id, string playlist, string place_id, bool route, bool delete)
        {
            var result = await _dynamoDbClient.UpdateDataIntoDbPlaylist(chat_id, playlist, place_id, route, delete);

            if (result == false)
            {
                return BadRequest("Уже содержит в себе этот элемент");
            }

            return Ok("Успешно добавленно");
        }

        [HttpPut("add_or_delete_playlist")]
        public async Task<IActionResult> Remove_or_add_play(string chat_id, string playlist, bool route, bool delete)
        {
            if (playlist != "Любимые")
            {
                var result = await _dynamoDbClient.UpdateDataIntoDbUser(chat_id, playlist, route, delete);

                if (result == false)
                {
                    return BadRequest("Cannot insert value to database. Please see console log");
                }

                return Ok("Value has been successfully added to DB");
            }
            else return BadRequest("Cannot insert value to database. Please see console log");
        }
    }
}
