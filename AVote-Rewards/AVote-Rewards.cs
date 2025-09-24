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
        public static Config Config;

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
            Config = Config.Read();
            if (Config.ApiKey == "xxx")
                Console.WriteLine("A-VOTE REWARDS: You should probably set your api key in AVote.json in your tShock folder! If not, whatevs, you do you b.");

            GeneralHooks.ReloadEvent += Reload;
            Commands.ChatCommands.Add(new Command("av.vote", Vote, "vote", "reward"));

        }

        private static void Reload(ReloadEventArgs args)
        {
            args.Player.SendMessage("VoteRewards - Config reloaded!", Color.LightGreen);
            Config = Config.Read();
        }

        public async void Vote(CommandArgs args)
        {
            if (!args.Player.IsLoggedIn)
            {
                args.Player.SendErrorMessage(Config.LoginMessage);
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
                        args.Player.SendInfoMessage("Testing mode enabled. Rewards will be given regardless of voting/claiming status.");
                    }
                }
            }

            var player = args.Player;
            var playerHasVotedResult = await CheckIfPlayerVoted(player);
            var playerHasClaimedResult = await CheckIfRewardClaimed(player);
            
            if (playerHasVotedResult || isBeingUsedForTesting)
            {
                if (playerHasClaimedResult || isBeingUsedForTesting)
                {
                    var rewardMsg = Config.RewardMessage;
                    rewardMsg = rewardMsg.Replace("%PLAYER%", player.Name);
                    TSPlayer.All.SendMessage(rewardMsg, Color.LightGreen);
                    foreach (var cmd in Config.Commands)
                    {
                        var newCmd = cmd.Replace("%PLAYER%", '"' + player.Name + '"');
                        Commands.HandleCommand(TSPlayer.Server, newCmd);
                    }

                    if (isBeingUsedForTesting)
                    {
                        player.SendInfoMessage("Has claimed vote? " + playerHasClaimedResult);
                        player.SendInfoMessage("Has voted? " + playerHasVotedResult);
                    }
                    return;
                }

                player.SendErrorMessage(Config.AlreadyClaimedMessage);
                return;
            }
            
            player.SendMessage(Config.HaventVotedMessage, Color.LightGreen);
        }

        public static async Task<bool> CheckIfRewardClaimed(TSPlayer player)
        {
            bool hasVoted = false;

            string url = "http://terraria-servers.com/api/?action=post&object=votes&element=claim&key=" + Config.ApiKey + "&username=" + player.Name;

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
            string url = $"http://terraria-servers.com/api/?object=votes&element=claim&key={Config.ApiKey}&username={player.Name}";

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
