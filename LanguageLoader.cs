using BepInEx;
using SimpleJSON;
using System.Collections.Generic;
using System.IO;

namespace MysticsItems
{
    public static class LanguageLoader
    {
        public static Dictionary<string, Dictionary<string, string>> stringsByToken = new Dictionary<string, Dictionary<string, string>>();
        public static string genericLanguageName = "generic";

        public static void Load(string filename)
        {
            string[] matchingLanguageFiles = Directory.GetFiles(Paths.PluginPath, filename, SearchOption.AllDirectories);
            if (matchingLanguageFiles.Length > 0)
            {
                JSONNode jsonNode = JSON.Parse(File.ReadAllText(matchingLanguageFiles[0]));
                if (jsonNode == null) return;
                foreach (string languageKey in jsonNode.Keys)
                {
                    JSONNode tokenValuePairs = jsonNode[languageKey];
                    if (tokenValuePairs == null) continue;

                    string languageName = languageKey;
                    if (languageName == "strings") languageName = genericLanguageName;

                    if (!stringsByToken.ContainsKey(languageName)) stringsByToken.Add(languageName, new Dictionary<string, string>());
                    foreach (string token in tokenValuePairs.Keys)
                    {
                        stringsByToken[languageName].Add(token, tokenValuePairs[token].Value);
                    }
                }
            }
            else
            {
                Main.logger.LogWarning("Missing " + filename + " file");
            }
        }

        public static string GetLoadedStringByToken(string token)
        {
            string loadedString;
            if (!stringsByToken[genericLanguageName].TryGetValue(token, out loadedString))
            {
                loadedString = token;
            }
            return loadedString;
        }

        public static void Init()
        {
            On.RoR2.Language.GetLocalizedStringByToken += (orig, self, token) =>
            {
                if (stringsByToken.ContainsKey(self.name))
                {
                    if (stringsByToken[self.name].ContainsKey(token)) return stringsByToken[self.name][token];
                }
                if (stringsByToken.ContainsKey(genericLanguageName))
                {
                    if (stringsByToken[genericLanguageName].ContainsKey(token)) return stringsByToken[genericLanguageName][token];
                }
                return orig(self, token);
            };

            On.RoR2.Language.TokenIsRegistered += (orig, self, token) =>
            {
                if (stringsByToken.ContainsKey(self.name))
                {
                    if (stringsByToken[self.name].ContainsKey(token)) return true;
                }
                if (stringsByToken.ContainsKey(genericLanguageName))
                {
                    if (stringsByToken[genericLanguageName].ContainsKey(token)) return true;
                }
                return orig(self, token);
            };
        }
    }
}