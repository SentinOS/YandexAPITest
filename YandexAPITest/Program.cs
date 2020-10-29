using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace YandexAPITest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string oauthToken = "<token>";
            IDiskApi diskApi = new DiskHttpApi(oauthToken);
            await DownloadAllFilesInFolder(diskApi);
        }

        static async Task DownloadAllFilesInFolder(IDiskApi diskApi)
        {
            //Getting information about folder /foo and all files in it
            Resource fooResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = "/Path", //Folder on Yandex Disk
            }, CancellationToken.None);

            //Getting all files from response
            IEnumerable<Resource> allFilesInFolder =
                fooResourceDescription.Embedded.Items.Where(item => item.Type == ResourceType.File);

            //Path to local folder for downloading files
            string localFolder = @"D:\Download";

            //Run all downloadings in parallel. DiskApi is thread safe.
            IEnumerable<Task> downloadingTasks =
                allFilesInFolder.Select(file =>
                  diskApi.Files.DownloadFileAsync(path: file.Path,
                                                  localFile: System.IO.Path.Combine(localFolder, file.Name)));

            //Wait all done
            await Task.WhenAll(downloadingTasks);
        }
    }

}
