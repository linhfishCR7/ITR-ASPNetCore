//using Amazon.S3;
//using Amazon.S3.Model;
//using Microsoft.AspNetCore.Mvc;
//using SportsStore.Models;

//namespace SportsStore.Services
//{
//    public class S3
//    {

//        private readonly IAmazonS3 _s3Client;
//        private StoreDbContext _dbContext;


//        public S3(IAmazonS3 s3Client, StoreDbContext ctx)
//        {
//            _s3Client = s3Client;
//            _dbContext = ctx;
//        }


//        [HttpPost]
//        public async Task<IActionResult> UploadFileAsync(IFormFile file, string? bucketName, string? prefix, long? productId)


//        {
//            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);


//            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
//            var request = new PutObjectRequest()
//            {
//                BucketName = bucketName,
//                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
//                InputStream = file.OpenReadStream()
//            };
//            request.Metadata.Add("Content-Type", file.ContentType);
//            await _s3Client.PutObjectAsync(request);

//            //Save file details into the database
//            //var product = new Product
//            //{
//            //    Image = file.FileName,
//            //    PresignedUrl = _s3Client.GetPreSignedURL(urlRequest),
//            //};
//            ////save object model
//            //_dbContext.Products.Add(product);
//            //await _dbContext.SaveChangesAsync();
//            // Find the existing product in the database by its ID

//            //return Ok(product);
//            var existingProduct = await _dbContext.Products.FindAsync(productId);

//            if (existingProduct != null)
//            {
//                // Update the product properties
//                existingProduct.Image = file.FileName;
//                existingProduct.PresignedUrl = $"https://{bucketName}.s3.amazonaws.com/{prefix}/{file.FileName}";

//                // Save the changes to the database
//                await _dbContext.SaveChangesAsync();

//                // Return the updated product
//                return Ok(existingProduct);
//            }
//            else
//            {
//                // Product not found
//                return NotFound();
//            }

//            //var uploadResponse = new
//            //{
//            //        key = $"{prefix}/{file.FileName}"
//            //};
//            //return Ok(uploadResponse);
//        }
//    }
//}

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Operations;
using SportsStore.Models;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SportsStore.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly StoreDbContext _dbContext;

        public FileUploadService(IAmazonS3 s3Client, StoreDbContext dbContext)
        {
            _s3Client = s3Client;
            _dbContext = dbContext;
        }
        public class MyClass
        {
            public string? MyString { get; set; }
        }

        public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, long? productId)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists)
            {
                MyClass myObject = new MyClass();
                myObject.MyString = $"Bucket {bucketName} does not exist.";

                return (IActionResult)myObject;

            }

            // Get the file extension
            var extension = Path.GetExtension(file.FileName).ToLower();

            // Determine the file type based on the extension
            var fileType = GetFileType(extension);

            if (fileType == FileType.Unknown)
            {
                MyClass myObject = new MyClass();
                myObject.MyString = "Invalid file type.";

                return (IActionResult)myObject;
            }

            var key = GenerateUniqueKey(file.FileName);

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(request);

            //if (productId.HasValue)
            //{
            //    var existingProduct = await _dbContext.Products.FindAsync(productId);
            //    if (existingProduct != null)
            //    {
            //        existingProduct.Image = key;
            //        existingProduct.PresignedUrl = $"https://{bucketName}.s3.amazonaws.com/{fileType}/{key}";

            //        await _dbContext.SaveChangesAsync();

            //        return existingProduct;
            //    }
            //}

            string url = $"https://{bucketName}.s3.amazonaws.com/{fileType}/{key}";

            // return value
            MyClass Value = new MyClass();
            Value.MyString = url;

            return (IActionResult)Value;
        }

        private FileType GetFileType(string extension)
        {
            if (IsImageExtension(extension))
            {
                return FileType.Image;
            }
            else if (IsDocumentExtension(extension))
            {
                return FileType.Document;
            }
            else if (IsVideoExtension(extension))
            {
                return FileType.Video;
            }

            return FileType.Unknown;
        }

        private bool IsImageExtension(string extension)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            return allowedExtensions.Contains(extension);
        }

        private bool IsDocumentExtension(string extension)
        {
            string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".txt" };
            return allowedExtensions.Contains(extension);
        }

        private bool IsVideoExtension(string extension)
        {
            string[] allowedExtensions = { ".mp4", ".avi", ".mov", ".wmv" };
            return allowedExtensions.Contains(extension);
        }

        private string GenerateUniqueKey(string fileName)
        {
            var guid = Guid.NewGuid().ToString();
            var key = $"{guid}{fileName}";
            return key;
        }
    }

    public interface IFileUploadService
    {
        Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, long? productId);
    }

    public enum FileType
    {
        Unknown,
        Image,
        Document,
        Video
    }
}

