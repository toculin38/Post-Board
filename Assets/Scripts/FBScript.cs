using Facebook.Unity;
using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FBScript : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;

    [SerializeField] Text debugText;
    [SerializeField] Image photoImage;

    void Awake()
    {
        InitializeFirebase();
        InitializeFaceBook();
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void InitializeFaceBook()
    {
        if (!FB.IsInitialized)
        {
            FB.Init();
        }
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                debugText.text += "Signed out " + user.UserId;
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                debugText.text += "\nSigned in " + user.UserId;
                debugText.text += "\nName : " + user.DisplayName;
                debugText.text += "\nEmail : " + user.Email;
                debugText.text += "\nPhotoUrl : " + user.PhotoUrl;
                StartCoroutine(SetPhotoImage(user.PhotoUrl.ToString()));
            }
        }
    }

    IEnumerator SetPhotoImage(string url) {
        WWW www = new WWW(url);
        yield return www;
        photoImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    public void Login()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "user_friends" },
            (loginResult) =>
            {
                if (loginResult.Error == null)
                {
                    Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(loginResult.AccessToken.TokenString);

                    auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            debugText.text += "\nSignInWithCredentialAsync was canceled.";
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            debugText.text += "\nSignInWithCredentialAsync encountered an error: " + task.Exception;
                            return;
                        }

                        Firebase.Auth.FirebaseUser newUser = task.Result;
                        debugText.text += string.Format("\nUser signed in successfully: { 0} ({ 1})", newUser.DisplayName, newUser.UserId);
                    });
                }
                else
                {
                    debugText.text += "\nloginResult.Error";
                }
            });
    }

}
