using Microsoft.WindowsAzure.Storage.Blob;
using ProStickers.ViewModel.Core;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProStickers.CloudDAL.Storage
{
    public class BlobStorage
    {
        public async static Task UploadPublicImage(string containerName, byte[] newImage, string guid, string contentType)
        {
            CloudBlobContainer container = Utility.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(guid);
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            await container.SetPermissionsAsync(permissions);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromByteArrayAsync(newImage, 0, newImage.Length);
        }

        public async static Task UploadPrivateImage(string containerName, byte[] newImage, string guid, string contentType)
        {
            CloudBlobContainer container = Utility.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(guid);
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();
            await container.SetPermissionsAsync(permissions);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromByteArrayAsync(newImage, 0, newImage.Length);
        }

        public static string DownloadBlobUri(string containerName, string blobName)
        {
            // Get Blob Container
            var container = Utility.GetBlobContainer(containerName);

            // Get reference to blob (binary content)
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            return blockBlob.Uri.ToString();
        }

        public async static Task DeleteBlob(string containerName, string blobName)
        {
            var container = Utility.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            await blockBlob.DeleteIfExistsAsync();
        }

        public async static Task UploadFile(string containerName, byte[] newImage, string guid, string contentType)
        {
            CloudBlobContainer container = Utility.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(guid);
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();
            await container.SetPermissionsAsync(permissions);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromByteArrayAsync(newImage, 0, newImage.Length);
        }

        public static long GetFileSize(string containerName, string blobName)
        {
            var container = Utility.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            return blockBlob.Properties.Length;
        }

        public static DownloadImageViewModel DownloadBlobByteArray(string containerName, string blobName)
        {
            var container = Utility.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            if (blockBlob.Exists() == true)
            {
                DownloadImageViewModel vm = new DownloadImageViewModel();
                using (MemoryStream ms = new MemoryStream())
                {
                    //Read content
                    blockBlob.DownloadToStream(ms);
                    vm.ImageBuffer = ms.ToArray();
                    vm.FileExtension = blockBlob.Properties.ContentType;
                    return vm;
                }
            }
            else
            {
                return null;
            }
        }
      
        public static double GetFileOrImageSize(string containerName, string blobName)
        {
            var container = Utility.GetBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            using (MemoryStream ms = new MemoryStream())
            {
                blockBlob.DownloadToStream(ms);
                byte[] block = ms.ToArray();
                return block.Length / 1024f;
            }
        }
    }
}
