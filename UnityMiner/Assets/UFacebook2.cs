using Facebook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class UFacebook2 : MonoBehaviour
{
		private static readonly string _facebookAppId = "802948049853464";
		private static readonly string _redirectEndpoint = "https://clicknmine-changron.c9users.io/login_success/";
		private static readonly short _checkInterval = 1000;

		public void Start ()
		{
				this.Login ();
		}

		public void Login ()
		{
				var token = Guid.NewGuid ().ToString ();
				var fbName = GameObject.Find ("Facebook Name").GetComponent<TextMesh> ();
				var fbPicture = GameObject.Find ("Facebook Picture").GetComponent<MeshRenderer> ();
				Debug.Log (token);
				Application.OpenURL (String.Format ("https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&state={2}", _facebookAppId, _redirectEndpoint, token));
		
				base.StartCoroutine (this.CheckForToken (token, access_token => {
						var fbClient = new FacebookClient (access_token);
						Debug.Log(fbClient.AccessToken);

						var me = (JsonObject)fbClient.Get ("me");
						Debug.Log(me);
						fbName.text = "Hello\n" + me["name"].ToString () + "\n";

						var profilePicture = new WWW (String.Format ("https://graph.facebook.com/{0}/picture?width=100&height=100", me ["id"].ToString ()));
						var tex = new Texture2D (100, 100, TextureFormat.DXT1, false);

						while (!profilePicture.isDone) {
						}
						;

						profilePicture.LoadImageIntoTexture (tex);
						fbPicture.material.mainTexture = tex;
				}));
		}

		private IEnumerator CheckForToken (string token, Action<string> result)
		{
				while (true) {
						var downloader = new WWW (_redirectEndpoint + "?token=" + token);

						yield return downloader;
						Debug.Log (_redirectEndpoint + "?token=" + token);
						Debug.Log (downloader.text);
						AccessToken at = JsonUtility.FromJson<AccessToken> (downloader.text);
						Debug.Log (at.access_token);
						Debug.Log (at.token_type);
						Debug.Log (at.expires_in);

						if (!String.IsNullOrEmpty (at.access_token)) {
								Debug.Log ("OK");			
								result (at.access_token);
								yield break;
						} else {
								yield return new WaitForSeconds (_checkInterval / 1000);

								this.CheckForToken (token, result);
						}
				}
		}

		[Serializable]
		public class AccessToken
		{
				public string access_token;
				public string token_type;
				public int expires_in;
				public AccessToken() {
					
				}
		}
}