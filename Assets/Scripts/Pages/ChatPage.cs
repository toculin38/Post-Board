using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ChatPage : MonoBehaviour
{
    [SerializeField] FirebaseScript firebaseScript;
    [SerializeField] CommentUnit commentUnitPrefab;
    [SerializeField] Transform chatContentTransform;
    [SerializeField] InputField inputField;
    [SerializeField] Font font;

    DatabaseReference reference;
    EventHandler<ChildChangedEventArgs> addCommentEvent;

    public void SetReference(DatabaseReference reference)
    {
        this.reference = reference;
        addCommentEvent = (object sender, ChildChangedEventArgs e2) => { Addcomment(e2.Snapshot); };
    }

    void OnEnable()
    {
        reference.ChildAdded += addCommentEvent;
    }

    void OnDisable()
    {
        reference.ChildAdded -= addCommentEvent;
        DestroyAllUnits();
    }

    void DestroyAllUnits()
    {
        CommentUnit[] units = chatContentTransform.GetComponentsInChildren<CommentUnit>();
        for (int i = 0; i < units.Length; i++)
        {
            Destroy(units[i].gameObject);
        }
    }

    void Addcomment(DataSnapshot snapshot)
    {

        string text = snapshot.Child("Message").Value.ToString();
        CommentUnit unit = Instantiate(commentUnitPrefab, chatContentTransform, false);

        if (snapshot.Child("PhotoUri").Value.ToString() != "Editor")
        {
            unit.SetPhoto(snapshot.Child("PhotoUri").Value.ToString());
        }

        font.RequestCharactersInTexture(text);

        for (int i = 0, counter = 0; i < text.Length; i++)
        {
            if (counter > 220)
            {
                text = text.Insert(i, "\n");
            }

            if (text[i] == '\n')
            {
                counter = 0;
            }
            else
            {
                CharacterInfo characterInfo;
                if (font.GetCharacterInfo(text[i], out characterInfo))
                {
                    counter = counter + characterInfo.advance;
                }
            }
        }

        unit.SetText(text);
    }

    public void OnInputSubmit()
    {
        string text = inputField.text;
        firebaseScript.StartTransaction(reference, (mutableData) =>
        {
            List<object> comments = mutableData.Value != null ? mutableData.Value as List<object> : new List<object>();

            Dictionary<string, object> newTopicMap = new Dictionary<string, object>();
            if (firebaseScript.User != null)
            {
                newTopicMap.Add("User", firebaseScript.User.DisplayName);
                newTopicMap.Add("PhotoUri", firebaseScript.User.PhotoUrl.OriginalString);
            }
            else
            {
                newTopicMap.Add("User", "Editor");
                newTopicMap.Add("PhotoUri", "Editor");
            }
            newTopicMap.Add("Message", text);
            newTopicMap.Add("Time", DateTime.Now.ToString());
            comments.Add(newTopicMap);
            mutableData.Value = comments;
            return TransactionResult.Success(mutableData);
        });

        inputField.text = "";
    }

}
