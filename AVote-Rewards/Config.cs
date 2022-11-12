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
		public List<string> Commands = new List<string>() { "/give 'meowmere' %PLAYER% 1 legendary" };

		public string apiKey = "xxx";

		public string rewardMessage = "[Vote Rewards] %PLAYER% has voted for us and receieved a reward. Use /vote to get the same reward!";
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
