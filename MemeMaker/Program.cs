using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MemeMaker.Entities;
using MemeMaker.Services.ImageDownload;
using MemeMaker.Services.ImageLoad;
using MemeMaker.Services.ImageParse;
using MemeMaker.Services.ImageSearch;
using MemeMaker.Services.PostGenerate;
using MemeMaker.Services.PostPublish;
using MemeMaker.Services.QuoteDownload;
using MemeMaker.Services.QuoteLoad;
using Newtonsoft.Json;

namespace MemeMaker
{
    internal static class Program
    {
        public static Config Config { get; private set; } = new Config();

        public static readonly Random Rand = new Random();
        
        private static async Task Main(string[] args)
        {
            if (File.Exists("config.json"))
            {
                try
                {
                    Program.Config = JsonConvert.DeserializeObject<Config>(await File.ReadAllTextAsync("config.json"));
                }
                catch (Exception ex)
                {
                    await Console.Error.WriteLineAsync(ex.Message);
                    Console.ReadLine();
                    return;
                }
            }
            else
            {
                await File.WriteAllTextAsync("config.json", JsonConvert.SerializeObject(new DefaultConfig(), Formatting.Indented));
                Console.WriteLine("A config.json file has been created, please fill it in and restart your application!");
                Console.ReadLine();
                return;
            }
            
            Console.Write("Get new data [y/N]?");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine();
                await Get();
            } else
                Console.WriteLine();

            Console.WriteLine("Loading images...");
            IReadOnlyList<ImageLoadEntity> images = await LoadImagesAsync();
            Console.WriteLine($"Loaded {images.Count} images!");
            
            Console.WriteLine("Loading quotes...");
            IReadOnlyList<QuoteLoadEntity> quotes = await LoadQuotesAsync();
            Console.WriteLine($"Loaded {quotes.Count} quotes!");

            if (images.Count == 0 || quotes.Count == 0)
                throw new Exception("Empty images or quotes data!");

            string path = AppDomain.CurrentDomain.BaseDirectory;
            
            while (true)
            {
                try
                {
                    PostGenerateEntity post = await GenPost(images, quotes);
                    Console.WriteLine($"Meme generated: file://{path}{post.Path}\n{post.QuoteText}");
                    Console.Write("Post? [y/N]?");
                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        await SendPost(post);
                        Console.WriteLine("\t\tPosted!");
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        private static async Task Get()
        {
            await Task.WhenAll(
                GetImagesAsync(),
                GetQuotesAsync()
            );
        }

        private static async Task GetImagesAsync()
        {
            ImageSearchService imageSearcher = new ImageSearchService(
                Program.Config.SerpApiCom.Key,
                0,
                Program.Config.Tags.ToArray()
            );
            IReadOnlyList<ImageSearchEntity> imgSearchResults = await imageSearcher.ProcessAsync();

            ImageParseService imageParser = new ImageParseService(imgSearchResults.ToArray());
            IReadOnlyList<ImageParseEntity> imgParseResults = await imageParser.ProcessAsync();

            ImageDownloadService imageDownloader = new ImageDownloadService(imgParseResults.ToArray());
            IReadOnlyList<ImageDownloadEntity> imgDownloadResults = await imageDownloader.ProcessAsync();
        }

        private static async Task GetQuotesAsync()
        {
            QuoteDownloadService quoteDownloader = new QuoteDownloadService(5000);
            IReadOnlyList<QuoteDownloadEntity> quoteDownloadResults = await quoteDownloader.ProcessAsync();
        }

        private static async Task<IReadOnlyList<ImageLoadEntity>> LoadImagesAsync()
        {
            ImageLoadService imageLoader = new ImageLoadService();
            IReadOnlyList<ImageLoadEntity> imageLoadResults = await imageLoader.ProcessAsync();

            return imageLoadResults;
        }

        private static async Task<IReadOnlyList<QuoteLoadEntity>> LoadQuotesAsync()
        {
            QuoteLoadService quoteLoader = new QuoteLoadService();
            IReadOnlyList<QuoteLoadEntity> quoteLoadResults = await quoteLoader.ProcessAsync();

            return quoteLoadResults;
        }

        private static async Task<PostGenerateEntity> GenPost(IReadOnlyList<ImageLoadEntity> images, IReadOnlyList<QuoteLoadEntity> quotes)
        {
            PostGenerateService postGenerator = new PostGenerateService();

            ImageLoadEntity image = images[Rand.Next(0, images.Count)];
            QuoteLoadEntity quote = quotes[Rand.Next(0, quotes.Count)];

            return await postGenerator.ProcessAsync(image, quote);
        }

        private static async Task SendPost(PostGenerateEntity post)
        {
            PostPublishService postPublisher = new PostPublishService(
                Program.Config.VkCom.AppId,
                Program.Config.VkCom.Login,
                Program.Config.VkCom.Password,
                Program.Config.VkCom.OwnerId,
                Program.Config.Template
            );

            await postPublisher.ProcessAsync(post, post.QuoteText);
        }
    }
}