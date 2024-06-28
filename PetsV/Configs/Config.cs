using GTA.Math;

namespace PetsV.Configs
{
    class Config
    {
        public static string ModID { get; } = "PetsV";
        public static string Version { get; } = "v0.2-alpha";
        public static string PathToModFolder { get; } = $@"scripts\{ModID}";
        public static string PathToPetsFolder { get; } = $@"{PathToModFolder}\Pets";
        public static Vector3 PetShopPos { get; } = new Vector3(562.259f, 2741.326f, 42.569f);
        public static Vector3 ShowcaseCameraPos { get; } = new Vector3(552, 2811, 43.5f);
        public static Vector3 ShowcaseCameraRot { get; } = new Vector3(-30, 0, 180);
        public static float ShowcaseCameraFov { get; } = 40;
    }
}
