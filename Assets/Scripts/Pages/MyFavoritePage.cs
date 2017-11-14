using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyFavoritePage : MonoBehaviour
{
    [SerializeField] FirebaseScript firebaseScript;
    [SerializeField] ChatPage chatPage;
    [SerializeField] Image photoImage;
    [SerializeField] Text nameText;

    [SerializeField] TopicUnit topicUnitPrefab;
    [SerializeField] Transform topicContentTransform;

    void Start()
    {
        SetFavoratie();
    }

    public void OnCreateTopicClick()
    {
        firebaseScript.StartTransaction((mutableData) =>
        {
            List<object> topics = mutableData.Value != null ? mutableData.Value as List<object> : new List<object>();

            Dictionary<string, object> newTopicMap = new Dictionary<string, object>();
            newTopicMap.Add("Title", "New Topic");
            newTopicMap.Add("DateTime", DateTime.Now.ToString());
            newTopicMap.Add("Info", "說你好");
            newTopicMap.Add("TrafficInfo", "大西洋游到太平洋轉台灣海峽在基隆港上岸");

            topics.Add(newTopicMap);
            mutableData.Value = topics;
            return TransactionResult.Success(mutableData);
        });
    }

    public void OnUserSignIn(Firebase.Auth.FirebaseUser firebaseUser)
    {
        SetUserContent(firebaseUser.DisplayName, firebaseUser.PhotoUrl);
        SetFavoratie();
    }

    void SetUserContent(string userName, Uri photoUrl)
    {
        nameText.text = userName;

        if (photoUrl != null)
        {
            StartCoroutine(SetPhotoImage(photoUrl));
        }
    }

    void SetFavoratie()
    {
        firebaseScript.Database.GetReference("Topics").ChildAdded += (object sender, ChildChangedEventArgs e2) => { AddFavorite(e2.Snapshot); };
    }

    void AddFavorite(DataSnapshot snapshot)
    {
        string title = snapshot.Child("Title").Value.ToString();
        string info = snapshot.Child("Info").Value.ToString();
        TopicUnit unit = Instantiate(topicUnitPrefab, topicContentTransform, false);
        unit.SetUnit(title, info);
        unit.GetComponent<Button>().onClick.AddListener(() =>
        {
            print(snapshot.Child("Comment").Reference.ToString());
            chatPage.gameObject.SetActive(true);
        });
    }

    IEnumerator SetPhotoImage(Uri uri)
    {
        WWW www = new WWW(uri.OriginalString);
        yield return www;
        photoImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }
}
