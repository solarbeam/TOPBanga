using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Detection.GameUtil
{
    enum GoalSide { Bottom, Top, Left, Right }
    [Flags]
    enum SidePass {
        Bottom = 1,
        Top = 2,
        Left = 4,
        Right = 8
    }
    class GoalChecker
    {
        static Boolean Check(GoalZone zone,Coordinates ballPos,uint iterations = 10)
        {
            /**
             * The zone can be of any specific shape
             * These Booleans will help us catch the 
             *      extreme cases
             */
            SidePass flags = 0;

            /**
            * Used to define the goal checking accuracy
            */
            float toAdd = 1 / iterations;

            float allowedDiff;

            /**
             * Start by checking the bottom edge
             */
            allowedDiff = (zone.bottomLeft.X + zone.bottomRight.X) / iterations;
            LinearInterpolation(zone,ballPos,GoalSide.Bottom,allowedDiff,toAdd, ref flags);

            /**
             * Check left edge
             */
            allowedDiff = (zone.bottomLeft.Y + zone.topLeft.Y) / iterations;
            LinearInterpolation(zone,ballPos,GoalSide.Left,allowedDiff,toAdd,ref flags);

            /**
             * Check top edge
             */
            allowedDiff = (zone.topLeft.X + zone.topRight.X) / iterations;
            LinearInterpolation(zone,ballPos,GoalSide.Top,allowedDiff,toAdd, ref flags);

            /**
             * Finally, check the right edge
             */
            allowedDiff = (zone.topRight.Y + zone.bottomRight.Y) / iterations;
            LinearInterpolation(zone,ballPos,GoalSide.Right,allowedDiff,toAdd, ref flags);
            
            /**
             * Check if all cases were met
             */
            if ( flags.HasFlag(SidePass.Top) && flags.HasFlag(SidePass.Bottom)
                && flags.HasFlag(SidePass.Left) && flags.HasFlag(SidePass.Right) )
            {
                return true;
            }

            /**
             * Check if it's one of the more extreme cases
             */
            int value = 0;

            if (flags.HasFlag(SidePass.Top)) value++;
            if (flags.HasFlag(SidePass.Bottom)) value++;
            if (flags.HasFlag(SidePass.Left)) value++;
            if (flags.HasFlag(SidePass.Right)) value++;

            if (value >= 2) return true;
            else
                return false;
        }
        private static void LinearInterpolation(GoalZone zone,
                                                    Coordinates ballPos,GoalSide side,
                                                    float allowedDiff, float toAdd,ref SidePass flags)
        {
            if ( side == GoalSide.Bottom || side == GoalSide.Top )
            {
                InterpolationTB(zone,ballPos,side,allowedDiff,toAdd,ref flags);
            }
            else
            {
                InterpolationLF(zone,ballPos,side,allowedDiff,toAdd, ref flags);
            }
        }
        private static void InterpolationLF(GoalZone zone, Coordinates ballPos,
                                                GoalSide side,float allowedDiff,
                                                float toAdd,ref SidePass flags)
        {
            Coordinates toCheck;
            for (float i = 0; i <= 1; i += toAdd)
            {
                if ( side == GoalSide.Left )
                {
                    toCheck = getHalfwayPoint(zone.bottomLeft, zone.topLeft, i);
                }
                else
                    toCheck = getHalfwayPoint(zone.bottomRight, zone.topRight, i);
                
                toCheck = getHalfwayPoint(zone.bottomLeft, zone.bottomRight, i);
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if ( side == GoalSide.Left )
                    {
                        if (ballPos.X > toCheck.X)
                        {
                            flags |= SidePass.Left;
                            return;
                        }
                        else
                            return;
                    }
                    else
                    {
                        if ( ballPos.X < toCheck.X )
                        {
                            flags |= SidePass.Right;
                            return;
                        }
                        else
                            return;
                    }
                }
                else
                    continue;
            }
            return;
        }
        private static void InterpolationTB(GoalZone zone, Coordinates ballPos,
                                                GoalSide side,float allowedDiff,
                                                float toAdd,ref SidePass flags)
        {
            Coordinates toCheck;
            for (float i = 0; i <= 1; i += toAdd)
            {
                if ( side == GoalSide.Top )
                {
                    toCheck = getHalfwayPoint(zone.topLeft, zone.topRight, i);
                }
                else
                    toCheck = getHalfwayPoint(zone.bottomLeft, zone.bottomRight, i);
                
                if (getDiff(ballPos.X, toCheck.X) <= allowedDiff)
                {
                    if ( side == GoalSide.Top )
                    {
                        if ( ballPos.Y < toCheck.Y )
                        {
                            flags |= SidePass.Top;
                            return;
                        }
                        else
                            return;
                    }
                    else
                    {
                        if ( ballPos.Y > toCheck.Y )
                        {
                            flags |= SidePass.Bottom;
                            return;
                        }
                        else
                            return;
                    }
                }
                else
                    continue;
            }
            return;
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
