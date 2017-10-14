using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    class GoalChecker
    {
        /**
         * Used to define the goal checking accuracy
         */
        const uint iterations = 10;
        const double toAdd = 1 / iterations;
        static Boolean Check(GoalZone zone,Coordinates ballPos)
        {
            double allowedDiff;
            Coordinates toCheck;
            /**
             * Start by checking the bottom edge
             */
            allowedDiff = (zone.bottomLeft.X + zone.bottomRight.X) / iterations;
            for (double i = 0; i < 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.bottomLeft, zone.bottomRight, i);
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if (ballPos.Y > toCheck.Y)
                    {
                        break;
                    }
                    else
                        return false;
                }
                else
                    continue;
            }
            /**
             * Check left edge
             */
            allowedDiff = (zone.bottomLeft.Y + zone.topLeft.Y) / iterations;
            for (double i = 0; i < 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.bottomLeft, zone.topLeft, i);
                if (getDiff(ballPos.Y, toCheck.Y) <= allowedDiff)
                {
                    if (ballPos.X > toCheck.X)
                    {
                        break;
                    }
                    else
                        return false;
                }
                else
                    continue;
            }
            /**
             * Check top edge
             */
            allowedDiff = (zone.topLeft.X + zone.topRight.X) / iterations;
            for (double i = 0; i < 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.topLeft, zone.topRight, i);
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if (ballPos.Y < toCheck.Y)
                    {
                        break;
                    }
                    else
                        return false;
                }
                else
                    continue;
            }
            /**
             * Finally, check the left edge
             */
            allowedDiff = (zone.topRight.Y + zone.bottomRight.Y) / iterations;
            for (double i = 0; i < 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.topRight, zone.bottomRight, i);
                if (getDiff(ballPos.Y, toCheck.Y) <= allowedDiff)
                {
                    if (ballPos.X > toCheck.X)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    continue;
            }
            return false;
        }
        private static Coordinates getHalfwayPoint(Coordinates one, Coordinates two, double coefficient)
        {
            double x, y;

            x = (1 - coefficient) * one.X + coefficient * two.X;
            y = (1 - coefficient) * one.Y + coefficient * two.Y;

            return new Coordinates(x, y);
        }
        private static double getDiff(double one, double two)
        {
            if (one > two)
            {
                return one - two;
            }
            else
                return two - one;
        }
    }
}
