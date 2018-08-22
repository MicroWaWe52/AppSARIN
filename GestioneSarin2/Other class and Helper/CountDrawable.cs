using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace GestioneSarin2.Other_class_and_Helper
{
    class CountDrawable : Drawable
    {
        private Paint mBadgePaint;
        private Paint mTextPaint;
        private Rect mTxtRect = new Rect();

        private String mCount = "";
        private bool mWillDraw;

        public CountDrawable(Context context)
        {
            var mTextSize = 45;
            mBadgePaint = new Paint
            {
                Color = Color.ParseColor("#51444b"),
                AntiAlias = true

            };
            mBadgePaint.SetStyle(Paint.Style.Fill);
            mTextPaint = new Paint
            {
                Color = Color.ParseColor("#f2efe8"),
                
                TextSize = mTextSize,
                TextAlign = Paint.Align.Center,
                AntiAlias = true

            };
            AssetManager am = context.Assets;
            Typeface tvDoc = Typeface.CreateFromAsset(am, "FiraSans-Regular.ttf");
            mTextPaint.SetTypeface(tvDoc);

        }
        public override void Draw(Canvas canvas)
        {
            if (!mWillDraw)
            {
                return;
            }

            var bounds = Bounds;
            var width = bounds.Right - bounds.Left;
            var height = bounds.Bottom - bounds.Top;
            var radius = ((Math.Max(width, height) / 2)) / 2;
            var centerX = (width - radius - 1) + 5;
            var centerY = radius - 5;
            if (mCount.Length <= 2)
            {
                canvas.DrawCircle(centerX, centerY, (int)(radius + 5.5), mBadgePaint);

            }
            else
            {
                canvas.DrawCircle(centerX, centerY, (int)(radius + 6.5), mBadgePaint);
            }
            mTextPaint.GetTextBounds(mCount, 0, mCount.Length, mTxtRect);
            var textHeight = mTxtRect.Bottom - mTxtRect.Top;
            var textY = centerY + (textHeight / 2f);
            canvas.DrawText(mCount.Length > 2 ? "99+" : mCount, centerX, textY, mTextPaint);
        }

        public override void SetAlpha(int alpha)
        {
            throw new NotImplementedException();
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
            throw new NotImplementedException();
        }

        public override int Opacity => 0;
        public void setCount(String count)
        {
            mCount = count;

            // Only draw a badge if there are notifications.
            mWillDraw = !count.Equals("0");
            InvalidateSelf();
        }


    }
}