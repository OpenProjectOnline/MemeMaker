using System;
using System.IO;
using System.Threading.Tasks;
using MemeMaker.Entities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace MemeMaker.Services.PostGenerate
{
    public class PostGenerateService
    {
        public async Task<PostGenerateEntity?> ProcessAsync(ImageLoadEntity image, QuoteLoadEntity quote)
        {
            try
            {
                const string dir = "data/posts";
                Directory.CreateDirectory(dir);
                string id = $"{Guid.NewGuid().ToString()}";
                string ext = ".png"; //Path.GetExtension(image.Path);

                PostGenerateEntity result = new PostGenerateEntity()
                {
                    Id = id,
                    ImageId = image.Id,
                    ImageCurrentPath = image.CurrentPath,
                    QuoteText = quote.QuoteText,
                    QuoteId = quote.GetUniqueHash(),
                    QuoteCurrentPath = quote.CurrentPath,
                    Path = $"{dir}/{id}{ext}",
                    CurrentPath = $"{dir}/{id}.json",
                };

                using (Image meme = await Image.LoadAsync(image.Path))
                {
                    meme.Mutate(
                        x => x.DrawText(
                            new DrawingOptions()
                            {
                                TextOptions = new TextOptions()
                                {
                                    ApplyKerning = true,
                                    TabWidth = 8,
                                    WrapTextWidth = meme.Width - 40,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    RenderColorFonts = true,
                                },
                            },
                            quote.QuoteText,
                            GetBoldFont(meme.Height / 20),
                            Brushes.Solid(Color.White),
                            Pens.Solid(Color.Black, 1f),
                            new PointF(20, meme.Height / 2f)
                        )
                    );
                    
                    meme.Mutate(
                        x => x.DrawText(
                            new DrawingOptions()
                            {
                                TextOptions = new TextOptions()
                                {
                                    ApplyKerning = true,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    RenderColorFonts = true,
                                },
                            },
                            Program.Config.Watermark,
                            GetMediumFont(14),
                            new RecolorBrush(Color.Black, Color.White, 0.5f),
                            new PointF(5, meme.Height - 19)
                        )
                    );

                    await meme.SaveAsync(result.Path, new PngEncoder());
                }

                await File.WriteAllTextAsync(result.CurrentPath, result.ToJson());

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return null;
            }
        }

        private Font GetMediumFont(int size) => new FontCollection().Install("fonts/alumni-sans/AlumniSans-Medium.ttf").CreateFont(size, FontStyle.Regular);
        private Font GetBoldFont(int size) => new FontCollection().Install("fonts/alumni-sans/AlumniSans-ExtraBold.ttf").CreateFont(size, FontStyle.Regular);
    }
}