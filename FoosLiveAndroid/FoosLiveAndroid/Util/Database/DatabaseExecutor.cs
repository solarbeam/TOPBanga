using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace FoosLiveAndroid.Util
{
    public static class DatabaseExecutor
    {

        public static bool InsertIntoHistory(string blueTeamName, string redTeamName, int bluePoints, int redPoints)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://yu-gi-oh.lt/a.php");
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("InsertIntoHistory;");
            stringBuilder.Append(blueTeamName);
            stringBuilder.Append(";");
            stringBuilder.Append(redTeamName);
            stringBuilder.Append(";");
            stringBuilder.Append(bluePoints);
            stringBuilder.Append(";");
            stringBuilder.Append(redPoints);
            streamWriter.Write(stringBuilder.ToString());
            streamWriter.Flush();
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string s = streamReader.ReadLine();
            if(s != null)
                if (s.Equals("SUCCESS"))
                    return true;
            return false;
        }
    }
}