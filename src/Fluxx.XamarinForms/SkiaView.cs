#if false
using System;
using System.Collections.Generic;
using Faml.Skia;
using SkiaSharp;
using Xamarin.Forms;
using SKCanvas = SkiaSharp.SKCanvas;

namespace Faml.XamarinForms 
{
    public class SkiaView : View, ISkiaViewController {
        //Action<SkiaCanvas, int, int> onDrawCallback;
        private IList<ISkiaDrawable> _children = new List<ISkiaDrawable>();

        public IList<ISkiaDrawable> Children => _children;

		public SkiaView()  {
		}

		void ISkiaViewController.SendDraw(SKCanvas canvas) {
            canvas.DrawColor(SKColors.White);

            /*
            SKPoint3 direction = new SKPoint3(1.0f, 1.0f, 1.0f);

            using (var paint = new SKPaint())
            using (var filter = SKMaskFilter.CreateEmboss(2.0f, direction, 0.3f, 0.1f))
            {
                paint.IsAntialias = true;
                paint.TextSize = 120;
                paint.TextAlign = SKTextAlign.Center;
                paint.MaskFilter = filter;

                canvas.DrawText("Skia", 50 / 2f, 50/ 2f, paint);
            }
            */

            foreach (ISkiaDrawable child in _children) {
                child.Draw(canvas);
            }
		}

        /*
		protected virtual void Draw(SkiaCanvas canvas)
		{
			onDrawCallback(canvas, (int)this.Width, (int)this.Height);
		}
        */
	}
}
#endif
