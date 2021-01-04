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
            GameFiber.StartNew(() =>
            {
                int frames = 0;
                try
                {
                    if (!IsInternetConnected()) return;
                    var time = DateTime.Now;
                    var st = new StackTrace(e, true);
                    string toSend = "";
                    foreach (var frame in st.GetFrames())
                    {
                        frames++;
                        var filePath = frame.GetFileName();
                        var fileName = filePath == null ? "NULL" : filePath.Substring(filePath.LastIndexOf('\\') + 1);
                        var lNumber = frame.GetFileLineNumber();
                        if (filePath == null && lNumber == 0) continue;
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
                        var result = await resp.Content.ReadAsStringAsync();
                        result.ToLog();
                        $"Data collection takes {(DateTime.Now - time).TotalSeconds:0.00} seconds".ToLog();
                    });
                    SendException.Start();
                    GameFiber.SleepUntil(() => SendException.ThreadState == System.Threading.ThreadState.Stopped, 12000);
                }
                catch (Exception exc)
                {
                    Peralatan.ToLog($"Fetch webhooks error. Loop: {frames}");
                    exc.ToString().ToLog();
                }
            });
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
        internal static Version CurrentVersion;
        internal static void CheckUpdate()
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    if (!IsInternetConnected()) return;
                    Thread FetchUpdate = new Thread(() =>
                    {
                        Uri UpdateAPI = new Uri("");
                        WebClient Client = new WebClient();
                        string WebVersion = Client.DownloadString(UpdateAPI);
                        if (Version.TryParse(WebVersion, out Version version))
                        {
                            CurrentVersion = version;
                        }
                    });
                    FetchUpdate.Start();
                    GameFiber.SleepUntil(() => FetchUpdate.ThreadState == System.Threading.ThreadState.Stopped, 5000);
                }
                catch (WebException we)
                {
                    "We have some internet problem".ToLog();
                    we.ToString().ToLog();
                }
                catch (Exception e)
                {
                    "Check update Error".ToLog();
                    e.ToString().ToLog();
                }
            });
        }
    }
}
