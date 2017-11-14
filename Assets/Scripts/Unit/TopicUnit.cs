using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicUnit : MonoBehaviour
{
    [SerializeField] Text topicNameText;
    [SerializeField] Text dateTimeText;

    public void SetUnit(string topicName, string dateTime)
    {
        topicNameText.text = topicName;
        dateTimeText.text = dateTime;
    }

}
