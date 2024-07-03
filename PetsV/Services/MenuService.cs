using GTA;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using static PetsV.Enums.PetEnums;

namespace PetsV.Services
{
    public class MenuService
    {
        public UIMenu AnimalArkMenu { get; set; }

        public UIMenu SpawnMenu { get; set; }

        public UIMenu PetMenu = PetsV.Instance._menuPet;

        private readonly List<object> genders = new List<object> { Gender.Male, Gender.Female };

        private readonly List<object> species = new List<object>(Enum.GetValues(typeof(Species)).Cast<object>());

        public MenuService()
        {
            var _lang = PetsV.Instance._lang;

            // 购买页面
            AnimalArkMenu = new UIMenu("", _lang.AnimalArk);
            AnimalArkMenu.SetBannerType(NativeUI.Sprite.WriteFileFromResources(Assembly.GetExecutingAssembly(), "PetsV.Assets.banner.png"));
            AnimalArkMenu.AddItem(new UIMenuItem(_lang.Name, "Customize your pet's name"));
            AnimalArkMenu.AddItem(new UIMenuListItem(_lang.Pets, species, 0, "Choose a pet as you like"));
            AnimalArkMenu.AddItem(new UIMenuListItem(_lang.Breeds, PetsV.Instance._petService.GetBreedListBySpecies(0), 0, "Choose a pet as you like"));
            AnimalArkMenu.AddItem(new UIMenuListItem(_lang.Gender, genders, 0, "Choose a gender for your pet"));
            AnimalArkMenu.AddItem(new UIMenuColoredItem(_lang.wConfirm, Color.FromArgb(133, 187, 101), Color.FromArgb(134, 220, 116)));


            // 管理页面
            SpawnMenu = new UIMenu("", _lang.PetMenu, new Point(0,-107));
            SpawnMenu.SetBannerType(NativeUI.Sprite.WriteFileFromResources(Assembly.GetExecutingAssembly(), "PetsV.Assets.Empty.png"));
            SpawnMenu.AddItem(new UIMenuListItem(_lang.bPets, PetsV.Instance._fileService.GetAllPetsFromDirectory(), 0));
            SpawnMenu.AddItem(new UIMenuItem(_lang.gSpawn));
            SpawnMenu.AddItem(new UIMenuItem(_lang.yRefresh));


            //
            PetMenu = new UIMenu("", "INTERACT", new Point(0, -107));
            PetMenu.SetBannerType(NativeUI.Sprite.WriteFileFromResources(Assembly.GetExecutingAssembly(), "PetsV.Assets.Empty.png"));
            PetMenu.AddItem(new UIMenuItem(_lang.Name));
            PetMenu.AddItem(new UIMenuItem(_lang.Health));
            PetMenu.AddItem(new UIMenuItem(_lang.Gender));
            PetMenu.AddItem(new UIMenuItem(_lang.Despawn));
        }
    }
}
