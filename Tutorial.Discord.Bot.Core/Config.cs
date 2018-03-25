using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tutorial.Discord.Bot.Core
{
    public class Config
    {
        private const string configFolder = "Resources";
        private const string configFile = "config.json";
        private static string ConfigLocation => $"{configFolder}/{configFile}";

        public static BotConfig bot;
        static Config()
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(ConfigLocation, json);
            }
            else
            {
                string json = File.ReadAllText(ConfigLocation);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string token;

        public string cmdPrefix;
    }
}
