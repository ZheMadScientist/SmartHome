using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using System;

namespace SmartHome
{
    public class CirclePickerV : View
    {
        private Paint ValueP, AmPm, arcStopper, hoursStopper;
        private Paint[] partPaints, hourPaints;

        private int width, height;
        private float centerX, centerY, radius, smallRadius, hoursRadius, ampmRadius, hoursCircleConst;

        private float angle = -90;
        private float sectionRad = 0.04F;
        private float sectionMinute = 0.066F;
        private float sectionHour = 0.33F;

        public static int percent = 0;
        private int pMinutes = 0;
        private int pHours = 0;

        public static int minutes = 0;
        public static int hours = 0;

        private Color color = Color.ParseColor("#00695C");
        private Color initColor = Color.ParseColor("#80CBC4");

        private int mode;
        private bool isPm = false;

        public CirclePickerV(Context context): this(context, null)
        {
            
        }

        public CirclePickerV(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.pickerMode, 0, 0);
            try
            {
                mode = a.GetInteger(Resource.Styleable.pickerMode_mode, 0);
            }
            finally
            {
                a.Recycle();
            }

            Initialize();
        }

        public CirclePickerV(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.pickerMode, 0, 0);

            try
            {
                mode = a.GetInteger(Resource.Styleable.pickerMode_mode, 0);
            }
            finally
            {
                a.Recycle();
            }

