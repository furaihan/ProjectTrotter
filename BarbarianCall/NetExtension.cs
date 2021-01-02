using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Rage;

namespace BarbarianCall
{
    internal static class NetExtension
    {
        internal static void SendError(Exception e)
        {
            try
            {
                if (!IsInternetConnected()) return;
                var st = new StackTrace(e, true);
                string toSend = string.Empty;
                foreach (var frame in st.GetFrames())
                {
                    var filePath = frame.GetFileName();
                    var fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                    var lNumber = frame.GetFileLineNumber();
                    toSend += $"[{frame.GetMethod().Name}] " + fileName + " line: " + lNumber.ToString() + " ==> ";
                }
                Thread SendException = new Thread(async () =>
                {
                    using var httpClient = new HttpClient();
                    using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://maker.ifttt.com/trigger/logReport/with/key/cWTXitSTdZE0TAGgM6ZgEF")
                    {
                        Content = new StringContent($"{{\"value1\":\"{e.Message}\",\"value2\":\"{toSend}\",\"value3\":\"{Game.LocalPlayer.Name} - {e.Source}\"}}".Replace("\\", "\\\\"))
                    };
#if DEBUG
                    $"{{\"value1\":\"{e.Message}\",\"value2\":\"{toSend}\",\"value3\":\"{Game.LocalPlayer.Name} - {e.Source}\"}}".Replace("\\", "\\\\").ToLog();
#endif
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var resp = await httpClient.SendAsync(request);
                });
                SendException.Start();
                GameFiber.SleepUntil(() => SendException.ThreadState == System.Threading.ThreadState.Stopped, 5000);
            }
            catch (Exception exc)
            {
                Peralatan.ToLog("Fetch webhooks error");
                exc.ToString().ToLog();
            }
        }
        internal static bool IsInternetConnected()
        {
            try
            {
                using var client = new WebClient();
                using var res = client.OpenRead("http://google.com/generate_204");
                return true;
            }
            catch (Exception e)
            {
                "we have Internet problem".ToLog();
                Peralatan.ToLog(e.ToString());
                return false;
            }
        }
        internal static void CheckUpdate()
        {

        }
    }
}
