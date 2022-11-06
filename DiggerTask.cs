using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Digger
{
    public class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    public class Player : ICreature
    {
        private bool CanMove(int x, int y)
        {
            return Game.Map[x, y] == null || !(Game.Map[x, y] is Sack);
        }

        public CreatureCommand Act(int x, int y)
        {
            var command = new CreatureCommand ();
            if (Game.KeyPressed == Keys.Up && y > 0 && CanMove(x, y - 1)) command.DeltaY = -1;
            if (Game.KeyPressed == Keys.Right && x < Game.MapWidth - 1 && CanMove(x + 1, y)) command.DeltaX = 1;
            if (Game.KeyPressed == Keys.Down && y < Game.MapHeight - 1 && CanMove(x, y + 1)) command.DeltaY = 1;
            if (Game.KeyPressed == Keys.Left && x > 0 && CanMove(x - 1, y)) command.DeltaX = -1;
            return command;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Gold)
                Game.Scores += 10;
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }
    }

    public class Sack : ICreature
    {
        private int fall = 0;
        public CreatureCommand Act(int x, int y)
        {
            if (y != Game.MapHeight - 1 && 
                (Game.Map[x, y + 1] == null || ((Game.Map[x, y + 1] is Player || Game.Map[x, y + 1] is Monster) && fall > 0)))
            {
                fall++;
                return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
            }

            else if (fall > 1)
                return new CreatureCommand { DeltaX = 0, DeltaY = 0, TransformTo = new Gold() };

            fall = 0;
            return new CreatureCommand { };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return false;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    public class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand { };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }

    public class Monster : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            var xMove = 0;
            var yMove = 0;
            var xDigger = IsDiggerInMap(x, y).x;
            var yDigger = IsDiggerInMap(x, y).y;
            if (xDigger < x) xMove = -1;
            else if (xDigger > x) xMove = 1;
            else if (yDigger < y) yMove = -1;
            else if (yDigger > y) yMove = 1;

            if (!(x + xMove >= 0 && x + xMove < Game.MapWidth && y + yMove >= 0 && y + yMove < Game.MapHeight))
                return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
            if (Game.Map[x + xMove, y + yMove] != null &&
                (Game.Map[x + xMove, y + yMove] is Terrain
                || Game.Map[x + xMove, y + yMove] is Sack
                || Game.Map[x + xMove, y + yMove] is Monster))
                return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
            return new CreatureCommand() { DeltaX = xMove, DeltaY = yMove };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Monster.png";
        }

        static private (int x, int y) IsDiggerInMap(int x, int y)
        {
            for (int i = 0; i < Game.MapWidth; i++)
            {
                for (int j = 0; j < Game.MapHeight; j++)
                {
                    if (Game.Map[i, j] != null && Game.Map[i, j] is Player)
                    {
                        return (i, j);
                    }
                }
            }
            return (x, y);
        }
    }
}
