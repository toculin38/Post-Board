using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ChatPage : MonoBehaviour
{

    [SerializeField] CommentUnit commentUnitPrefab;
    [SerializeField] Transform chatContentTransform;

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    public void OnInputEndEdit(string text)
    {
        for (int i = 0, counter = 0; i < text.Length; i++)
        {
            int charlen = Encoding.GetEncoding("big5").GetBytes(text[i].ToString()).Length;

            if (counter > 29)//每15個中文字或30個英文字就換行
            {
                text = text.Insert(i, "\n");
            }

            if (text[i] == '\n')
            {
                counter = 0;
            }
            else
            {
                counter = counter + charlen;
            }
        }

        CommentUnit unit = Instantiate(commentUnitPrefab, chatContentTransform, false);
        unit.SetText(text);
    }
}
