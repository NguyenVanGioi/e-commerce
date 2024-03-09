using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shopping.Helper
{
    public static class Utilities
    {
        public static void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string ToTitleCase(string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for (int index = 0; index < words.Length; index++)
                {
                    var s = words[index];
                    if (s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }

        private static Random random = new Random();

        public static string GetRandomKey(int length = 5)
        {
            const string pattern = "0123456789zxcvbnmmkjpoiuy[]{}:~!@^&*()+";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[random.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }

        
        public static string SEOUrl(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàảạãâấầẩậẫăắằẳặẵ]", "a");
            url = Regex.Replace(url, @"[éèẻẹẽêếềểệễ]", "e");
            url = Regex.Replace(url, @"[óòỏõọôốồổỗộơớờởỡợ]", "o");
            url = Regex.Replace(url, @"[úùủũụưứừửữự]", "u");
            url = Regex.Replace(url, @"[íìỉĩị]", "i");
            url = Regex.Replace(url, @"[đ]", "d");
            url = Regex.Replace(url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            url = Regex.Replace(url.Trim(), @"\s+", "-");
            url = Regex.Replace(url, @"\s", "-");
            while (true)
            {
                if (url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            return url;
        }


        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname)
        {
            try
            {
                if (file == null)
                {
                    return null; // Không có file mới được chọn
                }

                if (newname == null) newname = file.FileName;

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                CreateIfMissing(path);

                string pathFile = Path.Combine(path, newname);

                var supportedTypes = new[] { "jpg", "jfif", "webp", "png", "jpeg", "PNG", "JPG", "JPEG", "gif" };
                var fileExt = Path.GetExtension(file.FileName).Substring(1);

                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    return null;
                }
                else
                {
                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
