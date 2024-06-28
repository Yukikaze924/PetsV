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
            // 购买页面
            AnimalArkMenu = new UIMenu("", "Animal Ark");
            AnimalArkMenu.SetBannerType(NativeUI.Sprite.WriteFileFromResources(Assembly.GetExecutingAssembly(), "PetsV.banner.png"));
            AnimalArkMenu.AddItem(new UIMenuItem("Name", "Customize your pet's name"));
            AnimalArkMenu.AddItem(new UIMenuListItem("Pets", species, 0, "Choose a pet as you like"));
            AnimalArkMenu.AddItem(new UIMenuListItem("Breeds", PetsV.Instance._petService.GetBreedListBySpecies(0), 0, "Choose a pet as you like"));
            AnimalArkMenu.AddItem(new UIMenuListItem("Gender", genders, 0, "Choose a gender for your pet"));
            AnimalArkMenu.AddItem(new UIMenuColoredItem("~w~Confirm", Color.FromArgb(133, 187, 101), Color.FromArgb(134, 220, 116)));


            // 管理页面
            SpawnMenu = new UIMenu("", "Pet Menu");
            SpawnMenu.SetBannerType(NativeUI.Sprite.WriteFileFromResources(Assembly.GetExecutingAssembly(), "PetsV.banner2.png"));
            SpawnMenu.AddItem(new UIMenuListItem("~b~Pets", PetsV.Instance._fileService.GetAllPetsFromDirectory(), 0));
            SpawnMenu.AddItem(new UIMenuItem("~g~Spawn"));
            SpawnMenu.AddItem(new UIMenuItem("~y~Refresh"));


            //
            PetMenu = new UIMenu("", "");
            PetMenu.SetBannerType(new Sprite("shopui_title_barber", "shopui_title_barber", new Point(0, 0), new Size(0, 0)));
            PetMenu.AddItem(new UIMenuItem("Name"));
            PetMenu.AddItem(new UIMenuItem("Health"));
            PetMenu.AddItem(new UIMenuItem("Gender"));
            PetMenu.AddItem(new UIMenuItem("Despawn"));
        }
    }
}
