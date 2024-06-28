using System.Collections.Generic;
using static PetsV.Enums.PetEnums;

namespace PetsV.Services
{
    public class BreedService
    {
        public List<object> CatList { get; private set; }
        public List<object> HuskyList { get; private set; }
        public List<object> RetrieverList { get; private set; }
        public List<object> ShepherdList { get; private set; }

        public BreedService()
        {
            CatList = new List<object>
            {
                "Tabby Cat",
                "Cow Cat",
                "Ginger Cat",
            };
            HuskyList = new List<object>
            {
                "Siberian Husky",
                "Mackenzie River Husky",
                "Alaskan Husky",
            };
            RetrieverList = new List<object>
            {
                "Golden Retriever",
                "Flat Coated Retriever",
                "Labrador Retriever",
                "Chesapeake Bay Retriever",
            };
            ShepherdList = new List<object>
            {
                "Australian Shepherd",
                "Shepherd2",
                "Shepherd3",
            };
        }
    }
}
