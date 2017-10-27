using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOPBanga;
using TOPBanga.Detection;
using TOPBanga.Detection.GameUtil;

namespace Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GoalZone zone = new GoalZone(new Coordinates(0, 0), new Coordinates(100, 0),
                                         new Coordinates(0, 100), new Coordinates(100, 100), goalSide.Left);
            Coordinates ballPos = new Coordinates(60, 50);

            bool test = GoalChecker.Check(zone, ballPos);

            System.Console.WriteLine(test);

            Application.Run(new VideoFromFile(new ColorDetector()));
        }
    }
}
