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

        private int RedGoalieZone;
        private int RedDefenceZone;
        private int BlueAttackZone;
        private int RedMidfieldZone;
        private int BlueMidfieldZone;
        private int RedAttackZone;
        private int BlueDefenceZone;
        private int BlueGoalieZone;

        public void checkRow(PointF lastBallCoordinates, ref CurrentEvent currentEvent)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Contains(lastBallCoordinates.X, lastBallCoordinates.Y))
                {
                    switch (i)
                    {
                        case 0:
                            currentRow = Row.RedGoalie;
                            RedGoalieZone++;
                            break;
                        case 1:
                            currentRow = Row.RedDefence;
                            RedDefenceZone++;
                            break;
                        case 2:
                            currentRow = Row.BlueAttack;
                            BlueAttackZone++;
                            break;
                        case 3:
                            currentRow = Row.RedMidfield;
                            RedMidfieldZone++;
                            break;
                        case 4:
                            currentRow = Row.BlueMidfield;
                            BlueMidfieldZone++;
                            break;
                        case 5:
                            currentRow = Row.RedAttack;
                            RedAttackZone++;
                            break;
                        case 6:
                            currentRow = Row.BlueDefence;
                            BlueDefenceZone++;
                            break;
                        case 7:
                            currentRow = Row.BlueGoalie;
                            BlueGoalieZone++;
                            break;
                    }

                    if (currentEvent == CurrentEvent.None)
                        currentEvent = CurrentEvent.PositionChanged;

                    break;
                }
            }
        }

        public void CalculateRows(System.Drawing.Rectangle tableZone)
        {
            // Declare constants
            float[] multipliers = new float[8];
            multipliers[0] = 0.1158f;
            multipliers[1] = 0.0623f;
            multipliers[2] = 0.0865f;
            multipliers[3] = 0.1005f;
            multipliers[4] = 0.1145f;
            multipliers[5] = 0.1348f;
            multipliers[6] = 0.1832f;
            multipliers[7] = 0.2124f;

            rows = new RectF[8];

            rows[0] = new RectF(tableZone.Left, tableZone.Top,
                                tableZone.Right, tableZone.Top + (tableZone.Height * multipliers[0]));

            for (int i = 1; i < 8; i++)
            {
                float toAdd = 0;
                for (int j = i; j != 0; j--)
                {
                    toAdd += rows[j - 1].Height();
                }
                rows[i] = new RectF(tableZone.Left, rows[i - 1].Bottom,
                                    tableZone.Right, tableZone.Top + toAdd + (tableZone.Height * multipliers[i]));
            }
        }
    }
}