﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Goman_Plugin.Model;
using Goman_Plugin.Modules.Authentication;

namespace Goman_Plugin.Helpers
{
    internal class GomanHttpHelper
    {
        private static HttpClient HttpClient { get; set; }

        public class AccountFeeder
        {
            private static readonly Uri AccountUri = new Uri("https://goman.io/api/accounts/");

            private static HttpRequestMessage AccountRequestMessage(HttpContent httpContent)
            {
                return new HttpRequestMessage(HttpMethod.Post, AccountUri)
                {
                    Content = httpContent
                };
            }

            public static async Task<MethodResult<string>> SendAccounts(HttpContent httpContent)
            {
                var methodResults = await Authentication.Login();
                if (!methodResults.Success) return methodResults;

                try
                {
                    using (var httpRequestMessage = AccountRequestMessage(httpContent))
                    using (var httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            methodResults.Data = await httpResponseMessage.Content.ReadAsStringAsync();
                            methodResults.Success = true;
                            //using (var sw = new StreamWriter("accounts.txt"))
                            //{
                            //    await sw.WriteLineAsync(methodResults.Data);
                            //}
                        }
                        else
                        {
                            methodResults.Error =
                                new Exception(
                                    $"{(int) httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}");
                            HttpClient = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    methodResults.Error = ex;
                    HttpClient = null;
                }

                return methodResults;
            }
        }

        public class Authentication
        {
            private static readonly Uri LoginUri = new Uri("https://goman.io/api/login/");
            private static readonly Uri PingUri = new Uri("https://goman.io/api/ping/");
            private static readonly Uri LogoutUri = new Uri("https://goman.io/api/logout/");

            public static async Task<MethodResult<string>> Login()
            {
                if (HttpClient != null && AuthenticationSettings.LoggedIn)
                    return new MethodResult<string> {Success = true};

                AuthenticationSettings.LoggedIn = false;
                HttpClient = new HttpClient();
                var methodResults = new MethodResult<string>();

                try
                {
                    using (var httpRequestMessage = LoginRequestMessage())
                    using (var httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            methodResults.Data = await httpResponseMessage.Content.ReadAsStringAsync();
                            methodResults.Success = true;
                            AuthenticationSettings.LoggedIn = true;
                            //using (var sw = new StreamWriter("login.txt"))
                            //{
                            //    await sw.WriteLineAsync(methodResults.Data);
                            //}
                        }
                        else
                        {
                            methodResults.Error =
                                new Exception(
                                    $"{(int) httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}");
                            HttpClient = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    methodResults.Error = ex;
                    HttpClient = null;
                }

                return methodResults;
            }

            private static HttpRequestMessage LoginRequestMessage()
            {
                return new HttpRequestMessage(HttpMethod.Post, LoginUri)
                {
                    Content = LoginHttpContent()
                };
            }

            private static FormUrlEncodedContent LoginHttpContent()
            {
                return new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("u", AuthenticationSettings.Username),
                    new KeyValuePair<string, string>("p", AuthenticationSettings.Password)
                });
            }

            public static async Task<MethodResult<string>> Ping()
            {
                var methodResults = await Login();
                if (!methodResults.Success) return methodResults;

                try
                {
                    using (var httpRequestMessage = PingRequestMessage())
                    using (var httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            methodResults.Data = await httpResponseMessage.Content.ReadAsStringAsync();
                            methodResults.Success = true;
                            //using (var sw = new StreamWriter("ping.txt"))
                            //{
                            //    await sw.WriteLineAsync(methodResults.Data);
                            //}
                        }
                        else
                        {
                            methodResults.Error =
                                new Exception(
                                    $"{(int) httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}");
                            HttpClient = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    methodResults.Error = ex;
                    HttpClient = null;
                }

                return methodResults;
            }

            private static HttpRequestMessage PingRequestMessage()
            {
                return new HttpRequestMessage(HttpMethod.Post, PingUri)
                {
                    Content = HwidHttpContent()
                };
            }

            public static async Task<MethodResult<string>> Logout()
            {
                var methodResults = await Login();
                if (!methodResults.Success) return methodResults;


                try
                {
                    using (var httpRequestMessage = LogoutRequestMessage())
                    using (var httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            methodResults.Data = await httpResponseMessage.Content.ReadAsStringAsync();
                            methodResults.Success = true;
                            //using (var sw = new StreamWriter("logout.txt"))
                            //{
                            //    await sw.WriteLineAsync(methodResults.Data);
                            //}
                        }
                        else
                        {
                            methodResults.Error =
                                new Exception(
                                    $"{(int) httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}");
                            HttpClient = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    methodResults.Error = ex;
                    HttpClient = null;
                }
                finally
                {
                    AuthenticationSettings.LoggedIn = false;
                    HttpClient = null;
                }

                return methodResults;
            }

            private static HttpRequestMessage LogoutRequestMessage()
            {
                return new HttpRequestMessage(HttpMethod.Post, LogoutUri)
                {
                    Content = HwidHttpContent()
                };
            }

            private static FormUrlEncodedContent HwidHttpContent()
            {
                return new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("hwid", Hwid.FingerPrint)
                });
            }
        }

        public class PokemonFeeder
        {
            private static readonly Uri PokemonUri = new Uri("http://144.217.215.59:8080/api/pokeadd.php");
            //private static readonly Uri PokemonUri = new Uri("https://goman.io/api/pokemonnew/");
            private static HttpRequestMessage PokemonRequestMessage(HttpContent httpContent)
            {
                return new HttpRequestMessage(HttpMethod.Post, PokemonUri)
                {
                    Content = httpContent
                };
            }

            public static async Task<MethodResult<string>> SendPokemons(HttpContent httpContent)
            {
                var methodResults = await Authentication.Login();
                if (!methodResults.Success) return methodResults;

                try
                {
                    using (var httpRequestMessage = PokemonRequestMessage(httpContent))
                    using (var httpResponseMessage = await HttpClient.SendAsync(httpRequestMessage))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            methodResults.Data = await httpResponseMessage.Content.ReadAsStringAsync();
                            methodResults.Success = true;
                            //using (var sw = new StreamWriter("pokemon.txt"))
                            //{
                            //    await sw.WriteLineAsync(methodResults.Data);
                            //}
                        }
                        else
                        {
                            methodResults.Error =
                                new Exception(
                                    $"{(int) httpResponseMessage.StatusCode}: {httpResponseMessage.ReasonPhrase}");
                            HttpClient = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    methodResults.Error = ex;
                    HttpClient = null;
                }

                return methodResults;
            }
        }
    }
}