using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
    public abstract class Game
    {
        public GamePageModel GamePageModel { get; protected set; }
        public abstract void PlayGame();
        public abstract void ExitGame();
        public abstract void CancelGame();
    }

}
