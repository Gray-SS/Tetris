using ConsoleEngine.Inputs;
using System;
using System.Threading.Tasks;

namespace Tetris.Game
{
    public static class SoundMixer
    {
        private static Random _random = new();

        private static bool _paused;

        private static SoundPlayer _currentSound;
        private static SoundPlayer _nextSound;
        private static SoundPlayer[] _sounds = new SoundPlayer[]
        {
            StaticValues.MainBradinski,
            StaticValues.MainKarinka,
            StaticValues.MainLoginska,
            StaticValues.MainTroika,
        };

        public static void Start()
        {
            Task.Run(() =>
            {
                _nextSound = _sounds[_random.Next(_sounds.Length)];

                while (true)
                {
                    do { }
                    while (_paused);

                    var sound = GetRandomSound();
                    sound.Play(false);

                    do { }
                    while (!sound.IsBeingPlayed);

                    do { }
                    while (sound.IsBeingPlayed);
                }
            });
        }

        public static SoundPlayer GetRandomSound()
        {
            _currentSound = _nextSound;

            do { _nextSound = _sounds[_random.Next(_sounds.Length)]; }
            while (_nextSound == _currentSound);

            return _currentSound;
        }

        public static void PlayGameOverSound()
        {
            _paused = true;
            if (_currentSound.IsBeingPlayed)
                _currentSound.StopPlaying();

            StaticValues.MainGameOver.Play(false);

            do { }
            while (!StaticValues.MainGameOver.IsBeingPlayed);

            do { }
            while (StaticValues.MainGameOver.IsBeingPlayed);

            _paused = false;
        }
    }
}
