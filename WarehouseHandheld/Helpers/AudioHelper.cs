using System;
using Plugin.SimpleAudioPlayer;

namespace WarehouseHandheld.Helpers
{
    public class AudioHelper
    {

        static ISimpleAudioPlayer player;
        public static void PlayBeep()
        {
            InitializePlayer();
            player.Play();
        }

        public static void InitializePlayer()
        {
            if (player == null)
            {
                player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                player.Load("beep.mp3");
            }
            
        }


    }
}
