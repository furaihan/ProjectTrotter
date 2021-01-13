using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Net.NetworkInformation;
using Rage;

namespace BarbarianCall.Extensions
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
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (!IsInternetConnected()) return;
                    StackTrace st = new StackTrace(e, true);
                    string toSend = "";
                    foreach (StackFrame frame in st.GetFrames())
                    {
                        frames++;
                        string filePath = frame.GetFileName();
                        string fileName = filePath == null ? "NULL" : filePath.Substring(filePath.LastIndexOf('\\') + 1);
                        int lNumber = frame.GetFileLineNumber();
                        if (filePath == null && lNumber == 0) continue;
                        toSend += $"[{frame.GetMethod().Name}] " + fileName + " line: " + lNumber.ToString() + " ==> ";
                    }
                    Uri ifttt = new Uri("https://maker.ifttt.com/trigger/logReport/with/key/cWTXitSTdZE0TAGgM6ZgEF");
                    Thread SendException = new Thread(() =>
                    {
                        using HttpClient httpClient = new HttpClient();
                        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ifttt)
                        {
                            Content = new StringContent($"{{\"value1\":\"{e.GetType().Name} - {e.Message}\",\"value2\":\"{toSend}\",\"value3\":\"{Game.LocalPlayer.Name} - {e.Source}\"}}".Replace("\\", "\\\\"))
                        };
#if DEBUG
                        $"{{\"value1\":\"{e.GetType().Name} - {e.Message}\",\"value2\":\"{toSend}\",\"value3\":\"{Game.LocalPlayer.Name} - {e.Source}\"}}".Replace("\\", "\\\\").ToLog();
#endif
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        HttpResponseMessage resp = httpClient.SendAsync(request).Result;
                        string result = resp.Content.ReadAsStringAsync().Result;
                        result.ToLog();
                        $"Data collection takes {stopwatch.ElapsedMilliseconds:0.00} ms".ToLog();
                    });
                    SendException.Start();
                    GameFiber.SleepUntil(() => SendException.ThreadState == System.Threading.ThreadState.Stopped, 18000);
                }
                catch (Exception exc)
                {
                    $"Fetch webhooks error. Loop: {frames}".ToLog();
                    exc.ToString().ToLog();
                }
            });
        }
        internal static bool IsInternetConnected()
        {
            bool Success = false;
            Thread Pinger = new Thread(() =>
            {
                Ping ping = null;
                try
                {
                    string[] hosts = { "8.8.8.8", "1.1.1.1", "9.9.9.9", "208.67.220.220" }; //Google, CloudFlare, Quad9, OpenDNS
                    ping = new Ping();
                    foreach (string host in hosts)
                    {
                        if (!IPAddress.TryParse(host, out IPAddress IP)) continue;
                        try
                        {
                            PingReply pr = ping.SendPingAsync(IP, 850).Result;
                            if (pr.Status == IPStatus.Success)
                            {
                                Success = true;
                                $"Ping to {pr.Address} success. Time: {pr.RoundtripTime}".ToLog();
                                break;
                            }
                            else
                            {
                                $"Ping to {IP} failed. Error: {pr.Status}".ToLog();
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString().ToLog();
                }
                finally
                {
                    if (ping != null) ping.Dispose();
                }
            });
            Pinger.Start();
            GameFiber.SleepUntil(() => Pinger.ThreadState == System.Threading.ThreadState.Stopped, 6000);
            return Success;
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
                    GameFiber.SleepUntil(() => FetchUpdate.ThreadState == System.Threading.ThreadState.Stopped, 12000);
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
