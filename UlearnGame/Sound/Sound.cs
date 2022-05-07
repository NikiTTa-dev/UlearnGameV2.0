using System.Media;

namespace UlearnGame
{
    public static class Sound
    {
        private static SoundPlayer ClapPlayer;
        private static SoundPlayer StepPlayer;

        static Sound()
        {
            ClapPlayer = new SoundPlayer(@"Resources\Sounds\ClapSound.wav");
            ClapPlayer.Load();
            StepPlayer = new SoundPlayer(@"Resources\Sounds\StepSound.wav");
            StepPlayer.Load();
        }

        public static void Play(Sounds sound)
        {
            switch (sound)
            {
                case Sounds.ClapSound: ClapPlayer.Play(); break;
                case Sounds.StepSound: StepPlayer.Play(); break;
            }
        }
    }
}