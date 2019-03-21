using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BouncingSharp
{
    internal class World
    {
        bool IsLocked = false; 
        public IMovement PropertyX { get; set; }
        public IMovement PropertyY { get; set; }

        public readonly int ScreenHeight;
        public readonly int ScreenWidth;
        List<WorldObject> Content { get; set; }
        StringBuilder Output { get; set; }

        public World(int screenHeight, int screenWidth)
        {
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;

            PropertyX = new Movement()
            {
                Origin = 35,
                Target = 36,
                Direction = true,
                Maximum = screenHeight - 1
            };
            PropertyY = new Movement()
            {
                Origin = 15,
                Target = 14,
                Direction = false,
                Maximum = screenWidth - 1
            };

            Content = new List<WorldObject>();
            Output = new StringBuilder();

            Content.Add(new WorldObject { X = 15, Y = 15, Value = "#" });
        }

        public void DisplayWorld()
        {
            Console.Clear();
            Output = new StringBuilder();

            for (var x = 0; x < ScreenHeight; x++)
            {
                var line = "";
                for (var y = 0; y < ScreenWidth; y++)
                {
                    if (Content != null && Content.Exists(o => o.X == x && o.Y == y))
                    {
                        line += "#";
                    }
                    else
                    {
                        line += " ";
                    }
                }
                Output.Append(line);
                Output.AppendLine();
            }

            Output.AppendLine();
            Output.AppendLine($"Target X    : {PropertyX.Target.ToString("00")}    |  Target Y    : {PropertyY.Target.ToString("00")}");
            Output.AppendLine($"Current X   : {Content.First().X.ToString("00")}   |  Current Y   : {Content.First().Y.ToString("00")}");
            Output.AppendLine($"Direction-X : {PropertyX.Direction} |  Direction-Y : {PropertyY.Direction}");
            Console.Write(Output);

        }

        WorldObject NextMove(WorldObject obj)
        {
            Random r = new Random();
            var tempObjext = new WorldObject()
            {
                X = obj.X,
                Y = obj.Y
            };

            tempObjext.X = GetNextPosition(PropertyX, obj.X);
            tempObjext.Y = GetNextPosition(PropertyY, obj.Y);

            PropertyX = SetNewProperty(PropertyX, tempObjext.X);
            PropertyY = SetNewProperty(PropertyY, tempObjext.Y);

            return tempObjext;
        }

        int GetNextPosition(IMovement movement, int currentPosition)
        {
            int nextPosition = movement.Direction ? currentPosition + 1 : currentPosition - 1;
            nextPosition = nextPosition > movement.Maximum || (!movement.Direction && nextPosition < movement.Target) || (movement.Direction && nextPosition > movement.Target) ? currentPosition : nextPosition;
            nextPosition = nextPosition < 0 ? 0 : nextPosition;
            nextPosition = nextPosition > movement.Maximum ? movement.Maximum : nextPosition;
            return nextPosition;
        }
        IMovement SetNewProperty(IMovement m, int nextValue)
        {
            var @new = new Movement()
            {
                Direction = m.Direction,
                Target = m.Target,
                Maximum = m.Maximum,
                Origin = m.Origin
            };

            if (nextValue == 0)
            {
                @new.Target = 1;
                @new.Direction = true;
            }
            if (m.Direction && nextValue > m.Target)
            {
                @new.Direction = false;
            }
            if (m.Direction && nextValue < m.Target)
            {
                @new.Direction = true;
            }

            if (!m.Direction && nextValue == m.Target && m.Target > 0)
            {
                @new.Target = nextValue - 1;
            }
            else if (m.Direction && nextValue == m.Target && m.Target < m.Maximum)
            {
                @new.Target = nextValue + 1;
            }
            else if (nextValue > 0)
            {
                @new.Target = m.Direction ? nextValue + 1 < m.Maximum ? nextValue + 1 : nextValue : nextValue - 1;
                @new.Direction = !@new.Direction;
            }

            return @new;
        }

        public void Process()
        {
            if (IsLocked) return;
            IsLocked = true;

            Random r = new Random();
            Content.Where(obj => !string.IsNullOrEmpty(obj.Value)).ToList().ForEach(
            w =>
                {

                    WorldObject temp = NextMove(w);

                    Content.Clear();
                    Content.Add(new WorldObject { X = temp.X, Y = temp.Y, Value = "#" });

                }
             );

            IsLocked = false;
        }

    }

    public class WorldObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Value { get; set; }
    }

    interface IMovement
    {
        bool Direction { get; set; }
        int Target { get; set; }
        int Origin { get; set; }
        int Maximum { get; set; }
    }

    public class Movement : IMovement
    {
        public bool Direction { get; set; }
        public int Target { get; set; }
        public int Origin { get; set; }
        public int Maximum { get; set; }
    }

}
