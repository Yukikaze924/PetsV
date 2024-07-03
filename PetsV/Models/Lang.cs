using Newtonsoft.Json;

namespace PetsV.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Lang
    {
        public string AnimalArk { get; set; } = "ANIMAL ARK";
        public string Name { get; set; } = "Name";
        public string Pets { get; set; } = "Pets";
        public string Breeds { get; set; } = "Breeds";
        public string Gender { get; set; } = "Gender";
        public string wConfirm { get; set; } = "~w~Confirm";
        public string PetMenu { get; set; } = "PET MENU";
        public string bPets { get; set; } = "~b~Pets";
        public string gSpawn { get; set; } = "~g~Spawn";
        public string yRefresh { get; set; } = "~y~Refresh";
        public string Health { get; set; } = "Health";
        public string Despawn { get; set; } = "Despawn";
        public string PressKeyToPetStore { get; set; } = "Press ~INPUT_CONTEXT~ to view all available pets.";
        public string PressKeyToOpenMenu { get; set; } = "Press ~INPUT_FRONTEND_RDOWN~ to open menu.";
        public string PressKeyToExit { get; set; } = "Press ~INPUT_FRONTEND_RRIGHT~ to exit.";
        public string PressKeyToInteractWith { get; set; } = "Press ~INPUT_CONTEXT~ to interact with";
        public string PetDontFitInVeh { get; set; } = "This vehicle cannot carry pets.";
        public string PetNameEmpty { get; set; } = "You haven't filled in your pet's name yet.";
        public string PetNameExisted { get; set; } = "A pet with the same name already exists.";
        public string PetExisted { get; set; } = "is already next to you.";
        //public readonly string AnimalArk = "Animal Ark";
        //public readonly string Name = "Name";
        //public readonly string Pets = "Pets";
        //public readonly string Breeds = "Breeds";
        //public readonly string Gender = "Gender";
        //public readonly string wConfirm = "~w~Confirm";
        //public readonly string PetMenu = "Pet Menu";
        //public readonly string bPets = "~b~Pets";
        //public readonly string gSpawn = "~g~Spawn";
        //public readonly string yRefresh = "~y~Refresh";
        //public readonly string Health = "Health";
        //public readonly string Despawn = "Despawn";
        //public readonly string PressKeyToPetStore = "Press ~INPUT_CONTEXT~ to view all available pets.";
        //public readonly string PressKeyToOpenMenu = "Press ~INPUT_FRONTEND_RDOWN~ to open menu.";
        //public readonly string PressKeyToExit = "Press ~INPUT_FRONTEND_RRIGHT~ to exit.";
        //public readonly string PressKeyToInteractWith = "Press ~INPUT_CONTEXT~ to interact with";
        //public readonly string PetDontFitInVeh = "This vehicle cannot carry pets.";
        //public readonly string PetNameEmpty = "You haven't filled in your pet's name yet.";
        //public readonly string PetNameExisted = "A pet with the same name already exists.";
        //public readonly string PetExisted= "is already next to you.";
    }
}
