using GTA;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using static PetsV.Enums.PetEnums;

namespace PetsV.Services
{
    public class PetService
    {
        private readonly BreedService _breedService;
        private readonly List<object> cat_list;
        private readonly List<object> husky_list;
        private readonly List<object> retriever_list;
        private readonly List<object> shepherd_list;

        public PetService()
        {
            _breedService = new BreedService();
            cat_list = _breedService.CatList;
            husky_list = _breedService.HuskyList;
            retriever_list = _breedService.RetrieverList;
            shepherd_list = _breedService.ShepherdList;
        }

        public string GetModelName(Species species)
        {
            switch (species)
            {
                case Species.Cat:
                    return "a_c_cat_01";
                case Species.Husky:
                    return "a_c_husky";
                case Species.Retriever:
                    return "a_c_retriever";
                case Species.Shepherd:
                    return "a_c_shepherd";
            }

            return default;
        }

        public List<object> GetBreedListBySpecies(Species species)
        {
            switch (species)
            {
                case Species.Cat:
                    return cat_list;
                case Species.Husky:
                    return husky_list;
                case Species.Retriever:
                    return retriever_list;
                case Species.Shepherd:
                    return shepherd_list;
                default:
                    return default;
            }
        }

        public int GetPrice(Species species)
        {
            switch (species)
            {
                case Species.Cat:
                    return 1500;
                case Species.Husky:
                    return 3000;
                case Species.Retriever:
                    return 3500;
                case Species.Shepherd:
                    return 4500;
                default:
                    return 1000;
            }
        }

        public bool CanPedAttack(Ped ped)
        {
            if (ped.Model == PedHash.Cat) return false;
            else return true;
        }
    }
}
