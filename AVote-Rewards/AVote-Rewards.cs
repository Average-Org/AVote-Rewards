using Microsoft.Xna.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AVote
{
    /// <summary>
    /// The main plugin class should always be decorated with an ApiVersion attribute. The current API Version is 1.25
    /// </summary>
    [ApiVersion(2, 1)]
    public class AVote : TerrariaPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string Name => "AVote-Rewards";

        /// <summary>
        /// The version of the plugin in its current state.
        /// </summary>
        public override Version Version => new(1, 1);

        /// <summary>
        /// The author(s) of the plugin.
        /// </summary>
        public override string Author => "Average";

        /// <summary>
        /// A short, one-line, description of the plugin's purpose.
        /// </summary>
        public override string Description => "A simple, and light-weight Vote Rewards plugin.";
        public static Config config;

        /// <summary>
        /// The plugin's constructor
        /// Set your plugin's order (optional) and any other constructor logic here
        /// </summary>
        public AVote(Main game) : base(game)
        {

        }

        /// <summary>
        /// Performs plugin initialization logic.
        /// Add your hooks, config file read/writes, etc here
        /// </summary>
        public override void Initialize()
        {
            config = Config.Read();
            if (config.apiKey == "xxx")
                Console.WriteLine("A-VOTE REWARDS: You should probably set your api key in AVote.json in your tShock folder! If not, whatevs, you do you b.");

            GeneralHooks.ReloadEvent += Reload;
            Commands.ChatCommands.Add(new Command("av.vote", Vote, "vote", "reward"));

        }

        public void Reload(ReloadEventArgs args)
        {
            args.Player.SendMessage("VoteRewards - API key reloaded!", Color.LightGreen);
            config = Config.Read();
        }

        public async void Vote(CommandArgs args)
        {
            if (!args.Player.IsLoggedIn)
            {
                args.Player.SendErrorMessage(config.loginMessage);
                return;
            }

            var isBeingUsedForTesting = false;
            if (args.Parameters.Count == 1)
            {
                if (args.Parameters[0] == "-t")
                {
                    if (args.Player.HasPermission("av.admin"))
                    {
                        isBeingUsedForTesting = true;
                        args.Player.SendInfoMessage("-t");
                    }
                }
            }

            TSPlayer Player = args.Player;

            if (await CheckIfPlayerVoted(Player) == true)
            {
                if (await CheckIfRewardClaimed(Player) == true || isBeingUsedForTesting == true)
                {
                    string rewardMsg = config.rewardMessage;
                    rewardMsg = rewardMsg.Replace("%PLAYER%", Player.Name);
                    TSPlayer.All.SendMessage(rewardMsg, Color.LightGreen);
                    foreach (string cmd in config.Commands)
                    {
                        string newCmd = cmd.Replace("%PLAYER%", '"' + Player.Name + '"');
                        Commands.HandleCommand(TSPlayer.Server, newCmd);
                    }
                    return;
                }
                else
                {
                    Player.SendErrorMessage(config.alreadyClaimedMessage);
                    return;
                }

            }

            Player.SendMessage(config.haventVotedMessage, Color.LightGreen);
            return;
        }

        public static async Task<bool> CheckIfRewardClaimed(TSPlayer player)
        {
            bool hasVoted = false;

            string url = "http://terraria-servers.com/api/?action=post&object=votes&element=claim&key=" + config.apiKey + "&username=" + player.Name;

            try
            {
                using (HttpClient client = new())
                {
                    using (HttpResponseMessage res = await client.GetAsync(url))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();

                            if (data != null)
                            {
                                if (data == "1")
                                {
                                    hasVoted = true;
                                    return hasVoted;
                                }
                                else
                                {
                                    hasVoted = false;
                                    return hasVoted;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return hasVoted;
            }

            return hasVoted;

        }

        public static async Task<bool> CheckIfPlayerVoted(TSPlayer player)
        {
            string url = $"http://terraria-servers.com/api/?object=votes&element=claim&key={config.apiKey}&username={player.Name}";

            try
            {
                using (HttpClient client = new())
                {
                    using (HttpResponseMessage res = await client.GetAsync(url))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();

                            if (data != null)
                            {
                                if (data == "1" || data == "2")
                                    return true;
                                else
                                    return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return false;
        }

    }
}
