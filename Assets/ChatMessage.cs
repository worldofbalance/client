using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//hi
public class ChatMessage : MonoBehaviour {

	// Use this for initialization
  Text message;
  Text time;
  Text name;
  RectTransform header;
  void Awake() {
    this.header = this.transform.Find("header").GetComponent<RectTransform>();
    this.time = this.header.Find("time").GetComponent<Text>();
    this.name = this.header.Find("name").GetComponent<Text>();
    this.message = this.transform.Find("message").GetComponent<Text>();
  }
	void Start () {
    Awake();
	}
  public void setText(string text) {
    this.message.text = text;
  }
  public void setTime(string text) {
    this.time.text = text;
  }

  public void setUsername(string username) {
    this.name.text = username;
  }
	
	// Update is called once per frame
	void Update () {
	
	}
}
