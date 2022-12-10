using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;


namespace AVote
{
	public class Config
	{
		public List<string> Commands = new List<string>() { "/give meowmere %PLAYER% 1 legendary" };

		public string apiKey { get; set; } = "xxx";

		public string rewardMessage { get; set; } = "[Vote Rewards] %PLAYER% has voted for us and receieved a reward. Use /vote to get the same reward!";

		public string loginMessage { get; set; } = "You must be logged in to use this command!";

		public string alreadyClaimedMessage { get; set; } = "You have already claimed your reward for today!";

		public string haventVotedMessage { get; set; } = "You haven't voted today! Head to terraria-servers.com and vote for our server page!";


        public void Write()
		{
			string path = Path.Combine(TShock.SavePath, "AVote.json");
			File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
		}
		public static Config Read()
		{
			string filepath = Path.Combine(TShock.SavePath, "AVote.json");

			try
			{
				Config config = new Config();

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
