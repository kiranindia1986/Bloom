using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Bloom.Models
{
    public static class TagValidator
    {
        public static List<string> ValidateTags(string input)
        {
            var errors = new List<string>();
            var tagRegex = new Regex(@"\{(?<tagName>[a-zA-Z]+):(?<content>[^\}]*)\}", RegexOptions.Compiled);
            var matches = tagRegex.Matches(input);

            foreach (Match match in matches)
            {
                string tagName = match.Groups["tagName"].Value.ToLower();
                string content = match.Groups["content"].Value;

                switch (tagName)
                {
                    case "image":
                        if (!ValidateImageTag(content)) errors.Add($"Invalid {{image}} tag: {match.Value}");
                        break;
                    case "control":
                        if (!ValidateControlTag(content)) errors.Add($"Invalid {{control}} tag: {match.Value}");
                        break;
                    case "building":
                        if (!ValidateBuildingTag(content)) errors.Add($"Invalid {{building}} tag: {match.Value}");
                        break;
                    case "resources":
                        if (!ValidateResourceTag(content)) errors.Add($"Invalid {{resources}} tag: {match.Value}");
                        break;
                    case "alien":
                        if (!ValidateAlienTag(content)) errors.Add($"Invalid {{alien}} tag: {match.Value}");
                        break;
                    case "spawner":
                        if (!ValidateSpawnerTag(content)) errors.Add($"Invalid {{spawner}} tag: {match.Value}");
                        break;
                    case "powerup":
                        if (!ValidatePowerupTag(content)) errors.Add($"Invalid {{powerup}} tag: {match.Value}");
                        break;
                    case "winning":
                        if (!ValidateWinningTag(content)) errors.Add($"Invalid {{winning}} tag: {match.Value}");
                        break;
                    case "weather":
                        if (!ValidateWeatherTag(content)) errors.Add($"Invalid {{weather}} tag: {match.Value}");
                        break;
                    case "carrying":
                        if (!ValidateCarryingTag(content)) errors.Add($"Invalid {{carrying}} tag: {match.Value}");
                        break;
                    case "powercreatures":
                    case "plants":
                    case "watered":
                    case "harvested":
                    case "kills":
                    case "relics":
                        if (!ValidateNumericTag(content)) errors.Add($"Invalid {{{tagName}}} tag: {match.Value}");
                        break;
                    default:
                        errors.Add($"Unknown tag: {match.Value}");
                        break;
                }
            }
            return errors;
        }

        private static bool ValidateImageTag(string content) => content.Split(':').Length == 2 && int.TryParse(content.Split(':')[1], out _);
        private static bool ValidateControlTag(string content) => new[] { "acceleratekey", "zoomin", "zoomout", "missilekey", "laserkey", "activatekey" }.Contains(content.ToLower());
        private static bool ValidateBuildingTag(string content) => content.Split(':').Length == 2 && int.TryParse(content.Split(':')[1], out _);
        private static bool ValidateResourceTag(string content) => content.Split(':').Length == 2 && int.TryParse(content.Split(':')[1], out _);
        private static bool ValidateAlienTag(string content) => new[] { "yellowbird", "pinkbird", "assassin", "greenpirate", "worm", "yellowbirdmother", "minemother", "mine", "crab", "jelly", "pulsar" }.Contains(content.Split(':')[0].ToLower());
        private static bool ValidateSpawnerTag(string content) => content.Split(':').Length == 3 && new[] { "wormery", "firewormery", "crittery", "spidery" }.Contains(content.Split(':')[0].ToLower()) && int.TryParse(content.Split(':')[1], out _) && content.Split(':')[2].Contains(',');
        private static bool ValidatePowerupTag(string content) => content.Split(':').Length == 2 && content.Split(':')[1].Contains(',');
        private static bool ValidateWinningTag(string content) => new[] { "kills", "survival", "plants", "watered", "harvested", "relics" }.Contains(content.Split(':')[0].ToLower()) && (content.Split(':').Length == 1 || int.TryParse(content.Split(':')[1], out _));
        private static bool ValidateWeatherTag(string content) => content.Split(':').Length == 2 && new[] { "rain", "snow", "hail" }.Contains(content.Split(':')[0].ToLower()) && int.TryParse(content.Split(':')[1], out _);
        private static bool ValidateCarryingTag(string content) => new[] { "water", "seeds", "lava", "mineral", "wood", "crystal", "firefruit", "crab", "jelly" }.Contains(content.ToLower());
        private static bool ValidateNumericTag(string content) => int.TryParse(content, out _);
    }
}
