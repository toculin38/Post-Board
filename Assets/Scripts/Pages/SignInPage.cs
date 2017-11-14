using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignInPage : MonoBehaviour {

    public void OnUserSignIn(Firebase.Auth.FirebaseUser user) {
        gameObject.SetActive(false);
    }

    public void OnUserSignOut(Firebase.Auth.FirebaseUser user)
    {
        gameObject.SetActive(true);
    }
}
