using System.Collections.Generic;

namespace Dewritwo
{
    internal class Dictionaries
    {
        public static Dictionary<string, string> GetArmor()
        {
            var Armor = new Dictionary<string, string>();
            Armor.Add("Air Assault", "air_assault");
            Armor.Add("Stealth", "stealth");
            Armor.Add("Renegade", "renegade");
            Armor.Add("Nihard", "nihard");
            Armor.Add("Gladiator", "gladiator");
            Armor.Add("Mac", "mac");
            Armor.Add("Shark", "shark");
            Armor.Add("Juggernaut", "juggernaut");
            Armor.Add("Dutch", "dutch");
            Armor.Add("Chameleon", "chameleon");
            Armor.Add("Halberd", "halberd");
            Armor.Add("Cyclops", "cyclops");
            Armor.Add("Scanner", "scanner");
            Armor.Add("Mercenary", "mercenary");
            Armor.Add("Hoplite", "hoplite");
            Armor.Add("Ballista", "ballista");
            Armor.Add("Strider", "strider");
            Armor.Add("Demo", "demo");
            Armor.Add("Orbital", "orbital");
            Armor.Add("Spectrum", "spectrum");
            Armor.Add("Gungnir", "gungnir");
            Armor.Add("Hammerhead", "hammerhead");
            Armor.Add("Omni", "omni");
            Armor.Add("Oracle", "oracle");
            Armor.Add("Silverback", "silverback");
            Armor.Add("Widow Maker", "widow_maker");
            return Armor;
        }

        public static Dictionary<string, string> GetColor()
        {
            var Colors = new Dictionary<string, string>();
            Colors.Add("Blue", "blue");
            Colors.Add("Red", "red");
            Colors.Add("Green", "green");
            Colors.Add("Purple", "purple");
            Colors.Add("Orange", "orange");
            Colors.Add("Lime", "lime");
            Colors.Add("Emerald", "emerald");
            Colors.Add("Teal", "teal");
            Colors.Add("Cyan", "cyan");
            Colors.Add("Cobalt", "cobalt");
            Colors.Add("Indigo", "indigo");
            Colors.Add("Violet", "violet");
            Colors.Add("Pink", "pink");
            Colors.Add("Magenta", "magenta");
            Colors.Add("Crimson", "crimson");
            Colors.Add("Amber", "amber");
            Colors.Add("Yellow", "yellow");
            Colors.Add("Brown", "brown");
            Colors.Add("Olive", "olive");
            Colors.Add("Steel", "steel");
            Colors.Add("Mauve", "mauve");
            Colors.Add("Taupe", "taupe");
            Colors.Add("Sienna", "sienna");
            return Colors;
        }
        public static Dictionary<string, string> GetAction()
        {
            var Actions = new Dictionary<string, string>();
            Actions.Add("Bind", "bind");
            Actions.Add("Commands", "command");
            return Actions;
        }

        public static Dictionary<string, string> GetCommand()
        {
            var Commands = new Dictionary<string, string>();
            Commands.Add("Forge (Delete)", "Game.DeleteForgeItem");
            Commands.Add("Push to Talk Key", "+VoIP.Talk");
            Commands.Add("Hide HUD", "Camera.HideHUD");
            Commands.Add("Camera Mode", "Camera.Mode");
            Commands.Add("Camera Speed", "Camera.Speed");
            Commands.Add("Force Load Map", "Game.ForceLoad");
            Commands.Add("Load GameType", "Game.GameType");
            Commands.Add("Game Info", "Game.Info");
            Commands.Add("Language", "Game.LanguageID");
            Commands.Add("Filter Debug Log", "Game.LogFilter");
            Commands.Add("Debug Log Mode", "Game.LogMode");
            Commands.Add("Debug Log File Name", "Game.LogMode");
            Commands.Add("Load Forge/Map", "Game.Map");
            Commands.Add("HTML Menu URL", "Game.MenuURL");
            Commands.Add("HTML Menu Open", "Game.SetMenuEnabled");
            Commands.Add("Open Specific UI", "Game.ShowUI");
            Commands.Add("Start/Restart Game", "Game.Start");
            Commands.Add("Show Game Version", "Game.Version");
            Commands.Add("Bloom Value", "Graphics.Bloom");
            Commands.Add("Red Hue Value", "Graphics.RedHue");
            Commands.Add("Green Hue Value", "Graphics.GreenHue");
            Commands.Add("Blue Hue Value", "Graphics.BlueHue");
            Commands.Add("Depth of Field Value", "Graphics.DepthOfField");
            Commands.Add("Cinematic Letterbox", "Graphics.Letterbox");
            Commands.Add("Saturation Value", "Graphics.Saturation");
            Commands.Add("Global Chat Channel", "IRC.GlobalChannel");
            Commands.Add("Global Chat Server", "IRC.Server");
            Commands.Add("Global Chat Port", "IRC.ServerPort");
            Commands.Add("Print Player UID", "Player.PrintUID");
            Commands.Add("Print Private Stats Key", "Player.PrivKey");
            Commands.Add("Print Public Stats Key", "Player.PubKey");
            Commands.Add("Announce Server", "Server.Announce");
            Commands.Add("Announce Stats", "Server.AnnounceStats");
            Commands.Add("Connect to a Server", "Server.Connect");
            Commands.Add("Kick Player (Host Only)", "Server.KickPlayer");
            Commands.Add("List Players (Host Only)", "Server.ListPlayers");
            Commands.Add("HTTP Server Port", "Server.Port");
            Commands.Add("Automatically Announce", "Server.ShouldAnnounce");
            Commands.Add("Game Speed", "Time.GameSpeed");
            return Commands;
        }

