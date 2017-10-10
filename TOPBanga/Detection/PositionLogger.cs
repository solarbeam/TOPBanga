using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOPBanga.Interface;
using TOPBanga.Util;

namespace TOPBanga.Detection
{
    class PositionLogger
    {
        /**
         * Defines the object's position on the x axis
         **/
        public float posX { get; private set; }
        /**
         * Defines the object's position on the y axis
         **/
        public float posY { get; private set; }
        /**
         * Defines the relative delta between two different
         * positions. Will be used for statistics ( Speed and etc. )
         **/
        private int delta;
        private int previous_delta;
        private SURF_Form form;
        private VideoFromFile colorForm;
        public Point lastPos;
        private IWrite eventLog;
        private IDetector detector;
        public PositionLogger(SURF_Form form)
        {
            this.form = form;
            this.eventLog = new EventLog(Directory.GetCurrentDirectory() + "/event.log");
        }
        public PositionLogger(VideoFromFile form,IDetector detector)
        {
            this.colorForm = form;
            this.detector = detector;
            this.eventLog = new EventLog(Directory.GetCurrentDirectory() + "event.log");
        }
        /**
         * Add this function to an Event for best results
         */
        public void Update(object o, EventArgs e)
        {
            this.previous_delta = this.delta;
            this.delta = (int)Math.Abs((this.posX - this.lastPos.X)*(this.posX - this.lastPos.X) + (this.posY - this.lastPos.Y)*(this.posY - this.lastPos.Y));
            /**
            * Invoke function later so that
            * safe access is insured when coming
            * from a thread
            */
            if ( form != null )
            {
                form.Invoke(new MethodInvoker(delegate { form.setDeltaText("Relative delta: " + this.delta); }));
            }
            else
            {
                colorForm.Invoke(new MethodInvoker(delegate { colorForm.setDeltaText("Relative delta: " + this.delta); }));
            }
            eventLog.Write("Relative delta: " + this.delta);
            this.posX = lastPos.X;
            this.posY = lastPos.Y;

            float x, y, radius;
            detector.DetectBall(out x, out y, out radius);

            this.posX = x;
            this.posY = y;

            if (this.delta == 0)
            {
                switch (this.previous_delta)
                {
                    case 0:
                        {
                            eventLog.Write("Object lost!");
                            break;
                        }
                    default:
                        {
                            eventLog.Write("Continuing to search");
                            break;
                        }
                }
            }
        }
    }
}
