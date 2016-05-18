using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RunnerUI : MonoBehaviour {

	//Passed from from unity inspector
	public Canvas canvas;
	public EventSystem sys;

	//Vars passed from GameManager
	private float start;
	private float end;
	private GameObject player1;
	private GameObject player2;

	//p1Indicator Vars
	private GameObject p1Indicator;
	private RectTransform r1;
	private CanvasRenderer c1;
	private Image i1;

	//p2Indicator Vars
	private GameObject p2Indicator;
	private RectTransform r2;
	private CanvasRenderer c2;
	private Image i2;

	// Use this for initialization
	void Start () {
		p1Indicator = new GameObject ();
		p2Indicator = new GameObject ();

		p1Indicator.name = "p1Indicator";
		p2Indicator.name = "p2Indicator";

		p1Indicator.transform.parent = canvas.transform;
		p2Indicator.transform.parent = canvas.transform;

		r1 = p1Indicator.AddComponent<RectTransform> ();
		r1.sizeDelta = new Vector2 (160f, 160f);
		r2 = p2Indicator.AddComponent<RectTransform> ();
		r2.sizeDelta = new Vector2 (160f, 160f);

		c1 = p1Indicator.AddComponent<CanvasRenderer> ();
		c2 = p2Indicator.AddComponent<CanvasRenderer> ();

		i1 = p1Indicator.AddComponent<Image> ();
		i1.sprite = Resources.Load<Sprite> ("Art/player1Indicator");
		i2 = p2Indicator.AddComponent<Image> ();
		i2.sprite = Resources.Load<Sprite> ("Art/player2Indicator");
	}

	// Update is called once per frame
	void Update () {
		drawMiniMap ();
	}

	//Called by GameManager at startup
	public void setStartandEndPoints(float s, float e){
		start = s;
		end = e;
	}

	//Getting player1 for position monitoring
	public void setPlayer1(GameObject p1){
		player1 = p1;
	}

	//Getting player2 for position monitoring
	public void SetPlayer2(GameObject p2){
		player2 = p2;
	}

	//width of progressBar is 723.42 units
	//y position of progress bar is -274.3
	//x position 0 is the center of progress bar
	//x position -363.71 is beginning of progress bar
	private void drawMiniMap(){
		float x1; 
		float x2;
		float percentDone1; 
		float percentDone2; 
		float barPos1;
		float barPos2;

		x1 = player1.transform.localPosition.x;
		x2 =  player2.transform.localPosition.x;
		percentDone1 = 1 - ((end - x1) / end);
		percentDone2 = 1 - ((end - x2) / end);

    if (percentDone1 > .95 || percentDone2 > .95)
    {
        dealWithNearFinish();
    }

		barPos1 = 7234.2f * percentDone1;
		barPos2 = 7234.2f * percentDone2;

		r1.localPosition = new Vector3 (-3637.1f + barPos1, -2743f, 0f);
		r2.localPosition = new Vector3 (-3637.1f + barPos2, -2743f, 0f);
	}

  //Add logic for when one player is close to finishing
  void dealWithNearFinish()
  {
      // Add logic
      Debug.Log("Player near finish");
  }
}
