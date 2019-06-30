using EnsoulSharp;

namespace FrOnDaL_AIO.Common.Utilities.SpellBlocking
{
    internal class Utils
    {
        internal static int GameTimeTickCount
        {
            get { return (int)(Game.Time * 1000); }
        }
    }
}
