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

            Timer t = new Timer(DrawWorldCallback, world, 0, 160);
             
            Console.ReadLine(); 
        }
         
        private static void DrawWorldCallback(Object o)
        {
            var world = (World)o;
            world.Process();
            world.DisplayWorld(); 
            GC.Collect();
        }
   
    }
}
