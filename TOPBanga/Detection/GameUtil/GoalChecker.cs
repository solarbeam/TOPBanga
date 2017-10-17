using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    class GoalChecker
    {
        static Boolean Check(GoalZone zone,Coordinates ballPos,uint iterations = 10)
        {
            /**
             * The zone can be of any specific shape
             * These Booleans will help us catch the 
             *      extreme cases
             */
            Boolean left = false;
            Boolean right = false;
            Boolean bottom = false;
            Boolean top = false;

            /**
            * Used to define the goal checking accuracy
            */
            float toAdd = 1 / iterations;

            float allowedDiff;

            Coordinates toCheck;

            /**
             * Start by checking the bottom edge
             */
            allowedDiff = (zone.bottomLeft.X + zone.bottomRight.X) / iterations;
            for (float i = 0; i <= 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.bottomLeft, zone.bottomRight, i);
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if (ballPos.Y > toCheck.Y)
                    {
                        bottom = true;
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
            for (float i = 0; i <= 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.bottomLeft, zone.topLeft, i);
                if (getDiff(ballPos.Y, toCheck.Y) <= allowedDiff)
                {
                    if (ballPos.X > toCheck.X)
                    {
                        left = true;
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
            for (float i = 0; i <= 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.topLeft, zone.topRight, i);
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if (ballPos.Y < toCheck.Y)
                    {
                        top = true;
                        break;
                    }
                    else
                        return false;
                }
                else
                    continue;
            }
            /**
             * Finally, check the right edge
             */
            allowedDiff = (zone.topRight.Y + zone.bottomRight.Y) / iterations;
            for (float i = 0; i <= 1; i += toAdd)
            {
                toCheck = getHalfwayPoint(zone.topRight, zone.bottomRight, i);
                if (getDiff(ballPos.Y, toCheck.Y) <= allowedDiff)
                {
                    if (ballPos.X > toCheck.X)
                    {
                        right = true;
                        break;
                    }
                    else
                        return false;
                }
                else
                    continue;
            }
            
            /**
             * Check if all cases were met
             */
            if ( top && bottom && left && right )
            {
                return true;
            }

            /**
             * Check if it's one of the more extreme cases
             */
            int value = 0;

            if (bottom) value++;
            if (left) value++;
            if (top) value++;
            if (right) value++;

            if (value >= 2) return true;
            else
                return false;
        }
        private static Coordinates getHalfwayPoint(Coordinates one, Coordinates two, float coefficient)
        {
            float x, y;

            x = (1 - coefficient) * one.X + coefficient * two.X;
            y = (1 - coefficient) * one.Y + coefficient * two.Y;

            return new Coordinates(x, y);
        }
        private static float getDiff(float one, float two)
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
