using Aspose.Svg;
using Aspose.Svg.Dom.Css;
using ImageMagick;
using RestSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using FS.Commons;

namespace FS.Utility;

public static class Helpers
{
    public static string CDNDirectory { get; set; }
    public static string CDNUrl { get; set; }
    public static string Sha256Hash(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        var Sb = new StringBuilder();
        using (var hash = SHA256.Create())
        {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (var b in result)
                Sb.Append(b.ToString("x2"));
        }
        return Sb.ToString();
    }
    public static string SignSHA256(string message, string key)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(key);
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            string hex = BitConverter.ToString(hashmessage);
            hex = hex.Replace("-", "").ToLower();
            return hex;
        }
    }
    public static int TimestampUnix(DateTime? dateTime = null)
    {
        if (dateTime == null) dateTime = DateTime.Now;
        var dateTimeUtc = TimeZoneInfo.ConvertTimeToUtc((DateTime)dateTime);
        return (int)(dateTimeUtc.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
    public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
    public static DateTime UnixTimeStampToDateTime(long unixTimeStamp, bool milisecond = true)
    {
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        if (milisecond)
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
        else
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
    static string GetMd5Hash(MD5 md5Hash, string input)
    {

        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
    public static string Sha1Encryption(string input, string key)
    {
        var encoding = new UTF8Encoding();
        var keyByte = encoding.GetBytes(key);
        var messageBytes = encoding.GetBytes(input);
        using (var hmacsha1 = new HMACSHA1(keyByte, false))
        {
            return BitConverter.ToString(hmacsha1.ComputeHash(messageBytes)).Replace("-", "").ToLower();
        }
    }
    public static string Sha256Encryption(string input, string key)
    {
        var encoding = new UTF8Encoding();
        var keyByte = encoding.GetBytes(key);
        var messageBytes = encoding.GetBytes(input);
        using (var hmacsha1 = new HMACSHA256(keyByte))
        {
            return BitConverter.ToString(hmacsha1.ComputeHash(messageBytes)).Replace("-", "").ToLower();
        }
    }
    public static MatchCollection MatchPhone(string input)
    {
        const string matchPhone = "(086|096|097|098|032|033|034|035|036|037|038|039|088|091|0fenum94|083|084|085|081|082|089|090|093|070|079|077|076|078|092|056|058|099|059)(\\d{7})";
        var regex = new Regex(matchPhone, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var matchs = regex.Matches(input);
        return matchs;
    }
    public static MatchCollection MatchEmail(string input)
    {
        const string matchEmail = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
        var regex = new Regex(matchEmail, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var matchs = regex.Matches(input);
        return matchs;
    }
    public static string UppercaseCharFirst(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            input = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
        return input;
    }

    /// <summary>
    /// encode base 64
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// decode base 64
    /// </summary>
    /// <param name="base64EncodedData"></param>
    /// <returns></returns>
    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    /// <summary>
    /// generate số điện thoại theo mã vùng quốc gia
    /// </summary>
    public static string GeneratePhoneNumberCountry(string codePhoneCountry, string phoneNumber)
    {
        return codePhoneCountry + phoneNumber;
    }

    /// <summary>
    /// kiểm tra có phải là số
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNumber(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(value);
        }
        return false;
    }

    /// <summary>
    /// kiểm tra email  hợp lệ
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool IsValidEmail(string email)
    {
        try
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isEmail;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// format email
    /// </summary>
    /// <param name="email"></param>
    public static void FormatEmail(ref string email)
    {
        if (email.EndsWith("@gmail.com"))
        {
            string first = email.Split('@')[0];
            first = first.Replace(".", "");
            email = first + "@gmail.com";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ReplaceMultiSpace(string input, string newStr = " ")
    {
        const RegexOptions options = RegexOptions.None;
        var regex = new Regex("[ ]{2,}", options);
        return regex.Replace(input, newStr);
    }

    /// <summary>
    /// Formats the currency.
    /// </summary>
    /// <param name="currency">The currency.</param>
    /// <returns></returns>
    public static string FormatCurrency(this decimal currency, bool extends = true)
    {
        CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
        var stringReturn = currency.ToString("#,###", cul.NumberFormat);
        if (extends) stringReturn += "đ";
        return stringReturn;
    }

    /// <summary>
    /// lấy số lượng từ cuối cùng của chuỗi
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetLastWord(string input, int numberWord = 2)
    {
        var output = string.Empty;
        if (!string.IsNullOrEmpty(input))
        {
            input = ReplaceMultiSpace(input);
            var arr = input.Split(' ');
            if (arr != null && arr.Length > 0)
            {
                Array.Reverse(arr);
                //if (arr.Length == 1)
                //   output = arr[0];
                //else
                //   output = $"{arr[arr.Length - 2]} {arr[arr.Length - 1]}";

                var resultWord = new List<string>();
                if (arr.Length >= numberWord)
                    for (var i = 0; i < numberWord; i++)
                        resultWord.Add(arr[i]);
                else
                    for (var i = 0; i < arr.Length; i++)
                        resultWord.Add(arr[i]);
                if (resultWord.Count > 0)
                {
                    resultWord.Reverse();
                    output = String.Join(" ", resultWord);
                }
            }
        }
        return output;
    }

    /// <summary>
    /// validate số điện thoại
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public static string ValidatePhoneNumber(string phoneNumber)
    {
        if (!string.IsNullOrEmpty(phoneNumber))
        {
            if (!IsNumber(phoneNumber)) return "Số điện thoại phải là dạng số";
            var phone = long.Parse(phoneNumber);
            if (phone == 0) return "Số điện thoại không hợp lệ!";
            if (phoneNumber.Trim().Length < 9 || phoneNumber.Trim().Length > 11) return "Số điện thoại gồm 9 hoặc 11 số";
        }
        return string.Empty;
    }

    /// <summary>
    /// encrypt text with key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string Encrypt(string key, string plainText)
    {
        try
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// decrypt text with key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string Decrypt(string key, string cipherText)
    {
        try
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// get description from enum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string GetDescriptionEnum<T>(this T enumValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            return null;
        var description = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        if (fieldInfo != null)
        {
            var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                description = ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        return description;
    }

    /// <summary>
    /// chạy bat file
    /// </summary>
    /// <param name="wrootFullPath"></param>
    /// <param name="wrootWorkingPath"></param>
    /// <returns></returns>
    public static bool RunBatFile(string wrootFullPath, string wrootWorkingPath)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo(wrootFullPath)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            WorkingDirectory = wrootWorkingPath,
        };

        Process process = new Process
        {
            StartInfo = processStartInfo
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode == -1)
            throw new Exception(process.StandardOutput.ReadToEnd());
        return true;
    }

    /// <summary>
    /// tạo file
    /// </summary>
    /// <param name="context"></param>
    /// <param name="wrootFullPath"></param>
    /// <returns></returns>
    public static bool CreateFile(string context, string wrootFullPath)
    {
        StreamWriter writer = new StreamWriter($"{wrootFullPath}");
        writer.WriteLine(context);
        writer.Close();
        return true;
    }

    /// <summary>
    /// đọc file
    /// </summary>
    /// <param name="pathFile"></param>
    /// <returns></returns>
    public static string ReadFile(string pathFile)
    {
        using (StreamReader reader = File.OpenText(pathFile))
        {
            string s;
            while ((s = reader.ReadLine()) != null)
            {
                return s;
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// base64 to image
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static Image Base64ToImage(string base64String)
    {
        // Convert base 64 string to byte[]
        byte[] imageBytes = Convert.FromBase64String(base64String);
        // Convert byte[] to Image
        using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
        {
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }

    public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
    {
        using (var ms = new MemoryStream())
        {
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// url to image
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static Image UrlToImage(string url)
    {
        using (WebClient webClient = new WebClient())
        {
            byte[] imageBytes = webClient.DownloadData(url);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }
    }

    public static Image UrlCDNToImage(string url)
    {
        var uri = new Uri(url);
        var absolute = uri.AbsolutePath;
        var absoluteParams = absolute.Split('/');
        var cdnDirectory = Helpers.CDNDirectory;
        var path = Path.Combine(cdnDirectory, "wwwroot", Helpers.PathCombine(absoluteParams));

        var stream = new MemoryStream();
        using (var magickImage = new MagickImage(path))
        {
            magickImage.AutoOrient();
            magickImage.Write(stream);
            Image image = Image.FromStream(stream);
            return image;
        }
    }

    public static string UrlCDNToLocalPath(string url)
    {
        var uri = new Uri(url);
        var absolute = uri.AbsolutePath;
        var absoluteParams = absolute.Split('/');
        var cdnDirectory = Helpers.CDNDirectory;
        var path = Path.Combine(cdnDirectory, "wwwroot", Helpers.PathCombine(absoluteParams));
        return path;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string GetFullDateName(DateTime date)
    {
        var result = string.Empty;
        if (date != null)
        {
            var culture = new System.Globalization.CultureInfo("vi-vn");
            var dayName = culture.DateTimeFormat.GetDayName(date.DayOfWeek);
            var day = date.Day;
            var full = date.ToString(Constants.FormatDateTime);
            result = $"{dayName}, {full}";
        }
        return result;
    }

    /// <summary>
    /// get full address
    /// </summary>
    /// <param name="provinceName"></param>
    /// <param name="districtName"></param>
    /// <param name="wardName"></param>
    /// <param name="street"></param>
    /// <returns></returns>
    public static string GetFullAddress(string provinceName, string districtName, string wardName, string street)
    {
        var fullAddress = string.Empty;
        var lst = new List<string>();
        if (!string.IsNullOrEmpty(street)) lst.Add(street);
        if (!string.IsNullOrEmpty(wardName)) lst.Add(wardName);
        if (!string.IsNullOrEmpty(districtName)) lst.Add(districtName);
        if (!string.IsNullOrEmpty(provinceName)) lst.Add(provinceName);
        if (lst.Count > 0) fullAddress = string.Join(", ", lst);
        return fullAddress;
    }

    /// <summary>
    /// strip html
    /// </summary>
    /// <param name="htmlString"></param>
    /// <returns></returns>
    public static string StripHTML(string htmlString)
    {
        if (!string.IsNullOrEmpty(htmlString))
            return Regex.Replace(htmlString, "<.*?>", string.Empty);
        return string.Empty;
    }

    public static string ReplaceHTML(string htmlString)
    {
        if (!string.IsNullOrEmpty(htmlString))
            return Regex.Replace(htmlString, "\n", "<br/>");
        return string.Empty;
    }

    public static string ReplaceUnwantJson(string htmlString)
    {
        if (string.IsNullOrEmpty(htmlString)) return string.Empty;
        var context = htmlString.Replace("\"", "");
        context = context.Replace("\n", "");
        return context;
    }

    /// <summary>
    /// Lấy ngày thứ 2 của tuần hiện tại, thời gian lúc 0h00
    /// </summary>
    public static DateTime GetMondayOfWeek(DateTime currentTime)
    {
        DateTime monday = currentTime;
        switch (currentTime.DayOfWeek)
        {
            case DayOfWeek.Monday:
                break;
            case DayOfWeek.Tuesday:
                monday = currentTime.AddDays(-1);
                break;
            case DayOfWeek.Wednesday:
                monday = currentTime.AddDays(-2);
                break;
            case DayOfWeek.Thursday:
                monday = currentTime.AddDays(-3);
                break;
            case DayOfWeek.Friday:
                monday = currentTime.AddDays(-4);
                break;
            case DayOfWeek.Saturday:
                monday = currentTime.AddDays(-5);
                break;
            case DayOfWeek.Sunday:
                monday = currentTime.AddDays(-6);
                break;
        }
        return new DateTime(monday.Year, monday.Month, monday.Day, 0, 0, 0);
    }

    /// <summary>
    /// Lấy ngày chủ nhật của tuần
    /// </summary>
    /// <param name="currentTime"></param>
    /// <returns></returns>
    public static DateTime GetSundayOfWeek(DateTime currentTime)
    {
        DateTime sunday = currentTime;
        switch (currentTime.DayOfWeek)
        {
            case DayOfWeek.Monday:
                sunday = currentTime.AddDays(6);
                break;
            case DayOfWeek.Tuesday:
                sunday = currentTime.AddDays(5);
                break;
            case DayOfWeek.Wednesday:
                sunday = currentTime.AddDays(4);
                break;
            case DayOfWeek.Thursday:
                sunday = currentTime.AddDays(3);
                break;
            case DayOfWeek.Friday:
                sunday = currentTime.AddDays(2);
                break;
            case DayOfWeek.Saturday:
                sunday = currentTime.AddDays(1);
                break;
            case DayOfWeek.Sunday:
                break;
        }
        return new DateTime(sunday.Year, sunday.Month, sunday.Day, 23, 59, 59);
    }

    /// <summary>
    /// kiểm tra valid html color
    /// </summary>
    /// <param name="inputColor"></param>
    /// <returns></returns>
    public static bool CheckValidFormatHtmlColor(string inputColor)
    {
        //regex from http://stackoverflow.com/a/1636354/2343
        if (Regex.Match(inputColor, "^#(?:[0-9a-fA-F]{3}){1,2}$").Success)
            return true;

        var result = System.Drawing.Color.FromName(inputColor);
        return result.IsKnownColor;
    }

    /// <summary>
    /// download file from a api
    /// </summary>
    /// <param name="apiUrl"></param>
    /// <returns></returns>
    public static byte[] DownloadFile(string fileUrl)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        var client = new RestClient();
        var request = new RestRequest(fileUrl, Method.Get);
        return client.DownloadData(request);
    }

    /// <summary>
    /// Get random number
    /// </summary>
    /// <param name="length">The length of expected string.</param>
    /// <returns></returns>
    public static string GetRandomNumber(int length, int seedNumber = 0)
    {
        string digits = "0123456789";
        int digitLength = 10;
        seedNumber = seedNumber == 0 ? (int)DateTime.Now.Ticks : seedNumber;
        StringBuilder stringBuilder = new StringBuilder();
        Random random = new Random(seedNumber);
        for (int i = 0; i < length; i++)
        {
            int idx = random.Next(0, digitLength - 1);
            stringBuilder.Append(digits[idx]);
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// crop image theo selection
    /// </summary>
    /// <param name="source"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Bitmap CropImage(Bitmap source, Rectangle rect)
    {
        var bitmap = new Bitmap(rect.Width, rect.Height);
        bitmap.SetResolution(source.VerticalResolution, source.HorizontalResolution);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.DrawImage(source, 0, 0, rect, GraphicsUnit.Pixel);
            return bitmap;
        }
    }

    /// <summary>
    /// lấy random code
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string GetUniqueKeyOriginal_BIASED(int size)
    {
        char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        byte[] data = new byte[size];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return result.ToString();
    }

    /// <summary>
    /// Get editable phone number
    /// </summary>
    public static string GetEditablePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber)) return string.Empty;
        if (phoneNumber.StartsWith("+84"))
            phoneNumber = phoneNumber.Substring(3, phoneNumber.Length - 3);
        if (phoneNumber.StartsWith("0"))
            return phoneNumber;
        else return $"0{phoneNumber}";
    }

    /// <summary>
    /// Correcting the phonenumber before save to db.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public static string CorrectVietnamesePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return string.Empty;
        if (phoneNumber.StartsWith("+84"))
            phoneNumber = phoneNumber.Substring(3, phoneNumber.Length - 3);
        phoneNumber = phoneNumber.TrimStart('0');
        return $"+84{phoneNumber}";
    }

    /// validate vietnamese phone number
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public static bool ValidateVietnamesePhoneNumber(string phoneNumber)
    {
        var regex = new Regex(@"^(((\+84)?|0)[3|5|7|8|9])+([0-9]{8})\b|(((\+84)?|0)[2]+([0-9]{9})\b)$");
        var match = regex.IsMatch(phoneNumber);

        if (!match)
        {
            return false;
        }

        return true;
    }

    public static string GetFriendlyUrl(string friendlyUrl, long setId, long variantId)
    {
        if (string.IsNullOrEmpty(friendlyUrl) || setId == 0 || variantId == 0) return string.Empty;
        return $"{friendlyUrl}-i.{setId}.{variantId}";
    }

    public static string GetCollectionUrl(long collectionId)
    {
        return $"collection/{collectionId}";
    }

    public static string GenerateRandomString(int length)
    {
        string pattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        int size = pattern.Length;
        Random random = new Random();
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(pattern[random.Next(0, size - 1)]);
        }
        return stringBuilder.ToString();
    }

    public static string GetEnumDescription(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
    }

    /// <summary>
    /// This is used to covert to an URL
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static string UrlCombine(params string[] paths)
    {
        var trimmedPaths = paths.Where(x => !string.IsNullOrEmpty(x))
                                .Select(x => x.Trim('/'))
                                .ToArray();
        return "/" + string.Join("/", trimmedPaths);
    }


    /// <summary>
    /// This is used to convert to a directory
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static string PathCombine(params string[] paths)
    {
        return Path.Combine(paths.Where(x => !string.IsNullOrEmpty(x)).ToArray());
    }

    /// <summary>
    /// This is used to convert a string to camelCase or PascalCase
    /// </summary>
    /// <param name="input"></param>
    /// <param name="isCamelCase"></param>
    /// <returns></returns>
    public static string ConvertToPascalOrCamelCase(string input, bool isCamelCase = false)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string cleanedInput = Regex.Replace(input, @"[^a-zA-Z0-9]", " ");
        cleanedInput = Regex.Replace(cleanedInput, @"^\d+", "");

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        string pascalCase = textInfo.ToTitleCase(cleanedInput.ToLower()).Replace(" ", "");

        if (isCamelCase && pascalCase.Length > 0)
        {
            return char.ToLower(pascalCase[0]) + pascalCase.Substring(1);
        }

        return pascalCase;
    }

    public static Bitmap SVGToImage(string url, int width = 0, int height = 0, int density = 0)
    {
        var uri = new Uri(url);
        var absolute = uri.AbsolutePath;
        var absoluteParams = absolute.Split('/');
        var path = !string.IsNullOrEmpty(CDNDirectory) ? Path.Combine(CDNDirectory, "wwwroot", Helpers.PathCombine(absoluteParams)) : null;
        byte[] bytes = null;
        //nếu path không tồn tại, thì download file svg về
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            using (WebClient webClient = new WebClient())
            {
                bytes = webClient.DownloadData(url);
            }
        }
        else
        {
            bytes = File.ReadAllBytes(path);
        }

        using (MagickImage image = new MagickImage())
        {
            MagickReadSettings readSettings = new MagickReadSettings
            {
                Format = MagickFormat.Svg,
                BackgroundColor = MagickColors.Transparent,
                Density = new Density(600, 600)
            };
            if (width > 0) readSettings.Width = (uint)width;
            if (height > 0) readSettings.Height = (uint)height;
            if (density > 0) readSettings.Density = new Density(density, density);
            image.Read(bytes, readSettings);
            image.Format = MagickFormat.Png;
            var memoryStream = new MemoryStream();
            image.Write(memoryStream);
            var bitmap = new Bitmap(memoryStream);
            return bitmap;
        }
    }

    public static void FromXmlString(this RSA rsa, string xmlString)
    {
        RSAParameters parameters = new RSAParameters();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);

        if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
        {
            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                switch (node.Name)
                {
                    case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                }
            }
        }
        else
        {
            throw new Exception("Invalid XML RSA key.");
        }

        rsa.ImportParameters(parameters);
    }

    public static List<CSSModel> ScanSVG(string url)
    {
        List<CSSModel> cSSModels = new List<CSSModel>();
        using (var document = new SVGDocument(url))
        {
            //từ style sheets, lấy ra class name và color
            var styleSheets = document.StyleSheets;
            foreach (var style in styleSheets)
            {
                var cssRule = style.CSSRules;
                foreach (ICSSStyleRule rule in cssRule)
                {
                    var className = rule.SelectorText;
                    var cssText = rule.Style.CSSText;

                    //extract fill và stroke color từ cssText
                    var colorModel = GetFillFromCSSText(cssText);
                    if (colorModel != null)
                    {
                        var splitClassName = className.Split(',')?.ToList();
                        if (splitClassName.Any())
                        {
                            foreach (var cls in splitClassName)
                            {
                                cSSModels.Add(new CSSModel
                                {
                                    ClassName = cls?.Replace(".", ""),
                                    Fill = colorModel.Fill,
                                    Stroke = colorModel.Stroke
                                });
                            }
                        }
                    }
                }
            }
        }
        return cSSModels;
    }

    static Color? ParseRgbText(string rgbText)
    {
        rgbText = rgbText.Replace(" ", "");
        Regex regex = new Regex(@"rgb\((\d+),\s*(\d+),\s*(\d+)\)");
        Match match = regex.Match(rgbText);

        if (match.Success)
        {
            int red = int.Parse(match.Groups[1].Value);
            int green = int.Parse(match.Groups[2].Value);
            int blue = int.Parse(match.Groups[3].Value);

            return Color.FromArgb(red, green, blue);
        }
        else
        {
            return null;
        }
    }

    static string ColorToHex(Color color)
    {
        string hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        return hex.ToLower();
    }

    static CSSModel GetFillFromCSSText(string cssText)
    {
        cssText = cssText.Replace(" ", "");

        Regex fillRegex = new Regex(@"fill:(.*?);");
        Regex strokeRegex = new Regex(@"stroke:(.*?);");

        Match fillMatch = fillRegex.Match(cssText);
        Match strokeMatch = strokeRegex.Match(cssText);

        CSSModel model = new CSSModel();

        if (fillMatch.Success)
        {
            var color = fillMatch.Groups[1].Value;
            var convertColor = ParseRgbText(color);
            if (convertColor != null)
            {
                var hex = ColorToHex(convertColor.Value);
                model.Fill = hex;
            }
        }

        if (strokeMatch.Success)
        {
            var color = strokeMatch.Groups[1].Value;
            var convertColor = ParseRgbText(color);
            if (convertColor != null)
            {
                var hex = ColorToHex(convertColor.Value);
                model.Stroke = hex;
            }
        }

        return !string.IsNullOrEmpty(model.Fill) || !string.IsNullOrEmpty(model.Stroke) ? model : null;



    }
}

public class CSSModel
{
    public string ClassName { get; set; }
    public string Fill { get; set; }
    public string Stroke { get; set; }
}
