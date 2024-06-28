using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using iFruitAddon2;
using NativeUI;
using Newtonsoft.Json;
using PetsV.Configs;
using PetsV.Models;
using PetsV.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static PetsV.Enums.PetEnums;

namespace PetsV
{
    public class PetsV : Script
    {
        protected internal FileService _fileService;
        protected internal MenuService _menuService;
        protected internal PetService _petService;
        internal CustomiFruit _iFruit;
        internal MenuPool _pool;
        internal UIMenu _menuAnimalArk;
        internal UIMenu _menuPetSpawner;
        internal UIMenu _menuPet;
        internal Blip _petShopFrontDoorBlip;
        internal Pet _currentPetSetup;
        internal bool _isOnscreenKeyboardEditing;
        internal ObservableCollection<Pet> _spawnedPets;
        internal Ped _player = Game.Player.Character;
        internal Camera _camera;
        internal Vector3 _petShowcasePos = new Vector3(552, 2807, 41.5f);
        internal Ped _currentShowcaseModel;
        private readonly float _heading = 20f;
        private bool _isPlayerInShopMenu;
        private bool _isPlayerDrivingWithPets;
        private bool _isPlayerInPetMenu;

        public static PetsV Instance;

        public PetsV()
        {
            /**
             * Initialization
             */
            _fileService = new FileService();
            _fileService.CreateSetupIfNotExist();
            _petService = new PetService();
            _currentPetSetup = new Pet();
            _spawnedPets = new ObservableCollection<Pet>();
            Instance = this;


            /**
             * For menus
             */
            _menuService = new MenuService();
            _pool = new MenuPool();
            // menu1
            _menuAnimalArk = _menuService.AnimalArkMenu;
            _menuAnimalArk.MenuItems.Last().SetRightLabel($@"~HUD_COLOUR_HEIST_BACKGROUND~${_petService.GetPrice(0)}");
            _menuAnimalArk.OnItemSelect += OnAnimalArkMenuItemSelect;
            _menuAnimalArk.OnListChange += OnAnimalArkMenuListChanged;
            // menu2
            _menuPetSpawner = _menuService.SpawnMenu;
            _menuPetSpawner.OnItemSelect += OnSpawnMenuItemSelect;
            // menu3
            _menuPet = _menuService.PetMenu;
            _menuPet.OnItemSelect += OnPetMenuItemSelect;
            // pool
            _pool.Add(_menuAnimalArk);
            _pool.Add(_menuPetSpawner);
            _pool.Add(_menuPet);
            _pool.RefreshIndex();


            /**
             * For contacts
             */
            //_iFruit = new CustomiFruit();
            //iFruitContact petShop = new iFruitContact("Animal Ark");
            //petShop.Answered += ContactAnswered;
            //petShop.DialTimeout = 4000;
            //petShop.Active = true;
            //petShop.Icon = ContactIcon.Blank;
            //_iFruit.Contacts.Add(petShop);


            /**
             * For blips
             */
            _petShopFrontDoorBlip = World.CreateBlip(Config.PetShopPos);
            if (_petShopFrontDoorBlip.Exists())
            {
                _petShopFrontDoorBlip.Sprite = BlipSprite.Chop;
                _petShopFrontDoorBlip.Color = BlipColor.WhiteNotPure;
                _petShopFrontDoorBlip.Name = "Animal Ark";
            }


            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAborted;
        }

