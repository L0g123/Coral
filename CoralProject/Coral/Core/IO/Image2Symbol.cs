using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Coral.Core.IO
{
    public interface IImage2SymbolConverter
    {
        SymbolBuffer Convert(Stream imageStream, Vector2i targetSizeInSymbols);
        Task<SymbolBuffer> ConvertAsync(string path, Vector2i targetSizeInSymbols, CancellationToken ct = default);
    }


    /// <summary>
    /// Converts raster images into "SymbolBuffer"
    /// </summary>
    public class Image2SymbolConverter : IImage2SymbolConverter
    {
        private const char HalfBlock = '▀';

        public SymbolBuffer Convert(Stream imageStream, Vector2i targetSizeInSymbols)
        {
            using Image<Rgba32> image = Image.Load<Rgba32>(imageStream);
            return BuildBuffer(image, targetSizeInSymbols);
        }

        public async Task<SymbolBuffer> ConvertAsync(string path, Vector2i targetSizeInSymbols, CancellationToken ct = default)
        {
            using Image<Rgba32> image = await Image.LoadAsync<Rgba32>(path, ct);
            ct.ThrowIfCancellationRequested();
            return BuildBuffer(image, targetSizeInSymbols);
        }

        private static SymbolBuffer BuildBuffer(Image<Rgba32> image, Vector2i targetSizeInSymbols)
        {
            int targetWidth = targetSizeInSymbols.X;
            int targetHeight = targetSizeInSymbols.Y;

            if (targetWidth <= 0 || targetHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(targetSizeInSymbols),
                    "Target size must be positive in both dimensions.");
            }

            // Each symbol encodes two stacked pixels, so the pixel-space height is doubled
            int pixelWidth = targetWidth;
            int pixelHeight = targetHeight * 2;

            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(pixelWidth, pixelHeight),
                Mode = ResizeMode.Stretch,
                Sampler = KnownResamplers.Bicubic
            }));

            var buffer = new SymbolBuffer(targetSizeInSymbols);

            for (int y = 0; y < targetHeight; y++)
            {
                int topRow = y * 2;
                int bottomRow = topRow + 1;

                for (int x = 0; x < targetWidth; x++)
                {
                    Rgba32 topPixel = image[x, topRow];
                    Rgba32 bottomPixel = image[x, bottomRow];

                    Color fg = (topPixel.R, topPixel.G, topPixel.B, topPixel.A);
                    Color bg = (bottomPixel.R, bottomPixel.G, bottomPixel.B, bottomPixel.A);

                    buffer[x, y] = new ConsoleSymbol(new Color2(fg, bg), HalfBlock);
                }
            }

            return buffer;
        }
    }
}
