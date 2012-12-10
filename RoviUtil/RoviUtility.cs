using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace RoviUtil
{
    /// <summary>
    /// Class with helper methods for Rovi API.
    /// Using code samples from Rovi site as basis.
    /// http://prod-doc.rovicorp.com/mashery/index.php/Authentication-Code-Examples#CSharp 
    /// </summary>
    public static class RoviUtility
    {
        public static string CalculateRoviSig(string apikey, string secret, DateTime? time = null)
        {
            DateTime currentTime = DateTime.UtcNow;
            if (time.HasValue)
            {
                currentTime = time.Value;
            }
 
            //get the timestamp value
            string timestamp = (currentTime - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString(CultureInfo.InvariantCulture);

            //grab just the integer portion
            timestamp = timestamp.Substring(0, timestamp.IndexOf(".", System.StringComparison.Ordinal));

            //call the function to create the hash
            string sig = CreateMD5Hash(apikey + secret + timestamp);

            return sig;
        }


        /// <summary>
        /// Create Uri string for request to Song API
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="secret"></param>
        /// <param name="track">Title of the song</param>
        /// <param name="trackid">Rovi Music ID for a popular song, consisting of the prefix MT followed by a ten-digit number. For example: MT0009472348. </param>
        /// <param name="isrcid">International Standard Recording Code (ISRC) for a song recording. </param>
        /// <param name="muzeid">Legacy ID from the Muze database. </param>
        /// <param name="amgpoptrackid">AMG ID for a track on a popular music album, consisting of a ten-character string that starts with T and is followed by 9 digits with leading spaces. </param>
        /// <param name="amgclassicaltrackid">AMG ID for a track on a classical music album, consisting of a ten-character string that starts with Y and is followed by 9 digits with leading spaces. </param>
        /// <param name="include">Other Song requests to include in the request. For multiple includes, separate the values with commas like this: include="appearances,review,moods,styles,themes".</param>
        /// <param name="country">Country of the language of the response. The current release of the API only supports US. Default value = "US"</param>
        /// <param name="language">Language of the response data. This request only supports en (English). Default value = "en"</param>
        /// <param name="format">Format of the returned data (json or xml). Default value = "json"</param>
        /// <returns></returns>
        public static string ComposeUriForRoviSongAPI(string apikey, string secret, 
            string track = null, 
            string trackid = null,
            string isrcid = null,
            string muzeid = null,
            string amgpoptrackid = null,
            string amgclassicaltrackid = null,
            string include = null,
            string country = "US",
            string language = "en",
            string format = "json",
            DateTime? time = null)
        {
            #region check parameters
            if (string.IsNullOrEmpty(apikey))
            {
                throw new ArgumentNullException("apikey");
            }
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("secret");
            }
            if (null == track &&
                null == trackid &&
                null == isrcid &&
                null == muzeid &&
                null == amgpoptrackid &&
                null == amgclassicaltrackid &&
                null == include)
            {
                throw new ArgumentNullException("All parameters cannot be null. You must supply at least 1 value");
            }
            #endregion

            var baseUri = "http://api.rovicorp.com/data/v1/song/info?";
            var sb = new StringBuilder(baseUri);
            sb.Append("apikey=").Append(apikey)
              .Append("&sig=").Append(CalculateRoviSig(apikey, secret, time))
              .Append("&country=").Append(country)
              .Append("&language=").Append(language)
              .Append("&format=").Append(format);

            if (null != track)
            {
                sb.Append("&track=").Append(track);
            }
            if (null != trackid)
            {
                sb.Append("&trackid=").Append(trackid);
            }
            if (null != isrcid)
            {
                sb.Append("&isrcid=").Append(isrcid);
            }
            if (null != muzeid)
            {
                sb.Append("&muzeid=").Append(muzeid);
            }
            if (null != amgpoptrackid)
            {
                sb.Append("&amgpoptrackid=").Append(amgpoptrackid);
            }
            if (null != amgclassicaltrackid)
            {
                sb.Append("&amgclassicaltrackid=").Append(amgclassicaltrackid);
            }
            if (null != include)
            {
                sb.Append("&include=").Append(include);
            }

            var result = sb.ToString();
            return result;
        }

        private static string CreateMD5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));  //this will use lowercase letters, use "X2" instead of "x2" to get uppercase
            }
            return sb.ToString();

        }
    }
}