        public static Dictionary<string, string> GetCommandList()
        {
            var Commands = new Dictionary<string, string>();
            Commands.Add("Hide HUD", "Camera.HideHUD");
            Commands.Add("Camera Mode", "Camera.Mode");
            Commands.Add("Camera Speed", "Camera.Speed");
            Commands.Add("Force Load Map", "Game.ForceLoad");
            Commands.Add("Load GameType", "Game.GameType");
            Commands.Add("Game Info", "Game.Info");
            Commands.Add("Language", "Game.LanguageID");
            Commands.Add("Filter Debug Log", "Game.LogFilter");
            Commands.Add("Debug Log Mode", "Game.LogMode");
            Commands.Add("Debug Log File Name", "Game.LogMode");
            Commands.Add("Load Forge/Map", "Game.Map");
            Commands.Add("HTML Menu URL", "Game.MenuURL");
            Commands.Add("HTML Menu Open", "Game.SetMenuEnabled");
            Commands.Add("Open Specific UI", "Game.ShowUI");
            Commands.Add("Start/Restart Game", "Game.Start");
            Commands.Add("Show Game Version", "Game.Version");
            Commands.Add("Bloom Value", "Graphics.Bloom");
            Commands.Add("Red Hue Value", "Graphics.RedHue");
            Commands.Add("Green Hue Value", "Graphics.GreenHue");
            Commands.Add("Blue Hue Value", "Graphics.BlueHue");
            Commands.Add("Depth of Field Value", "Graphics.DepthOfField");
            Commands.Add("Cinematic Letterbox", "Graphics.Letterbox");
            Commands.Add("Saturation Value", "Graphics.Saturation");
            Commands.Add("Global Chat Channel", "IRC.GlobalChannel");
            Commands.Add("Global Chat Server", "IRC.Server");
            Commands.Add("Global Chat Port", "IRC.ServerPort");
            Commands.Add("Print Player UID", "Player.PrintUID");
            Commands.Add("Print Private Stats Key", "Player.PrivKey");
            Commands.Add("Print Public Stats Key", "Player.PubKey");
            Commands.Add("Announce Server", "Server.Announce");
            Commands.Add("Announce Stats", "Server.AnnounceStats");
            Commands.Add("Connect to a Server", "Server.Connect");
            Commands.Add("Kick Player (Host Only)", "Server.KickPlayer");
            Commands.Add("List Players (Host Only)", "Server.ListPlayers");
            Commands.Add("HTTP Server Port", "Server.Port");
            Commands.Add("Automatically Announce", "Server.ShouldAnnounce");
            Commands.Add("Game Speed", "Time.GameSpeed");
            return Commands;
        }

        public static Dictionary<string, string> GetTheme()
        {
            var Themes = new Dictionary<string, string>();
            Themes.Add("Dark", "BaseDark");
            Themes.Add("Light", "BaseLight");
            return Themes;
        }
    }
}