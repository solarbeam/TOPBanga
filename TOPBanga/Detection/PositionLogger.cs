using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOPBanga.Util;

namespace TOPBanga.Detection
{
    class PositionLogger
    {
        /**
         * Defines the object's position on the x axis
         **/
        private int posX;
        /**
         * Defines the object's position on the y axis
         **/
        private int posY;
        /**
         * Defines the relative delta between two different
         * positions. Will be used for statistics ( Speed and etc. )
         **/
        private int delta;
        private int previous_delta;
        private SURF_Form form;
        public Point lastPos;
        private EventLog eventLog;
        public PositionLogger(SURF_Form form)
        {
            this.form = form;
            Console.WriteLine(Directory.GetCurrentDirectory());
            this.eventLog = new EventLog(Directory.GetCurrentDirectory() + "/event.log");
        }
        /**
         * Add this function to an Event for best results
         */
        public void Update(object o, EventArgs e)
        {
            this.previous_delta = this.delta;
            this.delta = Math.Abs((this.posX - lastPos.X) ^ 2 + (this.posY - lastPos.Y) ^ 2);
            form.Invoke(new MethodInvoker(delegate { form.setDeltaText("Relative delta: " + this.delta); }));
            eventLog.WriteEvent("Relative delta: " + this.delta);
            this.posX = lastPos.X;
            this.posY = lastPos.Y;
            if (this.delta == 0)
            {
                switch (this.previous_delta)
                {
                    case 0:
                        {
                            eventLog.WriteEvent("Object lost!");
                            break;
                        }
                    default:
                        {
                            eventLog.WriteEvent("Continuing to search");
                            break;
                        }
                }
            }
        }
        public int getPosX()
        {
            return this.posX;
        }
        public int getPosY()
        {
            return this.posY;
        }
    }
}
