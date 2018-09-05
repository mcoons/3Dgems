using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TextObject
{
	public float myStartTime;
	public string myText;
	public Color myColor;
	public int mySize;
	public float myDisplayTime;
	public bool fadeMe;
	public float onTime;
	public float offTime;
}

[System.Serializable]
public class MessageQueue
{
	public GameObject myTextObject;
	public int myDefaultSize;
	public Color myDefaultColor;
	public List<TextObject> myQueue = new List<TextObject>();
}

[System.Serializable]
public class MessageQueues
{
	public List<MessageQueue> queueList = new List<MessageQueue>();
}

// The index refers to the 3Dtext objects on the screen
// Index +1 is for the text shadow

public enum TextQueueIndex
{
	Score = 0,
	Level5 = 2,
	Level4 = 4,
	Level3 = 6,
	Level2 = 8,
	Level1 = 10,
	Level0 = 12,
	Bottom = 14
}


public class TextManager : MonoBehaviour 
{
	public List<GameObject> Text3DObjects;
	public MessageQueues queueManager;
		
	public void AddMessage(int qIndex, string message, float displayTime, Color c)
	{
		if (qIndex > queueManager.queueList.Count) return;
		
		TextObject tmpO = new TextObject();
		
		tmpO.myText = message;
		tmpO.myDisplayTime = displayTime;
		tmpO.myStartTime = 0;
		tmpO.myColor = c;

		tmpO.mySize = queueManager.queueList[qIndex].myDefaultSize;
		tmpO.fadeMe = false;

		TextObject tmpOS = new TextObject();
		
		tmpOS.myText = message;
		tmpOS.myDisplayTime = displayTime;
		tmpOS.myStartTime = 0;
		tmpOS.myColor = Color.black;
		
		tmpOS.mySize = queueManager.queueList[qIndex+1].myDefaultSize;
		tmpOS.fadeMe = false;
		
		if (queueManager.queueList[qIndex].myQueue.Count > 0)
		{
			while (queueManager.queueList[qIndex].myQueue.Count > 0 && queueManager.queueList[qIndex].myQueue[0].myDisplayTime == -1)
				queueManager.queueList[qIndex].myQueue.RemoveAt(0);
		}
		
		if (queueManager.queueList[qIndex+1].myQueue.Count > 0)
		{
			while (queueManager.queueList[qIndex+1].myQueue.Count > 0 && queueManager.queueList[qIndex+1].myQueue[0].myDisplayTime == -1)
				queueManager.queueList[qIndex+1].myQueue.RemoveAt(0);
		}

		queueManager.queueList[qIndex].myQueue.Add(tmpO);
		queueManager.queueList[qIndex+1].myQueue.Add(tmpOS);
	}
	
	public void AddMessage(int qIndex, string message, float displayTime, Color c, bool fade, int size)
	{
		if (qIndex > queueManager.queueList.Count) return;
		
		TextObject tmpO = new TextObject();
		
		tmpO.myText = message;
		tmpO.myDisplayTime = displayTime;
		tmpO.myStartTime = 0;
		tmpO.myColor = c;
		tmpO.fadeMe = fade;
		
		tmpO.mySize = size;
		
		TextObject tmpOS = new TextObject();
		
		tmpOS.myText = message;
		tmpOS.myDisplayTime = displayTime;
		tmpOS.myStartTime = 0;
		tmpOS.myColor = Color.black;
		tmpOS.fadeMe = fade;
		
		tmpOS.mySize = size;

		// This code can result in unseen messages being removed !!!!!!
		// It may need a rewrite !!!!!!
		// Why did I do it this way?

		if (queueManager.queueList[qIndex].myQueue.Count > 0)
		{
			while (queueManager.queueList[qIndex].myQueue.Count > 0 && queueManager.queueList[qIndex].myQueue[0].myDisplayTime == -1)
				queueManager.queueList[qIndex].myQueue.RemoveAt(0);
		}

		
		if (queueManager.queueList[qIndex+1].myQueue.Count > 0)
		{
			while (queueManager.queueList[qIndex+1].myQueue.Count > 0 && queueManager.queueList[qIndex+1].myQueue[0].myDisplayTime == -1)
				queueManager.queueList[qIndex+1].myQueue.RemoveAt(0);
		}

		queueManager.queueList[qIndex].myQueue.Add(tmpO);
		queueManager.queueList[qIndex+1].myQueue.Add(tmpOS);
	}

	// Use this for initialization
	void Start () 
    {
		queueManager = new MessageQueues();
		queueManager.queueList = new List<MessageQueue>();

        foreach (GameObject item in Text3DObjects)
        {
            MessageQueue tmpQ = new MessageQueue();
            tmpQ.myTextObject = item;
            tmpQ.myQueue = new List<TextObject>();

			tmpQ.myDefaultColor = item.GetComponent<TextMesh>().color;
			tmpQ.myDefaultSize = item.GetComponent<TextMesh>().fontSize;

            queueManager.queueList.Add(tmpQ);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        foreach (MessageQueue Q in queueManager.queueList)
        {
            if (Q.myQueue.Count > 0)
            {
				if (Q.myQueue[0].myDisplayTime < 0 && Q.myQueue.Count == 1)
				{
					Q.myTextObject.GetComponent<TextMesh>().text = Q.myQueue[0].myText;
					Q.myTextObject.GetComponent<TextMesh>().fontSize = Q.myQueue[0].mySize;

					Color tmpColor = Q.myQueue[0].myColor;
					
					tmpColor.a = 1.0f;
					
					Q.myTextObject.GetComponent<TextMesh>().color = tmpColor;
				}
				else
				if (Q.myQueue[0].myStartTime == 0)
                {
                    Q.myQueue[0].myStartTime = Time.time;
                    Q.myTextObject.GetComponent<TextMesh>().text = Q.myQueue[0].myText;
					Q.myTextObject.GetComponent<TextMesh>().fontSize = Q.myQueue[0].mySize;

					Color tmpColor = Q.myQueue[0].myColor;
					
					tmpColor.a = 1.0f;
					
					Q.myTextObject.GetComponent<TextMesh>().color = tmpColor;
                }
                else
                if (Time.time - Q.myQueue[0].myStartTime > Q.myQueue[0].myDisplayTime)
                {
                    Q.myTextObject.GetComponent<TextMesh>().text = "";
                    Q.myQueue.RemoveAt(0);
                }
                else
				if (Q.myQueue[0].myDisplayTime - (Time.time - Q.myQueue[0].myStartTime) <=  1 && Q.myQueue[0].fadeMe)
	            {
                    Color tmpColor = Q.myQueue[0].myColor;
                    tmpColor.a = (Q.myQueue[0].myDisplayTime - (Time.time - Q.myQueue[0].myStartTime));
                    Q.myTextObject.GetComponent<TextMesh>().color = tmpColor;
                }
            }
        }
	}
}
