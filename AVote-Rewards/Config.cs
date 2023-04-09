using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TShockAPI;


namespace AVote
{
    public class Config
    {
        public List<string> Commands { get; set; } = new List<string>() { "/give meowmere %PLAYER% 1 legendary" };

        public string apiKey { get; set; } = "xxx";

        public string rewardMessage { get; set; } = "[Vote Rewards] %PLAYER% has voted for us and receieved a reward. Use /vote to get the same reward!";

        public string loginMessage { get; set; } = "You must be logged in to use this command!";

        public string alreadyClaimedMessage { get; set; } = "You have already claimed your reward for today!";

        public string haventVotedMessage { get; set; } = "You haven't voted today! Head to terraria-servers.com and vote for our server page!";

        public static Config Read()
        {
            string filepath = Path.Combine(TShock.SavePath, "AVote.json");

            try
            {
                Config config = new();

                if (!File.Exists(filepath))
                {
                    File.WriteAllText(filepath, JsonConvert.SerializeObject(config, Formatting.Indented));
                }
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filepath));


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