        private void OnTick(object sender, EventArgs e)
        {
            _pool.ProcessMenus();
            //_iFruit.Update();

            // 检测玩家进入菜单触发点区域
            if (_petShopFrontDoorBlip.Position.DistanceTo(_player.Position) < 1.5f)
            {
                if (!_pool.IsAnyMenuOpen())
                {
                    if (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) != 0)
                    {
                        if (!_isPlayerInShopMenu)
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to view all available pets.");
                            if (Game.IsControlJustPressed(GTA.Control.Context))
                            {
                                _camera = World.CreateCamera(Config.ShowcaseCameraPos, Config.ShowcaseCameraRot, Config.ShowcaseCameraFov);
                                _currentShowcaseModel = World.CreatePed(new Model(_petService.GetModelName(_currentPetSetup.Species)), _petShowcasePos, _heading);
                                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _currentShowcaseModel, 0, 0, _currentPetSetup.Breed, 0);
                                _isPlayerInShopMenu = true;
                                World.RenderingCamera = _camera;
                                _menuAnimalArk.Visible = true;
                                Hud.IsRadarVisible = false;
                            }
                        }
                    }
                }
            }
            if (_isPlayerInShopMenu)
            {
                if (!_pool.IsAnyMenuOpen())
                    GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_FRONTEND_RDOWN~ to open menu.\nPress ~INPUT_FRONTEND_RRIGHT~ to exit.", beep: false);
            }

            //检测玩家是否死亡、更换主角或Ped模型
            if (_player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_SWITCH_IN_PROGRESS))
            {
                if (_spawnedPets.Count > 0)
                {
                    for (int i = 0; i < _spawnedPets.Count; i++)
                    {
                        _spawnedPets[i].Entity.Delete();
                        _spawnedPets.RemoveAt(i);
                    }
                }
                _player = Game.Player.Character;
            }

            // 如果有玩家生成的宠物
            if (_spawnedPets.Count > 0)
            {
                if (!_player.IsInVehicle())
                {
                    if (!_menuPet.Visible)
                    {
                        // 则循环检测每一只宠物是否离玩家的距离小于2米
                        for (int i = 0; i < _spawnedPets.Count; i++)
                        {
                            if (_spawnedPets[i].Entity.Position.DistanceTo(_player.Position) < 2f)
                            {
                                GTA.UI.Screen.ShowHelpTextThisFrame($"Press ~INPUT_CONTEXT~ to interact with ~b~{_spawnedPets[i].Name}~w~.");
                                if (Game.IsControlJustPressed(GTA.Control.Context))
                                {
                                    var pet = _spawnedPets[i];
                                    _menuPet.MenuItems.First().SetRightLabel(pet.Name);
                                    _menuPet.MenuItems[1].SetRightLabel(pet.Entity.Health.ToString());
                                    _menuPet.MenuItems[2].SetRightLabel(pet.Entity.Gender.ToString());
                                    _menuPet.Visible = true;
                                }
                            }
                        }
                    }
                }
            }
            if (_isPlayerInPetMenu)
            {

            }

            // 如果有玩家生成的宠物
            // * 在所有设计收回宠物、删除宠物的检查中，这个判断式将放在最下面，不然 Ped.Delete() 也会触发 Ped.isDead=true
            //if (_spawnedPets.Count > 0)
            //{
            //    // 循环检测每一只的状态
            //    for (int i = 0; i < _spawnedPets.Count; i++)
            //    {
            //        // 如果有死亡的，则会将其Status设置为Dead，并且进行消档
            //        if (_spawnedPets[i].Entity.IsDead)
            //        {
            //            _spawnedPets[i].Status = Status.Dead;
            //            string json = JsonConvert.SerializeObject(_spawnedPets[i], Formatting.Indented);
            //            File.WriteAllText($@"{Config.PathToPetsFolder}\{_spawnedPets[i].Name}.json", json);
            //            _spawnedPets.RemoveAt(i);
            //            UIMenuListItem list = (UIMenuListItem)_menuPetSpawner.MenuItems.First();
            //            list.Items = _fileService.GetAllPetsFromDirectory();
            //        }
            //    }
            //}

            // 宠物进入、离开载具时
            if (_spawnedPets.Count > 0)
            {
                if (_player.IsInVehicle())
                {
                    Vehicle vehicle = _player.CurrentVehicle;
                    switch (vehicle.Type)
                    {
                        case VehicleType.Automobile:
                            foreach (var pet in _spawnedPets)
                            {
                                pet.Entity.SetIntoVehicle(vehicle, VehicleSeat.Passenger);
                                //pet.Entity.Task.PlayAnimation("creatures@retriever@amb@world_dog_sitting@base", "base");
                                pet.Entity.SetNoCollision(pet.Entity.CurrentVehicle, true);

                                Vector3 offset = new Vector3(0.0f, 0.0f, -0.3f); // Example offset (adjust as needed)
                                Vector3 pedPosition = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, pet.Entity, true);
                                Vector3 vehiclePosition = Function.Call<Vector3>(Hash.GET_ENTITY_COORDS, pet.Entity.CurrentVehicle, true);
                                Vector3 relativePosition = pedPosition - vehiclePosition;
                                Vector3 newPosition = vehiclePosition + offset + relativePosition;
                                Function.Call(Hash.SET_ENTITY_COORDS, pet.Entity, newPosition.X, newPosition.Y, newPosition.Z, true, true, true, false);
                                if (!_isPlayerDrivingWithPets) _isPlayerDrivingWithPets = true;
                            }
                            break;

                        default:
                            GTA.UI.Screen.ShowSubtitle("This vehicle cannot carry pets.", 1000);
                            break;
                    }
                }
            }
            if (_spawnedPets.Count > 0)
            {
                if (_isPlayerDrivingWithPets)
                {
                    if (!_player.IsInVehicle())
                    {
                        foreach (Pet pet in _spawnedPets)
                        {
                            pet.Entity.Position = _player.Position + _player.ForwardVector * 2;
                            //pet.Entity.SetNoCollision(veh, false);
                            //pet.Entity.Task.ClearAnimation("creatures@retriever@amb@world_dog_sitting@base", "base");
                            _isPlayerDrivingWithPets = false;
                        }
                    }
                }
            }

            /**
             * 1: User has finished editing
             */
            // * 此处可能存在与其他使用了文本框功能脚本的潜在冲突
            if (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) == 1)
            {
                // 如果没有菜单是开启状态
                if (!_pool.IsAnyMenuOpen())
                {
                    // 如果文本输入框是由本脚本唤出的
                    if (_isOnscreenKeyboardEditing == true)
                    {
                        // 使用原生c函数拿到文本框的const char*指针并转为C# 字符串
                        var result = Function.Call<string>(Hash.GET_ONSCREEN_KEYBOARD_RESULT);
                        _currentPetSetup.Name = result;
                        _menuAnimalArk.MenuItems.First().SetRightLabel(result);
                        // 恢复菜单并且结束编辑状态
                        _menuAnimalArk.Visible = true;
                        _isOnscreenKeyboardEditing = false;
                    }
                    // 反之则无视
                }
            }

            /**
             * 2: User has canceled editing
             */
            // 与上面同理
            if (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) == 2)
            {
                if (_isOnscreenKeyboardEditing == true)
                {
                    _menuAnimalArk.Visible = true;
                    _isOnscreenKeyboardEditing = false;
                }
            }
        }

        private void OnAnimalArkMenuItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            // Name
            if (selectedItem.Text == "Name")
            {
                _pool.CloseAllMenus();
                Wait(100);
                Function.Call(Hash.DISPLAY_ONSCREEN_KEYBOARD, 6, "FMMC_KEY_TIP8", "", "", "", "", "", 60);
                _isOnscreenKeyboardEditing = true;
                return;
            }

            // Confirm payment
            if (selectedItem.Text == "~w~Confirm")
            {
                if (_currentPetSetup.Name == null)
                {
                    GTA.UI.Screen.ShowSubtitle("You haven't filled in your pet's name yet.");
                    return;
                }
                if (File.Exists($@"{Config.PathToPetsFolder}\{_currentPetSetup.Name}.json"))
                {
                    GTA.UI.Screen.ShowSubtitle("A pet with the same name already exists.");
                    return;
                }
                try
                {
                    _currentPetSetup.Model = _petService.GetModelName(_currentPetSetup.Species);
                    string json = JsonConvert.SerializeObject(_currentPetSetup, Formatting.Indented);
                    File.WriteAllText($@"{Config.PathToPetsFolder}\{_currentPetSetup.Name}.json", json);
                }
                catch (Exception e)
                {
                    throw e;
                }
                UIMenuListItem list = (UIMenuListItem)_menuPetSpawner.MenuItems.First();
                list.Items = _fileService.GetAllPetsFromDirectory();
                Game.Player.Money -= _petService.GetPrice(_currentPetSetup.Species);
                Audio.PlaySoundAt(_player, "PROPERTY_PURCHASE", "HUD_AWARDS");
                Function.Call(Hash.ANIMPOSTFX_PLAY, "WeaponUpgrade", 5000, false);
                BigMessageThread.MessageInstance.ShowColoredShard("Purchased", $@"~b~{_currentPetSetup.Name} ~w~is now yours.", HudColor.HUD_COLOUR_BLACK, HudColor.HUD_COLOUR_GOLD);
            }
        }

        private void OnAnimalArkMenuListChanged(UIMenu sender, UIMenuListItem listItem, int newIndex)
        {
            if (listItem.Text == "Gender")
            {
                // 保存性别设置
                _currentPetSetup.Gender = (Gender)newIndex;
                return;
            }
            if (listItem.Text == "Pets")
            {
                // 保存物种设置
                _currentPetSetup.Species = (Species)newIndex;
                // 更新Breed列表
                _menuAnimalArk.MenuItems.Last().SetRightLabel($@"~HUD_COLOUR_HEIST_BACKGROUND~${_petService.GetPrice((Species)newIndex)}");
                var breedList = (UIMenuListItem)sender.MenuItems[2];
                breedList.Items = _petService.GetBreedListBySpecies(_currentPetSetup.Species);
                breedList.Index = 0;
                _currentPetSetup.Breed = 0;
                //
                RefreshShowcaseModel();

                return;
            }
            if (listItem.Text == "Breeds")
            {
                // 保存Breed设置
                _currentPetSetup.Breed = newIndex;
                //
                RefreshShowcaseModel();

                return;
            }
        }

        private void OnSpawnMenuItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            // 生成点击
            if (selectedItem.Text == "~g~Spawn")
            {
                UIMenuListItem listItem = (UIMenuListItem)sender.MenuItems.First();
                string selectedPetName = listItem.CurrentItem().ToString();
                // 如果选中的宠物名字不合法
                if (string.IsNullOrWhiteSpace(selectedPetName))
                {
                    return;
                }
                // 循环检查宠物名字是否已经被生成过
                foreach (var item in _spawnedPets)
                {
                    if (selectedPetName == item.Name)
                    {
                        GTA.UI.Screen.ShowSubtitle($@"~b~{selectedPetName}~w~ is already next to you.");
                        return;
                    }
                }
                var safePos = World.GetSafeCoordForPed(_player.Position, false, 16);
                if (safePos == Vector3.Zero)
                {
                    ShowSubtitle("There is no safe place nearby to spawn your pet.");
                    return;
                }
                // 从目录中读取宠物的json内容
                string petJson = File.ReadAllText($@"{Config.PathToPetsFolder}\{selectedPetName}.json");
                // 获取玩家、宠物的Ped类型
                Pet pet = JsonConvert.DeserializeObject<Pet>(petJson);
                Ped entity = World.CreatePed(pet.Model, safePos);
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, entity, 0, 0, pet.Breed, 0);
                pet.Entity = entity;
                _spawnedPets.Add(pet);
                //
                int playerGroup = Function.Call<int>(Hash.GET_PED_GROUP_INDEX, _player);
                Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, entity, playerGroup);
                Function.Call(Hash.SET_PED_CAN_TELEPORT_TO_GROUP_LEADER, entity, playerGroup, true);
                entity.NeverLeavesGroup = true;
                entity.MaxHealth = 1000;
                entity.Health = 1000;
                //
                if (_petService.CanPedAttack(entity))
                {
                    Function.Call(Hash.TASK_COMBAT_HATED_TARGETS_AROUND_PED, entity, 1000, 0);
                }
                // 创建一个小蓝点
                var pedBlip = entity.AddBlip();
                pedBlip.Sprite = (BlipSprite)489;
                pedBlip.Color = BlipColor.Blue;
                pedBlip.Name = pet.Name;
            }

            // 刷新点击
            if (selectedItem.Text == "~y~Refresh")
            {
                UIMenuListItem list = (UIMenuListItem)sender.MenuItems.First();
                list.Items = _fileService.GetAllPetsFromDirectory();
            }
        }

        private void OnPetMenuItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (selectedItem.Text == "Despawn")
            {
                var name = _menuPet.MenuItems.First().RightLabel;
                for (int i = 0; i < _spawnedPets.Count; i++)
                {
                    if (_spawnedPets[i].Name == name)
                    {
                        _spawnedPets[i].Entity.Delete();
                        _spawnedPets.RemoveAt(i);
                    }
                }
            }
        }

        //private void ContactAnswered(iFruitContact contact)
        //{
        //    GTA.UI.Screen.ShowSubtitle("Hello, I am considering owning a pet");
        //    Wait(2000);
        //    Notification.Show(NotificationIcon.Ashley, contact.Name, "Manager", "That's great", false, false);
        //    contact.EndCall();
        //}

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad2)
            {
                _menuPetSpawner.Visible = !_menuPetSpawner.Visible;
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (_isPlayerInShopMenu)
                {
                    if (!_isOnscreenKeyboardEditing)
                    {
                        if (!_pool.IsAnyMenuOpen())
                        {
                            _menuAnimalArk.Visible = true;
                        }
                    }
                }
                return;
            }
            if (e.KeyCode == Keys.Back)
            {
                if (!_isOnscreenKeyboardEditing)
                {
                    if (_isPlayerInShopMenu)
                    {
                        _player.Position = Config.PetShopPos;
                        _menuAnimalArk.Visible = false;
                        _isPlayerInShopMenu = false;
                        _currentShowcaseModel.Delete();
                        Function.Call(Hash.PLAY_PED_AMBIENT_SPEECH_NATIVE, _player, "GENERIC_BYE", "Speech_Params_Force_Shouted_Critical", 1);
                        World.RenderingCamera = null;
                        Hud.IsRadarVisible = true;
                        World.DestroyAllCameras();
                    }
                }
                return;
            }
            if (e.KeyCode == Keys.NumPad3)
            {
                ShowSubtitle(_spawnedPets.Count());
                return;
            }
            if (e.KeyCode == Keys.NumPad9)
            {
                var pet = _spawnedPets[0];
                pet.Entity.Health -= 20;
                _menuPet.Visible = true;
                return;
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            if (_spawnedPets.Count > 0)
            {
                foreach (Pet pet in _spawnedPets)
                {
                    pet.Entity.Delete();
                }
            }
        }

        protected internal void ShowSubtitle<T>(T value, int duration = 2500)
        {
            GTA.UI.Screen.ShowSubtitle(value.ToString(), duration);
        }

        private void RefreshShowcaseModel()
        {
            _currentShowcaseModel.Delete();
            _currentShowcaseModel = World.CreatePed(new Model(_petService.GetModelName(_currentPetSetup.Species)), _petShowcasePos, _heading);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, _currentShowcaseModel, 0, 0, _currentPetSetup.Breed, 0);
        }

        private void LoadIPL(string ipl)
        {
            if (!Function.Call<bool>(Hash.IS_IPL_ACTIVE, ipl))
            {
                Function.Call<int>(Hash.REQUEST_IPL, ipl);
            }
            return;
        }

        private void EnableInterior(int interior)
        {
            if (!Function.Call<bool>(Hash.IS_VALID_INTERIOR, interior))
                return;

            bool isDisabled = Function.Call<bool>(Hash.IS_INTERIOR_DISABLED, interior);
            bool isCapped = Function.Call<bool>(Hash.IS_INTERIOR_CAPPED, interior);

            if (isDisabled || isCapped)
            {
                Function.Call<bool>(Hash.PIN_INTERIOR_IN_MEMORY, interior);
                Function.Call<bool>(Hash.SET_INTERIOR_ACTIVE, interior, true);
                if (isDisabled)
                    Function.Call<bool>(Hash.DISABLE_INTERIOR, interior, false);
                if (isCapped)
                    Function.Call<bool>(Hash.CAP_INTERIOR, interior, false);
            }
            return;
        }

        private bool _isDebugging;
    }
}
