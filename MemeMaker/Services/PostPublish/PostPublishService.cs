using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MemeMaker.Entities;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace MemeMaker.Services.PostPublish
{
    public class PostPublishService
    {
        private readonly ulong applicationId;
        private readonly string login;
        private readonly string password;
        private readonly long ownerId;
        private readonly string template;

        public PostPublishService(ulong applicationId, string login, string password, long ownerId, string template)
        {
            this.applicationId = applicationId;
            this.login = login;
            this.password = password;
            this.ownerId = ownerId;
            this.template = template;
        }

        public async Task ProcessAsync(PostGenerateEntity post, string message)
        {
            try
            {
                VkApi client = new VkApi();

                client.Authorize(new ApiAuthParams
                {
                    ApplicationId = this.applicationId,
                    Login = this.login,
                    Password = this.password,
                    Settings = Settings.All
                });

                List<MediaAttachment> attachments = new List<MediaAttachment>();

                if (post is { })
                {
                    UploadServerInfo uploadServer = client.Photo.GetWallUploadServer();
                    byte[] uploadResponse;
                    using (WebClient webClient = new WebClient())
                        uploadResponse = await webClient.UploadFileTaskAsync(uploadServer.UploadUrl, post.Path);

                    string jsonResponse = Encoding.UTF8.GetString(uploadResponse);

                    ReadOnlyCollection<Photo> photos = await client.Photo.SaveWallPhotoAsync(
                        jsonResponse,
                        null
                    );

                    attachments.AddRange(photos);
                }

                await client.Wall.PostAsync(new WallPostParams()
                {
                    OwnerId = this.ownerId,
                    FromGroup = true,
                    Message = string.Format(this.template, message),
                    Attachments = attachments,
                    Signed = false,
                    PublishDate = DateTime.Now.AddMonths(3),
                });
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.Message);
            }
        }
    }
}