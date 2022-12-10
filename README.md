# Average's Vote Rewards
Support me & this plugin's (along with several others) development on Ko.Fi: [Here!](https://ko-fi.com/averageterraria)

A super simple and lightweight tShock V5 plugin. Allows the user to execute /vote, which checks if the player has voted or not. If the player has voted, they are given rewards. And yes, don't worry, there is a check to make sure the reward can't be claimed more than once per 24 hours!

Permission node for /vote: `av.vote`

### Config Explained
(tshock/AVote.json)

```json
{
  "Commands": [
    '/give "meowmere" %PLAYER% 1 legendary'
  ],
  "apiKey": "xxx",
  "rewardMessage": "[Vote Rewards] %PLAYER% has voted for us and receieved a reward. Use /vote to get the same reward!"
}

```

The `Commands` field is where you can add a list of commands, with `%PLAYER` being replaced with the player's name. This allows the plugin to be highly versatile and does not **DIRECTLY** depend on any other plugins (*cough* TSReward and SEConomy *cough*). For example, you could /rocket the player and then /givebal (or whatever your economy command is). Or literally whatever! It can be anything :D

The `apiKey` is the API key provided by terraria-servers.com, **this is necessary** to check if your player has voted or not.

The `rewardMessage` is the message that is sent to all players once a player has voted! The %PLAYER% placeholder also works here!

Thanks for reading :D
