using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TShockAPI;


namespace AVote
{
    public class Config
    {
        public string[] Commands { get; set; } = { "/give meowmere %PLAYER% 1 legendary" };

        [JsonProperty("apiKey")]
        public string ApiKey { get; set; } = "xxx";

        [JsonProperty("rewardMessage")]
        public string RewardMessage { get; set; } = "[Vote Rewards] %PLAYER% has voted for us and receieved a reward. Use /vote to get the same reward!";

        [JsonProperty("loginMessage")]
        public string LoginMessage { get; set; } = "You must be logged in to use this command!";

        [JsonProperty("alreadyClaimedMessage")]
        public string AlreadyClaimedMessage { get; set; } = "You have already claimed your reward for today!";

        [JsonProperty("haventVotedMessage")]
        public string HaventVotedMessage { get; set; } = "You haven't voted today! Head to terraria-servers.com and vote for our server page!";

        public static Config Read()
        {
            string filepath = Path.Combine(TShock.SavePath, "AVote.json");

            try
            {
                if (!File.Exists(filepath))
                {
                    var defaultConfig = new Config();
                    File.WriteAllText(filepath, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
                    return defaultConfig;
                }
                
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filepath)) ?? new Config();
                return config;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.ToString());
                return new Config();
            }
        }
    }
}
