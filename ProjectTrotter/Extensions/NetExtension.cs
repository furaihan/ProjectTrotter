using System;
using System.Reflection;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Net.NetworkInformation;
using Rage;

namespace ProjectTrotter.Extensions
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
                    System.Text.StringBuilder @string = new();
                    StackTrace st = new(e, true);
                    string toSend = "";
                    foreach (StackFrame frame in st.GetFrames())
                    {
                        frames++;
                        string filePath = frame.GetFileName();
                        string fileName = filePath == null ? "NULL" : filePath.Substring(filePath.LastIndexOf('\\') + 1);
                        int lNumber = frame.GetFileLineNumber();
                        if (filePath == null && lNumber == 0) continue;
                        @string.Append($"[{frame.GetMethod().Name}] " + fileName + " line: " + lNumber.ToString() + " ==> ");
                    }
                    toSend = @string.ToString();
                    Uri ifttt = new("https://maker.ifttt.com/trigger/logReport/with/key/cWTXitSTdZE0TAGgM6ZgEF");
                    Thread SendException = new(() =>
                    {
                        using HttpClient httpClient = new();
                        using HttpRequestMessage request = new(HttpMethod.Post, ifttt)
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
                catch (ThreadAbortException tae)
                {
                    Game.LogTrivial(tae.Message + " from " + tae.Source);
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
            Thread Pinger = new(() =>
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
                catch (ThreadAbortException tae)
                {
                    Game.LogTrivial(tae.Message + " from " + tae.Source);
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
            try
            {
                Pinger.Start();
            }
            catch (ThreadAbortException tae)
            {
                Game.LogTrivial(tae.Message + " from " + tae.Source);
            }         
            catch (Exception e)
            {
                e.ToString().ToLog();
            }
            GameFiber.SleepUntil(() => Pinger.ThreadState == System.Threading.ThreadState.Stopped, 6000);
            return Success;
        }
        internal static Version CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        internal static void CheckUpdate()
        {
            GameFiber.StartNew(() =>
            {
                try
                {
                    if (!IsInternetConnected()) return;
                    Thread FetchUpdate = new(() =>
                    {
                        Uri UpdateAPI = new("");
                        WebClient Client = new();
                        string WebVersion = Client.DownloadString(UpdateAPI);
                        if (Version.TryParse(WebVersion, out Version version))
                        {
                            CurrentVersion = version;
                        }
                        Client.Dispose();
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
