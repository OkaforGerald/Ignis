using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using System.Net.Http;
using System.Reflection;

namespace Ignis.Client.Pages
{
    public partial class Edit
    {
        private SKGLView skglView;
        private ElementReference canvasContainer;
        private SKBitmap originalBitmap;
        private bool isLoaded = false;
        private int containerWidth;
        private int containerHeight;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadImage("https://i.imgur.com/bSNdqGY.jpeg");
        }

        private void Repaint()
        {
            if (skglView != null) skglView.Invalidate();
        }


        private void SetBitmap(SKBitmap bitmap)
        {
            if (originalBitmap != null)
            {
                originalBitmap.Dispose();
                originalBitmap = null;
            }
            originalBitmap = bitmap;
            Repaint();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dimensions = await JSRuntime.InvokeAsync<int[]>("getContainerDimensions", canvasContainer);
                containerWidth = dimensions[0];
                containerHeight = dimensions[1];
                StateHasChanged();
            }
        }

        public async Task LoadImage(string url)
        {
            try
            {
                using (Stream imgStream = await HttpClient.GetStreamAsync(url))
                using (MemoryStream memStream = new MemoryStream())
                {
                    await imgStream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);

                    var imgCodec = SKCodec.Create(memStream);
                    var map = SKBitmap.Decode(imgCodec);
                    SetBitmap(map);
                };
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message.ToUpper());
            }
            isLoaded = true;
        }

        private void PaintSurface(SKPaintGLSurfaceEventArgs args)
        {
            var canvas = args.Surface.Canvas;
            canvas.Clear(SKColors.Black);

            var canvasBounds = new SKRectI(0, 0, args.BackendRenderTarget.Width, args.BackendRenderTarget.Height);
            if (isLoaded && originalBitmap != null)
            {
                //var rect = GetRenderRect(originalBitmap, new SKSize(containerWidth, containerHeight));
                var rect = GetRenderRect(originalBitmap, canvasBounds.Size);
                canvas.DrawBitmap(originalBitmap, rect);
            }
        }

        private SKRect GetRenderRect(SKBitmap bmp, SKSize size)
        {
            float srcAspectRatio = bmp.Width / (float)bmp.Height;
            float dstAspectRatio = size.Width / size.Height;

            float scaleFactor;
            float renderWidth, renderHeight;

            if (srcAspectRatio > dstAspectRatio)
            {
                renderWidth = size.Width;
                renderHeight = renderWidth / srcAspectRatio;
            }
            else
            {
                renderHeight = size.Height;
                renderWidth = renderHeight * srcAspectRatio;
            }

            var origin = new SKPoint((size.Width - renderWidth) / 2.0f, (size.Height - renderHeight) / 2.0f);
            return new SKRect(origin.X, origin.Y, renderWidth + origin.X, renderHeight + origin.Y);
        }

        private void ApplyGreyscale()
        {
            if (originalBitmap != null)
            {
                using (var canvas = new SKCanvas(originalBitmap))
                {
                    using (var paint = new SKPaint())
                    {
                        var colorMatrix = new float[]
                        {
                        0.21f, 0.72f, 0.07f, 0, 0,
                        0.21f, 0.72f, 0.07f, 0, 0,
                        0.21f, 0.72f, 0.07f, 0, 0,
                        0,     0,     0,     1, 0
                        };

                        paint.ColorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
                        canvas.DrawBitmap(originalBitmap, 0, 0, paint);
                    }
                }
                skglView.Invalidate();
            }
        }
    }
}