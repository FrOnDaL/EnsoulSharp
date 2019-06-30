using EnsoulSharp;
using EnsoulSharp.SDK;
using System;
using FrOnDaL_AIO.Champions;

namespace FrOnDaL_AIO
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnLoadingComplete;
        }
        private static void OnLoadingComplete()
        {
            try
            {
                switch (GameObjects.Player.CharacterName)
                {
                    case "Ashe":
                        Ashe.GameOn();
                        Chat.PrintChat("FrOnDaL AIO " + ObjectManager.Player.CharacterName + " Loaded <font color='#1dff00'>by FrOnDaL</font>");
                        Console.WriteLine("FrOnDaL AIO " + ObjectManager.Player.CharacterName + " loaded");
                        break;
                    default:
                        Chat.PrintChat("<font color='#b756c5' size='25'>FrOnDaL AIO Does Not Support " + ObjectManager.Player.CharacterName + " - By FrOnDaL</font>");
                        Console.WriteLine("FrOnDaL AIO Does Not Support " + ObjectManager.Player.CharacterName + " - By FrOnDaL");
                        break;
                }
            }
            catch (Exception)
            {
                Chat.PrintChat("Error in loading - By FrOnDaL");
                Console.WriteLine("Error in loading - By FrOnDaL");
            }
        }
    }
}
