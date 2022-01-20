using ConsoleEngine;
using ConsoleEngine.Graphics;

namespace Tetris.Game
{
    public abstract class GameState
    {
        public GraphicsDevice Graphics => _engine.Graphics;
        protected int ConsoleWidth => _engine.ConsoleWidth;
        protected int ConsoleHeight => _engine.ConsoleHeight;

        private Engine _engine;

        public GameState(Engine engine)
        {
            _engine = engine;
        }

        public virtual void OnStateCalled() { }
        public virtual void Initialize() { }
        public virtual void Load() { }
        public virtual void Update(float dt) { }
        public virtual void Draw() { }
    }
}
