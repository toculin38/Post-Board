using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentUnit : MonoBehaviour
{

    [SerializeField] Image photoImage;
    [SerializeField] Text commentText;
    [SerializeField] CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup.DOFade(1, 0.3f).SetDelay(0.2f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void SetText(string text)
    {
        commentText.text = text;
    }

    public void SetPhoto(string uri)
    {
        StartCoroutine(SetPhotoImage(uri));
    }

    IEnumerator SetPhotoImage(string uri)
    {
        WWW www = new WWW(uri);
        yield return www;
        photoImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }
}
