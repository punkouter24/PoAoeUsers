using Microsoft.EntityFrameworkCore;
using PoAoeUsers.Data;

namespace PoAoeUsers.Services
{
    public class ListItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://aoe4world.com/api/v0";


        public ListItemService(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // Get ListItems for a specific user
        public async Task<List<ListItem>> GetListItemsAsync(string userId)
        {
            return await _context.ListItems
                                 .Where(li => li.UserId == userId)
                                 .ToListAsync();
        }

        // Add a new ListItem
        public async Task AddListItemAsync(ListItem listItem)
        {
            _ = _context.ListItems.Add(listItem);
            _ = await _context.SaveChangesAsync();
        }

        // Update an existing ListItem
        public async Task UpdateListItemAsync(ListItem listItem)
        {
            _ = _context.ListItems.Update(listItem);
            _ = await _context.SaveChangesAsync();
        }

        // Delete a ListItem
        public async Task DeleteListItemAsync(int id)
        {
            ListItem? listItem = await _context.ListItems.FindAsync(id);
            if (listItem != null)
            {
                _ = _context.ListItems.Remove(listItem);
                _ = await _context.SaveChangesAsync();
            }
        }


        //https://aoe4world.com/api/v0/players/search?query=barbecue

        public async Task<ListItem?> GetPlayerDataAsync(string playerName, string mode)
        {
            string url = $"{_baseUrl}/players/search?query={playerName}"; // Adjust URL based on the actual API endpoint
            try
            {
                PlayerApiResponse? response = await _httpClient.GetFromJsonAsync<PlayerApiResponse>(url);
                if (response?.Players != null && response.Players.Count > 0)
                {
                    Player player = response.Players[0]; // Assuming you want the first player in the list

                    ListItem listItem = new()
                    {
                        Name = player.Name,
                        // Set UserId if necessary
                       // OneVOneRating = player.Leaderboards.ContainsKey("rm_1v1_elo") ? player.Leaderboards["rm_1v1_elo"].Rating : 0,
                      //  FourVFourRating = player.Leaderboards.ContainsKey("rm_4v4_elo") ? player.Leaderboards["rm_4v4_elo"].Rating : 0
                       Rating = player.Leaderboards.ContainsKey(mode) ? player.Leaderboards[mode].Rating : 0
                    };

                    return listItem;
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception details here
                Console.WriteLine($"Error fetching player data: {ex.Message}");
            }

            return null;
        }


        public class PlayerApiResponse
        {
            public int TotalCount { get; set; }
            public List<Player> Players { get; set; }
        }

        public class Player
        {
            public string Name { get; set; }
            public Dictionary<string, Leaderboard> Leaderboards { get; set; }
        }

        public class Leaderboard
        {
            public int Rating { get; set; }
        }
    }
}
