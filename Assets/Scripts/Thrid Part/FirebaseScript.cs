using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseScript : MonoBehaviour
{
    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser User { get; private set; }
    public FirebaseDatabase Database { get;private set; }

    [Serializable] class PassUser : UnityEvent<FirebaseUser> { };
    [SerializeField] PassUser OnUserSignIn;
    [SerializeField] PassUser OnUserSignOut;

    // Use this for initialization
    void Awake()
    {
        InitializeDatabase();
        InitializeAuth();
    }

    void InitializeAuth()
    {
        Auth = FirebaseAuth.DefaultInstance;
        Auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void InitializeDatabase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://post-board-c5bf7.firebaseio.com/");
        Database = FirebaseDatabase.DefaultInstance;
    }

    public void SignInWithTokenString(string tokenString)
    {
        Credential credential = FacebookAuthProvider.GetCredential(tokenString);
        Auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("SignInWithCredentialAsync was canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
            }
            else
            {
                FirebaseUser newUser = task.Result;
                Debug.LogFormat("\nUser signed in successfully: { 0} ({ 1})", newUser.DisplayName, newUser.UserId);
            }
        });
    }

    public void StartTransaction(Func<MutableData, TransactionResult> transaction)
    {
        DatabaseReference reference = Database.GetReference("Topics");
        reference.RunTransaction(transaction).ContinueWith((task) =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception.ToString());
            }
            else
            {
                Debug.Log("Transaction Complete");
                Debug.Log(task.Result.ToString());
            }
        });
    }

    protected void StartListener()
    {
        Database.GetReference("Leaders").OrderByChild("score")
          .ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
          {
              if (e2.DatabaseError == null)
              {
                  Debug.Log("score value change");
                  Debug.Log(e2.Snapshot.ToString());
              }
              else
              {
                  Debug.LogError(e2.DatabaseError.Message);
              }
          };
    }

    void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (Auth.CurrentUser != User)
        {
            if (User != null)
            {
                OnUserSignOut.Invoke(User);
            }

            User = Auth.CurrentUser;

            if (Auth.CurrentUser != null)
            {
                OnUserSignIn.Invoke(User);
            }
        }
    }

}
