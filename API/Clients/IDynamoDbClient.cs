using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API;

namespace API
{
    public interface IDynamoDbClient
    {
        public Task<List<string>> GetDataByUser(string chat_id, bool route);
        public Task<List<string>> GetDataByPlaylist(string chat_id, string name_pl, bool route);
        public Task<PlaceDbRepository> GetDataByPlace(string place_id);
        public Task<bool> PostDataToDbUser(string chat_id);
        public Task<bool> PostDataToDbPlace(PlaceDbRepository data);
        public Task<bool> UpdateDataIntoDbPlaylist(string chat_id, string playlist, string place_id, bool route, bool delete);
        public Task<bool> UpdateDataIntoDbUser(string chat_id, string playlist, bool route, bool delete);
    }
}
