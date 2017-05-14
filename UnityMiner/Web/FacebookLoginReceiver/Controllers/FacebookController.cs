using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FacebookLoginReceiver.Controllers
{
    public class FacebookController : Controller
    {
        private static readonly string _fbClientId = "346314538774437";
        private static readonly string _fbClientSecret = "85dce2adec3dfcd47a753fd5b9639caf";
        private static IDictionary<string, string> _fbTokens = new Dictionary<string, string>();
        private static FacebookClient _fbClient = new FacebookClient();

        public ActionResult Index(string code, string state, string token)
        {
            if (!String.IsNullOrWhiteSpace(code) && !String.IsNullOrWhiteSpace(state))
            {
                try
                {

                    var response = (JsonObject)_fbClient.Get("oauth/access_token", new
                    {
                        client_id = _fbClientId,
                        client_secret = _fbClientSecret,
                        redirect_uri = Url.Action("Index", "Facebook", null, "http"),
                        code = code
                    });

                    if (response.ContainsKey("access_token") && response.ContainsKey("expires"))
                    {
                        _fbTokens.Add(state, response["access_token"].ToString());

                        ViewBag.Header = "Thank you for logging in.";
                        ViewBag.Message = "Please close this window and return to the game to continue";

                        return View();
                    }
                }
                catch (FacebookApiException e)
                {
                    if (e.ErrorCode == 100)
                    {
                        ViewBag.Header = "An error has occured.";
                        ViewBag.Message = e.Message;

                        return View();
                    }
                }
            }
            else if (!String.IsNullOrWhiteSpace(token))
            {
                string access_token = (_fbTokens.ContainsKey(token) ? _fbTokens[token] : String.Empty);

                if (!String.IsNullOrWhiteSpace(access_token))
                    _fbTokens.Remove(token);

                return this.Json(new { access_token = access_token });
            }

            return this.Json(new { error = true });
        }

        private new ActionResult Json(object data)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver()
            };

            var json = JsonConvert.SerializeObject(data, Formatting.None, settings);

            return new ContentResult
            {
                ContentType = "application/json",
                Content = json
            };
        }
    }
}