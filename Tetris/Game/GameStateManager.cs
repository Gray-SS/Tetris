using System.Collections.Generic;

namespace Tetris.Game
{
    public class GameStateManager
    {
        private GameState _currentState;
        private readonly List<GameState> _states = new();
        private static readonly GameStateManager _instance = new();

        private GameStateManager() { }

        public static void AppendState<TState>(TState state) where TState : GameState
        {
            _instance._states.Add(state);
        }

        public static void SetCurrentState<TState>()
        {
            _instance._currentState = _instance._states.Find(x => x.GetType() == typeof(TState));
            _instance._currentState.OnStateCalled();
        }

        public static void Load()
        {
            foreach(var instance in _instance._states)
            {
                instance.Load();
            }
        }

        public static void Update(float dt)
        {
            _instance._currentState.Update(dt);
        }

        public static void Draw()
        {
            _instance._currentState.Draw();
        }
    }
}
