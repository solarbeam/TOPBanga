using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    enum Side { bottom , top , left , right }
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
            bottom = LinearInterpolation(zone,ballPos,Side.bottom,allowedDiff,toAdd);

            /**
             * Check left edge
             */
            allowedDiff = (zone.bottomLeft.Y + zone.topLeft.Y) / iterations;
            left = LinearInterpolation(zone,ballPos,Side.left,allowedDiff,toAdd);

            /**
             * Check top edge
             */
            allowedDiff = (zone.topLeft.X + zone.topRight.X) / iterations;
            top = LinearInterpolation(zone,ballPos,Side.top,allowedDiff,toAdd);

            /**
             * Finally, check the right edge
             */
            allowedDiff = (zone.topRight.Y + zone.bottomRight.Y) / iterations;
            right = LinearInterpolation(zone,ballPos,Side.right,allowedDiff,toAdd);
            
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
        private static Boolean LinearInterpolation(GoalZone zone,
                                                    Coordinates ballPos,Side side,
                                                    float allowedDiff, float toAdd)
        {
            if ( side == Side.bottom || side == Side.top )
            {
                return InterpolationTB(zone,ballPos,side,allowedDiff,toAdd);
            }
            else
            {
                return InterpolationLF(zone,ballPos,side,allowedDiff,toAdd);
            }
        }
        private static Boolean InterpolationLF(GoalZone zone, Coordinates ballPos,
                                                Side side,float allowedDiff,
                                                float toAdd)
        {
            Coordinates toCheck;
            for (float i = 0; i <= 1; i += toAdd)
            {
                if ( side == Side.left )
                {
                    toCheck = getHalfwayPoint(zone.bottomLeft, zone.topLeft, i);
                }
                else
                    toCheck = getHalfwayPoint(zone.bottomRight, zone.topRight, i);
                
                toCheck = getHalfwayPoint(zone.bottomLeft, zone.bottomRight, i);
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if ( side == Side.left )
                    {
                        if ( ballPos.X > toCheck.X )
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        if ( ballPos.X < toCheck.X )
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                }
                else
                    continue;
            }
            return false;
        }
        private static Boolean InterpolationTB(GoalZone zone, Coordinates ballPos,
                                                Side side,float allowedDiff,
                                                float toAdd)
        {
            Coordinates toCheck;
            for (float i = 0; i <= 1; i += toAdd)
            {
                if ( side == Side.top )
                {
                    toCheck = getHalfwayPoint(zone.topLeft, zone.topRight, i);
                }
                else
                    toCheck = getHalfwayPoint(zone.bottomLeft, zone.bottomRight, i);
                
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if ( side == Side.top )
                    {
                        if ( ballPos.Y < toCheck.Y )
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        if ( ballPos.Y > toCheck.Y )
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                }
                else
                    continue;
            }
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
