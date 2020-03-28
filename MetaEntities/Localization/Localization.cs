using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace MetaEntities
{
    public enum Lang
    {
        Ru,
        En
    }

    public class Localization
    {
        public readonly Lang Language;
        private Dictionary<string, string> IdToTemplate;
        private const char Delimeter = '\t';
        private static ILog log = LogManager.GetLogger(typeof(Localization));

        public Localization(Lang lang, string pathToData)
        {
            Language = lang;

            IdToTemplate = File.ReadAllLines(pathToData)
                .Select(line =>
                {
                    var sp = line.Split(Delimeter);
                    if (sp.Length != 2)
                        log.Fatal(
                            $"Incorrect line in localization: '{line}'. Lang is '{lang}', path is '{pathToData}'");
                    return new KeyValuePair<string, string>(sp[0], sp[1]);
                }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public string Localize(string templateId, params object[] args)
        {
            if (!IdToTemplate.TryGetValue(templateId, out var template))
            {
                log.Fatal($"Can't get template for lang '{Language}' and templateId '{templateId}'");
                return null;
            }

            return string.Format(template, args);
        }
    }

    public class Localizator
    {
        private static ILog log = LogManager.GetLogger(typeof(Localizator));
        private Dictionary<Lang, Localization> localizations;

        public Localizator(Dictionary<Lang, string> paths)
            => localizations = paths
                .Select(kvp => new KeyValuePair<Lang, Localization>(kvp.Key, new Localization(kvp.Key, kvp.Value)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


        public string Localize(Lang lang, string templateId, params object[] args)
        {
            if (!localizations.TryGetValue(lang, out var localization))
            {
                log.Fatal($"Can't get localization for lang '{lang}'");
                return null;
            }

            return localization.Localize(templateId, args);
        }
    }
}