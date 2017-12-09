using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using static FoosLiveAndroid.Util.GameControl.Enums;

namespace FoosLiveAndroid.Util.GameControl
{
    class RowChecker
    {
        /// <summary>
        /// Defines the rows of foosmen
        /// </summary>
        public RectF[] rows;

        /// <summary>
        /// Defines the row the ball is currently in
        /// </summary>
        public Row currentRow;

        private int _RedGoalieZone;
        private int _RedDefenceZone;
        private int _BlueAttackZone;
        private int _RedMidfieldZone;
        private int _BlueMidfieldZone;
        private int _RedAttackZone;
        private int _BlueDefenceZone;
        private int _BlueGoalieZone;

        public void CheckRow(PointF lastBallCoordinates, ref CurrentEvent currentEvent)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                {
                    switch (i)
                    {
                        case 0:
                            currentRow = Row.RedGoalie;
                            _RedGoalieZone++;
                            break;
                        case 1:
                            currentRow = Row.RedDefence;
                            _RedDefenceZone++;
                            break;
                        case 2:
                            currentRow = Row.BlueAttack;
                            _BlueAttackZone++;
                            break;
                        case 3:
                            currentRow = Row.RedMidfield;
                            _RedMidfieldZone++;
                            break;
                        case 4:
                            currentRow = Row.BlueMidfield;
                            _BlueMidfieldZone++;
                            break;
                        case 5:
                            currentRow = Row.RedAttack;
                            _RedAttackZone++;
                            break;
                        case 6:
                            currentRow = Row.BlueDefence;
                            _BlueDefenceZone++;
                            break;
                        case 7:
                            currentRow = Row.BlueGoalie;
                            _BlueGoalieZone++;
                            break;
                    }

                    if (currentEvent == CurrentEvent.None)
                        currentEvent = CurrentEvent.PositionChanged;

                    break;
                }
            }
        }

        public void CalculateRows(System.Drawing.Rectangle tableZone, CaptureMode mode)
        {
            // Declare constants
            float[] multipliers = new float[8];

            if (mode == CaptureMode.Video)
            {
                float toAssign = PropertiesManager.GetFloatProperty("percentageVideo");
                for (int i = 0; i < multipliers.Length; i ++)
                    multipliers[i] = toAssign;
            }
            else
            {
                multipliers[0] = PropertiesManager.GetFloatProperty("rowOne");
                multipliers[1] = PropertiesManager.GetFloatProperty("rowTwo");
                multipliers[2] = PropertiesManager.GetFloatProperty("rowThree");
                multipliers[3] = PropertiesManager.GetFloatProperty("rowFour");
                multipliers[4] = PropertiesManager.GetFloatProperty("rowFive");
                multipliers[5] = PropertiesManager.GetFloatProperty("rowSix");
                multipliers[6] = PropertiesManager.GetFloatProperty("rowSeven");
                multipliers[7] = PropertiesManager.GetFloatProperty("rowEight");
            }
            
            rows = new RectF[8];

            // Assign the first row values, because we'll use it in the for cycle
            rows[0] = new RectF(tableZone.Left, tableZone.Top,
                                tableZone.Right, tableZone.Top + (tableZone.Height * multipliers[0]));

            // Assign the rows values
            for (int i = 1; i < rows.Length; i++)
            {
                // We calculate Y values based on the previous rows
                float toAdd = 0;

                // Here, we add to toAdd the Y values of the previous rows
                for (int j = i; j != 0; j--)
                {
                    toAdd += rows[j - 1].Height();
                }

                // Finally, we assign the new row values
                rows[i] = new RectF(tableZone.Left, rows[i - 1].Bottom,
                                    tableZone.Right, tableZone.Top + toAdd + (tableZone.Height * multipliers[i]));
            }
        }
    }
}