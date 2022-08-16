using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace WinUniversalTool.Models
{
    public static class Downloader
    {
        public static async Task downloadFile(Uri downloadURL, StorageFile file, IProgress<int> progression)
        {
            DownloadOperation downloadOperation;
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            BackgroundDownloader backgroundDownloader = new BackgroundDownloader();

            //Set url and file
            downloadOperation = backgroundDownloader.CreateDownload(downloadURL, file);


            Progress<DownloadOperation> progress = new Progress<DownloadOperation>((dOperation) =>
            {
                var _total = (long)dOperation.Progress.TotalBytesToReceive;
                var _received = (long)dOperation.Progress.BytesReceived;
                int _progress = (int)(100 * (double)(_received / (double)_total));
                switch (dOperation.Progress.Status)
                {
                    case BackgroundTransferStatus.Running:
                        //Update your progress here
                        progression.Report(_progress);
                        break;
                    case BackgroundTransferStatus.Error:
                        //An error occured while downloading
                        progression.Report(-100);
                        break;
                    case BackgroundTransferStatus.PausedNoNetwork:
                        //No network detected
                        progression.Report(-50);
                        break;
                    case BackgroundTransferStatus.PausedCostedNetwork:
                        //Download paused because of metered connection
                        break;
                    case BackgroundTransferStatus.PausedByApplication:
                        //Download paused
                        break;
                    case BackgroundTransferStatus.Canceled:
                        //Download canceled
                        progression.Report(-100);
                        break;
                }
                if (_progress >= 100)
                {
                    //Download Done
                    dOperation = null;
                }
            });


            await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
        }
    }
}
