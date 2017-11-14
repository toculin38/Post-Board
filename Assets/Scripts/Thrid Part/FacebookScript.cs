using Facebook.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FacebookScript : MonoBehaviour
{
    [Serializable] class PassString : UnityEvent<string> { };
    [SerializeField] PassString OnSignInSuccess;

    void Awake()
    {
        InitializeFaceBook();
    }

    void InitializeFaceBook()
    {
        if (!FB.IsInitialized)
        {
            FB.Init();
        }
    }

    public void FaceBookLogIn()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "user_friends" }, OnFaceBookLogInFinish);
    }

    public void FaceBookLogOut()
    {
        FB.LogOut();
    }

    void OnFaceBookLogInFinish(ILoginResult loginResult)
    {
        if (loginResult.Error == null)
        {
            OnSignInSuccess.Invoke(loginResult.AccessToken.TokenString);
        }
        else
        {
            Debug.Log(loginResult.Error);
        }
    }

}
