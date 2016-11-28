﻿using System;
using System.IO;
using RestSharp;

namespace Drcom_Dialer.Model.Utils
{
    /// <summary>
    /// 心跳包升级类
    /// </summary>
    static class HeartBeatUpdate
    {
        /// <summary>
        /// 检测DLL是否存在
        /// </summary>
        /// <returns></returns>
        public static bool CheckDLL()
        {
            return File.Exists("gdut-drcom.dll");
        }
        /// <summary>
        /// 检测更新
        /// </summary>
        /// <returns></returns>
        public static bool CheckUpdate()
        {
            RestClient client = new RestClient("https://api.github.com");
            RestRequest request = new RestRequest("/repos/chenhaowen01/gdut-drcom/releases/latest");
            IRestResponse<GithubReleaseResponse> response = client.Execute<GithubReleaseResponse>(request);

            if(response != null && response.Content !="" && response.ResponseStatus == ResponseStatus.Completed)
            {
                if (response.Data.name != GDUT_Drcom.Version)
                    return true;
                else
                    return false;
            }
            //另一个mirror
            client = new RestClient("https://api.github.com");
            request = new RestRequest("/repos/chenhaowen01/gdut-drcom/releases/latest");
            response = client.Execute<GithubReleaseResponse>(request);

            if (response != null && response.Content != "" && response.ResponseStatus == ResponseStatus.Completed)
            {
                if (response.Data.name != GDUT_Drcom.Version)
                    return true;
                else
                    return false;
            }
            return false;
        }
        /// <summary>
        /// 升级DLL
        /// </summary>
        /// <returns></returns>
        public static bool Update()
        {
            RestClient client = new RestClient("https://api.github.com");
            RestRequest request = new RestRequest("/repos/chenhaowen01/gdut-drcom/releases/latest");
            IRestResponse<GithubReleaseResponse> response = client.Execute<GithubReleaseResponse>(request);

            if (response != null && response.Content != "" && response.ResponseStatus == ResponseStatus.Completed)
            {
                if(response.Data.assets != null)
                    foreach(GithubReleaseAssetItem fileName in response.Data.assets)
                    {
                        if(fileName.name == "gdut-drcom.dll")
                        {
                            if (downloadFile(fileName.browser_download_url, "gdut-drcom.dll"))
                                return true;
                        }
                    }
            }

            //mirror
            client = new RestClient("https://api.github.com");
            request = new RestRequest("/repos/chenhaowen01/gdut-drcom/releases/latest");
            response = client.Execute<GithubReleaseResponse>(request);

            if (response != null && response.Content != "" && response.ResponseStatus == ResponseStatus.Completed)
            {
                if (response.Data.assets != null)
                    foreach (GithubReleaseAssetItem fileName in response.Data.assets)
                    {
                        if (fileName.name == "gdut-drcom.dll")
                        {
                            if (downloadFile(fileName.browser_download_url, "gdut-drcom.dll"))
                                return true;
                        }
                    }
            }

            return false;

        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">链接</param>
        /// <param name="path">本地文件</param>
        /// <returns></returns>
        private static bool downloadFile(string url,string path)
        {
            int index = url.IndexOf("/", 10); //懒的用其他的了，这是第三个/的出现的位置

            RestClient client = new RestClient(url.Substring(0,index));
            RestRequest request = new RestRequest(url.Substring(index,url.Length - index));

            byte[] result = client.DownloadData(request);
            if (result.Length < 1024)
                return false;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                fs.Write(result, 0, result.Length);
                fs.Close();
            }
            catch(Exception e)
            {
                Log4Net.WriteLog(e.Message, e);
                return false;
            }

            return true;
        }


    }

    public class GithubReleaseResponse
    {
        public string tag_name;
        public string name;
        public GithubReleaseAssetItem[] assets;
    }

    public class GithubReleaseAssetItem
    {
        public string name;
        public string browser_download_url;
    }
}