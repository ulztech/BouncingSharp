using System;
using System.Threading; 

namespace BouncingSharp
{
    partial class Program
    { 
        static void Main(string[] args)
        { 
            Console.Clear();

            var world = new World(25, 75); 
            Timer t = new Timer(DrawWorldCallback, world, 0, 50);
            Timer t2 = new Timer(ThinkCallback, world, 0, 50);
              
            Console.ReadLine(); 
        }
         
        private static void DrawWorldCallback(Object o)
        {
            var world = (World)o;
            world.DisplayWorld();
 
            GC.Collect();
        }
 
        private static void ThinkCallback(Object o)
        {
            var world = (World)o;
            world.Process();  
            GC.Collect();
        }


    }
}
