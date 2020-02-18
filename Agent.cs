using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace N2ImageAgentGithub
{

    public class Agent
    {

        private GitHubClient GithubClient;


        private Repository Repo { get; set; }

        public Agent(ContainerInfo containerInfo)
        {
            if (containerInfo == null)
            {
                throw new ArgumentNullException("containerInfo");
            }

            GithubClient = new GitHubClient(new ProductHeaderValue(containerInfo.ProductHeaderValue));
            var tokenAuth = new Credentials(containerInfo.GithubToken);
            GithubClient.Credentials = tokenAuth;


            Repo = GithubClient.Repository.Get(containerInfo.GithubName, containerInfo.RepoName).Result;



        }

        /// <summary>
        /// Get Image thumb url from Github
        /// if the file not existed I will copy from N2SOURCE/{id}.gif 
        /// And make thumb file to N2_{w}x{h}/id.gif and return
        /// 取得縮圖的網址，如果沒有的話我會從 N2SOURCE/{id}.gif  來製作縮圖
        /// 放置 N2_{w}x{h}/id.gif 並且回傳網址
        /// </summary>
        /// <param name="id">圖片編號</param>
        /// <param name="w">寬度 如果0 隨高度換算</param>
        /// <param name="h">高度 如果0 隨寬度換算</param>
        /// <returns></returns>
        public string GetImageThumbFromSource(string id, int w, int h)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            RepositoryContent repoContent = GetFileRepositoryContent("N2SOURCE" + "/" + id + ".gif");
            if (repoContent == null) return null;

            if (w == 0 && h == 0)
            {
                return repoContent.DownloadUrl;
            }

            RepositoryContent repoContentThumb = GetFileRepositoryContent("N2_" + w + "x" + h + "/" + id + ".gif");
            if (repoContentThumb != null)
            {
                return repoContentThumb.DownloadUrl;
            }

            //Get Image From Source 
            var bytes = Convert.FromBase64String(repoContent.EncodedContent);
            var sourceImage = Image.FromStream(new MemoryStream(bytes));

            var thumbHandler = new ImageUtility();

            Image source2 = null;
            //按照寬度，高度隨意
            if (w > 0 && h == 0)
            {
                source2 = thumbHandler.MakeThumbnail(sourceImage, w, h, "W");

            }


            ////按照高度，寬度隨意
            if (h > 0 && w == 0)
            {
                source2 = thumbHandler.MakeThumbnail(sourceImage, w, h, "H");

            }

            ////強制任性
            if (h > 0 && w > 0)
            {
                source2 = thumbHandler.MakeThumbnail(sourceImage, w, h, "WH");

            }

            var memStream = new MemoryStream();
            source2.Save(memStream, sourceImage.RawFormat);
            UpoloadImage(id, memStream.ToArray(), "upload by N2ImageAgent", "N2_" + w + "x" + h);
            sourceImage.Dispose();
            source2.Dispose();

            return GetFileRepositoryContent("N2_" + w + "x" + h + "/" + id + ".gif").DownloadUrl;

        }



        /// <summary>
        /// Is file existed.
        /// 是否檔案存在
        /// </summary>
        /// <param name="pathFilename"></param>
        /// <returns></returns>
        protected bool IsFileExisted(string pathFilename)
        {
            try
            {
                var res = GithubClient.Repository.Content.GetAllContents(Repo.Id, pathFilename).Result;
                if (res.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Is  N2INFO/{id}.gif existed
        /// 是否 N2INFO/{id}.gif 存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsImageInfoExisted(string id)
        {
            return IsFileExisted("N2INFO" + "/" + id + ".json");
        }


        /// <summary>
        /// Is thumb existed in  N2_{w}x{h}/{id}.gif  or N2SOURCE/{id}.gif
        /// 是否縮圖存在於  N2_{w}x{h}/{id}.gif  or N2SOURCE/{id}.gif
        /// </summary>
        /// <param name="id"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public bool IsImageThumbExisted(string id, int w, int h)
        {
            if (w == 0 && h == 0)
            {
                return IsFileExisted("N2SOURCE" + "/" + id + ".gif");
            }
            return IsFileExisted("N2_" + w + "x" + h + "/" + id + ".gif");
        }


        /// <summary>
        /// Upload Image info
        /// 上傳圖片資訊
        /// </summary>
        /// <param name="imageInfo"></param>
        /// <param name="message"></param>
        public void UpoloadImageInfo(ImageInfo imageInfo, string message)
        {

            if (imageInfo == null) throw new ArgumentNullException("imageInfo");



            DeleteImageInfo(imageInfo.Id);

            var updateRequest = new UpdateFileRequest(message, JsonConvert.SerializeObject(imageInfo), "SHA", true);

            var res = GithubClient.Repository.Content.UpdateFile(Repo.Id, "N2INFO" + "/" + imageInfo.Id + ".json", updateRequest).Result;

        }


        /// <summary>
        /// Upload image to N2SOURCE/{id}.gif
        /// 上傳圖片至 N2SOURCE/{id}.gif
        /// </summary>
        /// <param name="id">圖片編號</param>
        /// <param name="localImagePath">本地圖片位置</param>
        /// <param name="message">comment</param>
        public void UpoloadImageToSource(string id, string localImagePath, string message)
        {
            if (string.IsNullOrEmpty(localImagePath)) throw new ArgumentNullException("localImagePath");
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            if (!File.Exists(localImagePath))
            {
                throw new FileNotFoundException("local image not found.");
            }

            DeleteSource(id);

            var updateRequest = new UpdateFileRequest(message, Convert.ToBase64String(File.ReadAllBytes(localImagePath)), "SHA", false);

            var res = GithubClient.Repository.Content.UpdateFile(Repo.Id, "N2SOURCE" + "/" + id + ".gif", updateRequest).Result;

        }

        /// <summary>
        /// Upload image to N2SOURCE/{id}.gif
        /// 上傳圖片至 N2SOURCE/{id}.gif
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imageBytes"></param>
        /// <param name="message"></param>
        public void UpoloadImageToSource(string id, byte[] imageBytes, string message)
        {

            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            if (imageBytes == null) throw new ArgumentNullException("imageBytes");

            DeleteSource(id);
            var updateRequest = new UpdateFileRequest(message, Convert.ToBase64String(imageBytes), "SHA", false);

            var res = GithubClient.Repository.Content.UpdateFile(Repo.Id, "N2SOURCE" + "/" + id + ".gif", updateRequest).Result;

        }


        private void UpoloadImage(string id, byte[] imageBytes, string message, string path)
        {

            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            if (imageBytes == null) throw new ArgumentNullException("imageBytes");

            DeleteFile(path + "/" + id + ".gif");
            var updateRequest = new UpdateFileRequest(message, Convert.ToBase64String(imageBytes), "SHA", false);
            var res = GithubClient.Repository.Content.UpdateFile(Repo.Id, path + "/" + id + ".gif", updateRequest).Result;

        }

        /// <summary>
        /// Is Source Image existed in N2SOURCE/{id}.gif
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsSourceExisted(string id)
        {
            return IsImageInfoExisted("N2SOURCE" + "/" + id + ".gif");
        }

        /// <summary>
        /// Get Path RepositoryContent
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private RepositoryContent GetFileRepositoryContent(string filepath)
        {
            try
            {
                var res = GithubClient.Repository.Content.GetAllContents(Repo.Id, filepath).Result;
                if (res.Count > 0)
                {
                    return res[0];
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get Source Image RepositoryContent
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private RepositoryContent GetSourceRepositoryContent(string id)
        {
            try
            {
                var res = GithubClient.Repository.Content.GetAllContents(Repo.Id, "N2SOURCE" + "/" + id + ".gif").Result;
                if (res.Count > 0)
                {
                    return res[0];
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Delete File  
        /// </summary>
        /// <param name="pathfile"></param>
        public async void DeleteFile(string pathfile)
        {
            try
            {
                var res = GithubClient.Repository.Content.GetAllContents(Repo.Id, pathfile).Result;
                if (res.Count > 0)
                {
                    await GithubClient.Repository.Content.DeleteFile(Repo.Id, pathfile, new DeleteFileRequest("delete file", res[0].Sha));

                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Delete Image Info from N2INFO/{id}.json
        /// </summary>
        /// <param name="id"></param>
        public async void DeleteImageInfo(string id)
        {
            try
            {
                var res = GithubClient.Repository.Content.GetAllContents(Repo.Id, "N2INFO" + "/" + id + ".json").Result;
                if (res.Count > 0)
                {
                    await GithubClient.Repository.Content.DeleteFile(Repo.Id, "N2INFO/" + id + ".json", new DeleteFileRequest("delete file", res[0].Sha));

                }

            }
            catch
            {

            }
        }

        /// <summary>
        /// Delete Source from N2SOURCE/{id}.gif
        /// </summary>
        /// <param name="id"></param>
        public async void DeleteSource(string id)
        {

            var res = GetSourceRepositoryContent(id);
            if (res != null)
            {
                await GithubClient.Repository.Content.DeleteFile(Repo.Id, "N2SOURCE/" + id + ".gif", new DeleteFileRequest("delete file", res.Sha));

            }

        }


        /// <summary>
        /// Delete All Image Thumb from all Sizes.
        /// 刪除所有某id 的縮圖，預設 delay 1000 , 是因為刪得太快速他會莫名的不成功
        /// </summary>
        /// <param name="id"></param>
        /// <param name="delay">call fast will be fail. unit is milliseconds</param>
        public void DeleteAllImageById(string id, int delay = 1000)
        {
            var res = GithubClient.Repository.Content.GetAllContents(Repo.Id, "/").Result;
            if (res.Count > 0)
            {
                foreach (var r in res)
                {
                    if (r.Type.Value == ContentType.Dir)
                    {
                        if (r.Name.StartsWith("N2_"))
                        {
                            try
                            {
                                GithubClient.Repository.Content.DeleteFile(Repo.Id, r.Name + "/" + id + ".gif", new DeleteFileRequest("delete file", GithubClient.Repository.Content.GetAllContents(Repo.Id, r.Name + "/" + id + ".gif").Result[0].Sha));
                                Thread.Sleep(delay);
                            }

                            catch (Exception ex)
                            {
                                Thread.Sleep(delay);
                                continue;
                            }
                        }

                    }
                }
            }
            GC.Collect();
        }

    }
}