            Initialize();
        }

        private void Initialize()
        {            
            partPaints = new Paint[100];
            for (int i = 0; i < 100; i++) {
                partPaints[i] = new Paint(PaintFlags.AntiAlias);
                partPaints[i].Color = initColor;
                partPaints[i].StrokeWidth = Context.Resources.DisplayMetrics.Density * 5;
            }

            ValueP = new Paint(PaintFlags.AntiAlias);
            ValueP.Color = Color.ParseColor("#B71C1C");
            ValueP.TextAlign = Paint.Align.Center;
            ValueP.TextSize = 120;

            AmPm = new Paint(PaintFlags.AntiAlias);
            AmPm.Color = Color.ParseColor("#B71C1C");
            AmPm.TextAlign = Paint.Align.Center;
            AmPm.TextSize = 120;

            arcStopper = new Paint(PaintFlags.AntiAlias);
            arcStopper.Color = Color.White;
            arcStopper.StrokeWidth = Context.Resources.DisplayMetrics.Density * 5;

            hourPaints = new Paint[12];
            for (int i = 0; i < 12; i++) {
                hourPaints[i] = new Paint(PaintFlags.AntiAlias);
                hourPaints[i].Color = initColor;
                hourPaints[i].StrokeWidth = Context.Resources.DisplayMetrics.Density * 5;
            }

            hoursStopper = new Paint(PaintFlags.AntiAlias);
            hoursStopper.Color = Color.White;
            hoursStopper.StrokeWidth = Context.Resources.DisplayMetrics.Density * 5;

            coloring();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            switch (mode)
            {
                case Constants.Constants.modePercents:
                    for (int i = 0; i < 100; i++)
                    {
                        canvas.DrawArc(0, 0, width, width, angle, 3, true, partPaints[i]);
                        angle += 3.6F;
                    }
                    canvas.DrawCircle(centerX, centerY, smallRadius, arcStopper);

                    canvas.DrawText(percent + "%", centerX, centerY, ValueP);
                    break;

                case Constants.Constants.modeTime:
                    for (int i = 0; i < 60; i++)
                    {
                        canvas.DrawArc(0, 0, width, width, angle, 4, true, partPaints[i]);
                        angle += 6;
                    }
                    angle = -90;
                    
                    canvas.DrawCircle(centerX, centerY, smallRadius, arcStopper);

                    for (int i = 0; i < 12; i++)
                    {
                        canvas.DrawArc(hoursCircleConst, hoursCircleConst, width - hoursCircleConst, width - hoursCircleConst, angle, 28, true, hourPaints[i]);
                        angle += 30;
                    }
                    canvas.DrawCircle(centerX, centerY, hoursRadius - 50, hoursStopper);
                    
                    if (isPm) {
                        canvas.DrawText("Pm", centerX, centerY + 100, AmPm);                        
                    }
                    else { canvas.DrawText("Am", centerX, centerY + 100, AmPm); }
                    canvas.DrawText(hours + ":" + minutes, centerX, centerY, ValueP);
                    break;
            }
     
            coloring();
            angle = -90;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
            width = MeasuredWidth;
            height = MeasuredWidth;
            smallRadius = MeasuredWidth / 2.1F;
            hoursRadius = MeasuredWidth / 2.5F;
            ampmRadius = MeasuredWidth / 4F;
            hoursCircleConst = centerX - hoursRadius;
            updateCenter(width, height);
        }

        private void updateCenter(int width, int height)
        {
            centerX = width / 2;
            centerY = height / 2;
            radius = width - centerX;
        }

        private bool isInside(float x, float y)
        {
            return Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(centerX - PaddingLeft, 2);
        }

        private bool isHours(float x, float y)
        {
            return Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(hoursRadius - PaddingLeft, 2) && Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) >= Math.Pow(width / 4 - PaddingLeft, 2);
        }

        private void percentStabilization() {
            if (percent >= 100)
            {
                percent = 99;
            }
            if (percent == -1)
            {
                percent = 100;
            }
        }

        private void timeStabilization()
        {
            if (pMinutes >= 60 || pMinutes == -1)
            {
                pMinutes = 59;
            }
            if (pHours >= 12 || pHours == -1)
            {
                pHours = 12;
            }
            minutes = pMinutes;
            hours = pHours;
            if (isPm && hours < 24)
                hours += 11;
        }

        private void coloring() {
            if (mode == Constants.Constants.modePercents)
            {
                for (int i = 0; i < 100; i++)
                {
                    partPaints[i].Color = initColor;
                }
                for (int i = 0; i < percent; i++)
                {
                    partPaints[i].Color = color;
                }
            }
            else {
                for (int i = 0; i < 60; i++)
                {
                    partPaints[i].Color = initColor;
                }
                for (int i = 0; i < pMinutes; i++)
                {
                    partPaints[i].Color = color;
                }

                for (int i = 0; i < 12; i++)
                {
                    hourPaints[i].Color = initColor;
                }
                for (int i = 0; i < pHours; i++)
                {
                    hourPaints[i].Color = color;
                }
            }
        }

        private void setter(float x, float y, float relevantRadius, int quarter) {
            switch (quarter) {
                case (1):
                    percent = (int) (((x - centerX) / relevantRadius) / sectionRad);
                    Log.Debug("1st quarter", "percent = " + percent + " , acos = " + Math.Acos((x - centerX) / relevantRadius));
                    break;

                case (2):
                    percent = 75;
                    percent += (int) (Math.Asin((y - centerY) / relevantRadius) / sectionRad * (-1));
                    Log.Debug("2st quarter", "percent = " + percent + " , sin = " + (y - centerY) / relevantRadius + " , y-cen = " + (y - centerY));
                    break;

                case (3):
                    percent = 50;
                    percent += (int) ((((x - centerX) / relevantRadius)) / sectionRad) * (-1);
                    Log.Debug("3st quarter", "percent = " + percent);
                    break;

                case (4):
                    percent = 25;
                    percent += (int)(Math.Acos((x - centerX) / relevantRadius) / sectionRad);
                    Log.Debug("4st quarter", "percent = " + percent);
                    break;
            }
            --percent;
            percentStabilization();
        }

        private void timeSetter(float x, float y, float relevantRadius, int quarter, bool ishours )
        {
            switch (quarter)
            {
                case (1):
                    if (ishours)
                    {                        
                        pHours = (int)(((x - centerX) / relevantRadius) / sectionHour);
                    }
                    else
                    {
                        pMinutes = (int)(((x - centerX) / relevantRadius) / sectionMinute);
                    }
                    break;

                case (2):
                    if (ishours)
                    {
                        pHours = 9;
                        pHours += (int)(Math.Asin((y - centerY) / relevantRadius) / sectionHour * (-1));
                    }
                    else
                    {
                        pMinutes = 45;
                        pMinutes += (int)(Math.Asin((y - centerY) / relevantRadius) / sectionMinute * (-1));
                    }
                    break;

                case (3):
                    if (ishours)
                    {
                        pHours = 6;
                        pHours += (int)((((x - centerX) / relevantRadius)) / sectionHour) * (-1);
                    }
                    else {
                        pMinutes = 30;
                        pMinutes += (int)((((x - centerX) / relevantRadius)) / sectionMinute) * (-1);
                    }
                    break;

                case (4):
                    if (ishours)
                    {
                        pHours = 3;
                        pHours += (int)(Math.Acos((x - centerX) / relevantRadius) / sectionHour);
                    }
                    else
                    {
                        pMinutes = 15;
                        pMinutes += (int)(Math.Acos((x - centerX) / relevantRadius) / sectionMinute);
                    }
                    break;
            }
            timeStabilization();
        }

        private int quarter(float x, float y) {
            if ((x - centerX) > 0 && (y - centerY) < 0) {
                return 1;
            }

            if ((x - centerX) < 0 && (y - centerY) < 0)
            {
                return 2;
            }

            if ((x - centerX) < 0 && (y - centerY) > 0)
            {
                return 3;
            }

            if ((x - centerX) > 0 && (y - centerY) > 0)
            {
                return 4;
            }
            else { return 1; }     
        }

        bool ishours = true;
        public override bool OnTouchEvent(MotionEvent e)
        {
            float x = e.GetX();
            float y = e.GetY();
            double dsumm = Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2);
            float relevantR = (float) Math.Sqrt(dsumm);            
            
            switch (e.Action) {
                case (MotionEventActions.Down):
                    if (isInside(x, y)) {
                        ishours = isHours(x, y);
                        if (mode == Constants.Constants.modePercents) { setter(x, y, relevantR, quarter(x, y)); }
                        if (mode == Constants.Constants.modeTime) { timeSetter(x, y, relevantR, quarter(x, y), ishours); }

                        if (x >= centerX - 80 && x <= centerX + 80 && y <= centerY + 100 + 80 && y >= centerY + 80)
                        {
                            isPm = !isPm;
                            if (isPm)
                                hours += 12;
                        }

                    }
                    break;

                case (MotionEventActions.Up):

                    break;

                case (MotionEventActions.Move):
                    if (isInside(x, y)) {
                        if (mode == Constants.Constants.modePercents) { setter(x, y, relevantR, quarter(x, y)); }
                        if (mode == Constants.Constants.modeTime) { timeSetter(x, y, relevantR, quarter(x, y), ishours); }
                    }
                    break;
            }
            Invalidate();
            return true;
        }

    }
}