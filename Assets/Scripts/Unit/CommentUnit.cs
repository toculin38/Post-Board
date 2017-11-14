using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentUnit : MonoBehaviour {

    [SerializeField] Text commentText;

    public void SetText(string text) {
        commentText.text = text;
    }
}
