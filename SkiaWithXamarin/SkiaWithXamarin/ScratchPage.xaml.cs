using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkiaWithXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScratchPage : ContentPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        string height;
        string width;

        //color is used to draw rectangle
        SKPaint color = new SKPaint
        {
            Color = SKColors.CornflowerBlue
        };

        SKRect rect = new SKRect();


        // paint is used to create skpath
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            /*here by making the blend mode to clear we make the scratched area transparent so that in the background 
            we can show the image and text*/
            BlendMode = SKBlendMode.Clear,
            StrokeWidth = 100,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        public ScratchPage()
        {
            InitializeComponent();
        }

        private void CanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();
            height = canvasView.Height.ToString();
            width = canvasView.Width.ToString();
            float w = float.Parse(width);
            float h = float.Parse(height);
            //canvas.DrawRect(0, 0,w,h, color);
            canvas.DrawRect(0, 0, 2000, 2400, color);
            //canvas.DrawRect(10, 10, 1000, 1000, color1);
            var area = rect.Width * rect.Height;
            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
                rect.Size = new SKSize(100, 100);
                path.GetBounds(out rect);


                if(area >= 1500000)
                {
                    btn.IsVisible = true;
                    btn.IsEnabled = true;
                }

            }

            foreach (SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
                rect.Size = new SKSize(100, 100);
                path.GetBounds(out rect);


                if (area >= 1100000)
                {
                    btn.IsVisible = true;
                    btn.IsEnabled = true;
                    /* color = new SKPaint
                     {
                         Color = SKColors.White
                     };*/
                }
            }


        }

        private void TouchEffect_OnTouchAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);

                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }

        }
        SKPoint ConvertToPixel(TouchTrackingPoint pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        private async void Btn_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage());
        }
    }
}