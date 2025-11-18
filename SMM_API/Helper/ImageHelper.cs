namespace SMM_API.Helper
{
    public class ImageHelper
    {
        public static string SaveImageToFile(IFormFile imageFile, string directoryPath)
        {
            string directory = $"wwwroot/{directoryPath}";
            if (imageFile == null || imageFile.Length == 0)
                return null;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string fileExtension = Path.GetExtension(imageFile.FileName);
            if (string.IsNullOrWhiteSpace(fileExtension))
                fileExtension = ".jpeg";

            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string fullPath = $"{directoryPath}/{fileName}";
            string fullPathToWrite = $"{directory}/{fileName}";

            using (var stream = new FileStream(fullPathToWrite, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return fullPath;
        }

        public static byte[] ReadFileBytes(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return null;

            return File.ReadAllBytes(filePath);
        }

        public static string DeleteFileFromUrl(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return "File URL is required.";

            var uri = new Uri(fileUrl);
            var fileName = Path.GetFileName(uri.LocalPath);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

            if (!System.IO.File.Exists(filePath))
                return "File not found.";

            System.IO.File.Delete(filePath);
            return "File deleted successfully.";
        }
    }
}
