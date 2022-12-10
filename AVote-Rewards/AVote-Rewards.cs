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
        public override Version Version => new Version(1, 0, 1);

        /// <summary>
        /// The author(s) of the plugin.
        /// </summary>
        public override string Author => "Average";

        /// <summary>
        /// A short, one-line, description of the plugin's purpose.
        /// </summary>
        public override string Description => "A simple, and light-weight Vote Rewards plugin.";
        public static string apiKey;
        public Config config;

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
            if(apiKey == "xxx")
            {
                Console.WriteLine("A-VOTE REWARDS: You should probably set your api key in AVote.json in your tShock folder! If not, whatevs, you do you b.");
            }

            GeneralHooks.ReloadEvent += Reload;
            ServerApi.Hooks.GameInitialize.Register(this, GameInit);

        }

        public void GameInit(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("av.vote", Vote, "vote", "reward"));
            config = Config.Read();
            apiKey = config.apiKey;
        }
        
        public void Reload(ReloadEventArgs args)
        {
            args.Player.SendMessage("VoteRewards - API key reloaded!", Color.LightGreen);
            config = Config.Read();
            apiKey = config.apiKey;
        }

        public void Vote(CommandArgs args)
        {
            if (!args.Player.IsLoggedIn)
            {
                args.Player.SendErrorMessage("You must be logged in first!");
                return;
            }

            var isBeingUsedForTesting = false;
            if(args.Parameters.Count == 1)
            {
                if(args.Parameters[0] == "-t")
                {
                    if (args.Player.HasPermission("av.admin"))
                    {
                        isBeingUsedForTesting = true;
                        args.Player.SendInfoMessage("-t");
                    }
                }
            }

            TSPlayer Player = args.Player;

            if (checkifPlayerVoted(Player).Result == true)
            {
                if(rewardClaimed(Player).Result == true || isBeingUsedForTesting == true)
                {
                    string rewardMsg = config.rewardMessage;
                    rewardMsg = rewardMsg.Replace("%PLAYER%", Player.Name);
                    TSPlayer.All.SendMessage(rewardMsg, Microsoft.Xna.Framework.Color.LightGreen);
                    foreach (string cmd in config.Commands)
                    {
                        string newCmd = cmd.Replace("%PLAYER%", '"' + Player.Name + '"');
                        Commands.HandleCommand(TSPlayer.Server, newCmd);
                    }
                    return;
                }
                else
                {
                    Player.SendErrorMessage("You have already claimed your reward for today!");
                    return;
                }

            }

            Player.SendMessage("You haven't voted today! Head to terraria-servers.com and vote for our server page!", Color.LightGreen);
            return;
        }

        public static async Task<bool> rewardClaimed(TSPlayer player)
        {
            bool hasVoted = false;

            string voteUrl = "http://terraria-servers.com/api/?action=post&object=votes&element=claim&key=" + apiKey + "&username=" + player.Name;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(voteUrl))
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
                            else
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return hasVoted;
            }

            return hasVoted;

        }

        public static async Task<bool> checkifPlayerVoted(TSPlayer player)
        {
            bool hasVoted = false;

            string voteUrl = ($"http://terraria-servers.com/api/?object=votes&element=claim&key={apiKey}&username={player.Name}");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(voteUrl))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();

                            if (data != null)
                            {
                                if (data == "1" || data == "2")
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
                            else
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return hasVoted;
            }

            return hasVoted;

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= Reload;
                ServerApi.Hooks.GameInitialize.Deregister(this, GameInit);
            }
            base.Dispose(disposing);
        }
    }
}
