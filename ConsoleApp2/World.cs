using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingSharp
{
    internal class World
    {
        static object lockMe = new object();
         
        public IMovement PropertyX { get; set; }
        public IMovement PropertyY { get; set; }

        public readonly int ScreenHeight;
        public readonly int ScreenWidth;
        List<WorldObject> Content { get; set; }
        List<WorldObject> LatestContent { get; set; }
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

            if (LatestContent == null)
            {
                LatestContent = Content.ToList();
            }

            var latest = LatestContent.ToList();

            var output = new StringBuilder();

            for (var x = 0; x < ScreenHeight; x++)
            {
                var line = "";
                for (var y = 0; y < ScreenWidth; y++)
                { 
                    foreach(var item in latest)
                    {
                        if (item != null && item.X == x && item.Y == y)
                        {
                            line += "#";
                        }
                        else
                        {
                            line += " ";
                        }
                    } 
                }
                output.Append(line);
                output.AppendLine();
            }


            //output.AppendLine();
            //output.AppendLine($"Target X    : {PropertyX.Target.ToString("00")}    |  Target Y    : {PropertyY.Target.ToString("00")}");
            //output.AppendLine($"Current X   : {Content.First().X.ToString("00")}   |  Current Y   : {Content.First().Y.ToString("00")}");
            //output.AppendLine($"Direction-X : {PropertyX.Direction} |  Direction-Y : {PropertyY.Direction}");
            Console.Write(output);

            LatestContent = Content.ToList();
        }

        WorldObject NextMove(WorldObject obj)
        {
            var tempObjext = new WorldObject()
            {
                X = obj.X,
                Y = obj.Y
            };
             
            tempObjext.X = GetNextPosition(PropertyX, obj.X).GetAwaiter().GetResult();
            tempObjext.Y = GetNextPosition(PropertyY, obj.Y).GetAwaiter().GetResult();

            PropertyX = SetNewProperty(PropertyX, tempObjext.X);
            PropertyY = SetNewProperty(PropertyY, tempObjext.Y);
            
            return tempObjext;
        }

        async Task<int> GetNextPosition(IMovement movement, int currentPosition)
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
            Content.Where(obj => !string.IsNullOrEmpty(obj.Value)).ToList().ForEach(
        w =>
            {
                WorldObject temp = NextMove(w);
                Content.Clear();
                Content.Add(new WorldObject { X = temp.X, Y = temp.Y, Value = "#" });
            }
         );
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
