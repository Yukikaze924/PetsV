using Newtonsoft.Json;
using PetsV.Configs;
using PetsV.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PetsV.Services
{
    public class FileService
    {
        private readonly string _configFilePath=Configuration.PathToModFolder+"\\config.json";

        public Config Initialize()
        {
            string petsFolderPath = Configuration.PathToPetsFolder;
            string langFolderPath = Configuration.PathToLangFolder;
            if (!Directory.Exists(petsFolderPath))
            {
                Directory.CreateDirectory(petsFolderPath);
            }
            if (!Directory.Exists(langFolderPath))
            {
                Directory.CreateDirectory(langFolderPath);
            }

            if (!File.Exists(_configFilePath))
            {
                File.WriteAllText(_configFilePath, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            }

            var configText = File.ReadAllText(_configFilePath);
            return JsonConvert.DeserializeObject<Config>(configText);
        }

        public Lang GetI18nLang(string language)
        {
            var fileName = $"{language}.json";
            var dI = new DirectoryInfo(Configuration.PathToLangFolder);
            var files = dI.GetFiles(language+".json");
            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    if (file.Name == fileName)
                    {
                        return JsonConvert.DeserializeObject<Lang>(File.ReadAllText(file.FullName));
                    }
                }
            }
            var lang = new Lang();
            File.WriteAllText($@"{Configuration.PathToLangFolder}\{fileName}",
                            JsonConvert.SerializeObject(lang, Formatting.Indented)
                            );
            return lang;
        }

        public List<object> GetAllPetsFromDirectory()
        {
            var list = new List<object>();
            var files = Directory.GetFiles(Configuration.PathToPetsFolder);
            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    string fileExtension = Path.GetExtension(file);
                    if (fileExtension == ".json")
                    {
                        string filePath = Path.GetFullPath(file);
                        string json = File.ReadAllText(filePath);
                        Pet pet = JsonConvert.DeserializeObject<Pet>(json);
                        if (pet.Status == Enums.PetEnums.Status.Dead)
                        {
                            continue;
                        }
                        list.Add(pet.Name);
                    }
                }
            }
            if (list.Count == 0)
            {
                list.Add("");
            }

            return list;
        }
    }
}
