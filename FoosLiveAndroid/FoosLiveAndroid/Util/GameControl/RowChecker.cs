using System.Drawing;
using Android.Graphics;
using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Util.Model;
using PointF = Android.Graphics.PointF;

namespace FoosLiveAndroid.Util.GameControl
{
    class RowChecker : IRowChecker
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

        public void CheckRow(PointF lastBallCoordinates)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                {
                    switch (i)
                    {
                        case 0:
                            currentRow = Row.Team2Goalie;
                            _RedGoalieZone++;
                            break;
                        case 1:
                            currentRow = Row.Team2Defence;
                            _RedDefenceZone++;
                            break;
                        case 2:
                            currentRow = Row.Team1Attack;
                            _BlueAttackZone++;
                            break;
                        case 3:
                            currentRow = Row.Team2MidField;
                            _RedMidfieldZone++;
                            break;
                        case 4:
                            currentRow = Row.Team1MidField;
                            _BlueMidfieldZone++;
                            break;
                        case 5:
                            currentRow = Row.Team2Attack;
                            _RedAttackZone++;
                            break;
                        case 6:
                            currentRow = Row.Team1Defence;
                            _BlueDefenceZone++;
                            break;
                        case 7:
                            currentRow = Row.Team1Goalie;
                            _BlueGoalieZone++;
                            break;
                    }

                    break;
                }
            }
        }

        public void CalculateRows(Rectangle tableZone, ECaptureMode mode)
        {
            // Declare constants
            float[] multipliers = new float[8];

            if (mode == ECaptureMode.Recording)
            {
                float toAssign = PropertiesManager.GetFloatProperty("percentage_video");
                for (int i = 0; i < multipliers.Length; i ++)
                    multipliers[i] = toAssign;
            }
            else
            {
                multipliers[0] = PropertiesManager.GetFloatProperty("row_one");
                multipliers[1] = PropertiesManager.GetFloatProperty("row_two");
                multipliers[2] = PropertiesManager.GetFloatProperty("row_three");
                multipliers[3] = PropertiesManager.GetFloatProperty("row_four");
                multipliers[4] = PropertiesManager.GetFloatProperty("row_five");
                multipliers[5] = PropertiesManager.GetFloatProperty("row_six");
                multipliers[6] = PropertiesManager.GetFloatProperty("row_seven");
                multipliers[7] = PropertiesManager.GetFloatProperty("row_eight");
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

        public void CalculateZones(Rectangle tableInfo)
        {

        }

        /// <summary>
        /// Returns all of the zone counters in an array
        /// </summary>
        /// <returns>An array, holding all of the zone counters</returns>
        public int[] GetRowInformation()
        {
            return new int[]
            {
                _RedGoalieZone,
                _RedDefenceZone,
                _BlueAttackZone,
                _RedMidfieldZone,
                _BlueMidfieldZone,
                _RedAttackZone,
                _BlueDefenceZone,
                _BlueGoalieZone
            };
        }
    }
}