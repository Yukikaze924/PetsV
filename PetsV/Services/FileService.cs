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
        public void CreateSetupIfNotExist()
        {
            string petsFolderPath = Config.PathToPetsFolder;
            if (!Directory.Exists(petsFolderPath))
            {
                Directory.CreateDirectory(petsFolderPath);
            }
        }

        public List<object> GetAllPetsFromDirectory()
        {
            var list = new List<object>();
            var files = Directory.GetFiles(Config.PathToPetsFolder);
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
