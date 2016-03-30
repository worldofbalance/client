using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class BarGraph : MonoBehaviour
{

	private int window_id = Constants.GRAPH_WIN;
	// Window Properties
	private float left;
	private float top;
	private float width = 900;
	private float height = 400;
	private bool isActive = false;
	private bool isLegendActive = true;
	// Other
	private string title;
	private Rect windowRect;
	private List<string> xAxisLabels = new List<string> ();
	private int xRange;
	private int yRange = 1;
	private int xMin;
	private int xMax;
	private Texture2D lineMarkerTex;
	private string selected;
	private Vector3 lastMousePosition = Vector3.zero;
	private List<string> mouseOverLabels = new List<string> ();
	private Rect graphRect;
	private Rect sliderRect;
	private string lastSeriesToDraw;
	private float thickness = 2f;
	private Vector2 hStart;
	private Vector2 hEnd;
	private int xNumMarkers = 12;
	private int yNumMarkers = 5;

	public string xTitle { get; set; }

	public string yTitle { get; set; }

	private float xAxisLength;
	private float xUnitLength;
	private float yAxisLength;
	private float yUnitLength;
	private Vector2 vStart;
	private Vector2 vEnd;
	private Texture2D barTexture;
	private int mode = 1;

	public List<CSVObject> csvList { get; set; }

	private BarLegend barLegend;
	public Dictionary<string, Color> colorList = new Dictionary<string, Color> ();

	public List<SeriesSet> seriesSets { get; private set; }

	private int sliderValue = 0;
	private int maxSliderValue = 0;
	private static int perPage = 6;
	private float barWidth = 50;
	private float interBarWidth;

	// DH change
	private List<int> scores;  // list of scores, starting from initial and all attempts 
	private bool oppGraph;    // true = graph for opponent
	private string oppName;     // Name for opponent
	private string title2;
	private List<int> oppScores;     // Opponent score values; up to 5
	private int yRangeO;          // Opponent yRange value
    public DynamicRect rectO;

	public ConvergeManager convergeManager { get; set; }

	void Awake ()
	{
		seriesSets = new List<SeriesSet> ();
		csvList = new List<CSVObject> ();
		convergeManager = null;
		
		title = "Progress Report: Average Biomass Difference from Target";
		xTitle = "Attempt #";
		yTitle = "Biomass Difference";

		left = (Screen.width - width) / 2;
		top = 100;

		windowRect = new Rect (left, top, width, height);
		graphRect = new Rect (20, 30, 650, 325);
		sliderRect = new Rect (
			graphRect.x + (graphRect.width / 2) - 100, 
			graphRect.x + graphRect.height + 20, 
			200, 
			30
			);

		hStart = new Vector2 (85, graphRect.height - 75);
		hEnd = new Vector2 (graphRect.width - 50, hStart.y);
		
		xAxisLength = Vector2.Distance (hStart, hEnd) * 0.95f;
		xUnitLength = xAxisLength / xNumMarkers;
		interBarWidth = (xAxisLength - (barWidth * perPage)) / perPage;
		
		vStart = new Vector2 (hStart.x, hStart.y);
		vEnd = new Vector2 (vStart.x, 30);

		yAxisLength = Vector2.Distance (vStart, vEnd) * 0.95f;
		yUnitLength = yAxisLength / yNumMarkers;

		lineMarkerTex = Resources.Load<Texture2D> ("chart_dot");

		barTexture = Functions.CreateTexture2D (Color.white);
		
		barLegend = new BarLegend(new Rect(width - 200 - 20, 30, 200, 325), this);

		//gameObject.AddComponent<GraphInput>().graph = this;  jtc-commented out

		// DH change
		scores = new List<int>();  // arraylist of scores, starting from initial 
		oppGraph = false;  // default is player graph
		oppName = "";     // name of opponent
        rectO = new DynamicRect(0, 0, 0, barWidth);

	}

	// Use this for initialization
	void Start ()
	{
		//Debug.Log ("BarGraph::Start()");
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown ("1")) {
			SetMode (0);
		}
		
		if (Input.GetKeyDown ("2")) {
			SetMode (1);
		}
	}

	void OnGUI ()
	{
		//if (GUI.Button (new Rect (10, 70, 80, 30), Strings.GRAPH)) {
		//	SetActive (!isActive);
		//}

		if (isActive) {
			windowRect = GUI.Window (window_id, windowRect, MakeWindow, title, GUIStyle.none);
		}
	}
	
	void MakeWindow (int id)
	{
        if (oppGraph) {
            calcOppValues ();
        }
		Functions.DrawBackground (new Rect (0, 0, windowRect.width, windowRect.height), Constants.BG_TEXTURE_01);
		GUI.BringWindowToFront(id);
		// Window Title Styling
		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.font = Constants.FONT_01;
		style.fontSize = 16;
		// Window Title
		title2 = title;
		if (oppGraph) {
			title2 = title + " for " + oppName;
		}
		GUI.Label (new Rect ((windowRect.width - 530) / 2, 0, 500, 30), title2, style);
		// Draw Axes
		DrawGrid (graphRect);
		// Draw Slider
		if (!oppGraph)
			DrawSlider ();
		// Draw Legend
		if (isLegendActive && !oppGraph) {
			barLegend.Draw();
		}
		if (GUI.Button (new Rect (20, windowRect.height - 40, 50, 30), "Close")) {
			SetActive (!isActive);
		}
		

		// Handle Mouseover Collisions
		HandleCollision ();

		GUI.DragWindow ();
	}

	public void SetMode (int mode)
	{
		this.mode = mode;

		foreach (SeriesSet seriesSet in seriesSets) {
			seriesSet.rect.deltaTime = 0;
		}
	}
	
	public void SetActive (bool active)
	{
		isActive = active;
	}
	
	private void DrawGrid (Rect rect)
	{
		Color color = Color.white;

		GUI.BeginGroup (rect, GUI.skin.box);
		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.normal.textColor = Color.white;
		style.richText = true;
			
		// X-Axis
		Drawing.DrawLine (hStart, hEnd, color, thickness, false);

		// X-Axis Markers
		for (int i = 0; i < xNumMarkers; i++) {
			float xPos = hStart.x + xUnitLength * (i + 1), yPos = hStart.y - 5;
			// Unit Line
//				Drawing.DrawLine(new Vector2(xPos, yPos), new Vector2(xPos, yPos + 10), color, thickness, false);
			// Unit Label
			style.alignment = TextAnchor.UpperCenter;

			if (i < xAxisLabels.Count) {
//					GUI.Label(new Rect(xPos - 42, yPos + 10, 80, 70), xAxisLabels[xMin + i], style);
			}
		}
		// X-Axis Label
		style.alignment = TextAnchor.UpperCenter;
		GUI.Label (new Rect (hStart.x + (xAxisLength - 80) / 2, hStart.y + 35, 80, 30), "<color=yellow>" + xTitle + "</color>", style);
			
		// Y-Axis
		Drawing.DrawLine (vStart, vEnd, color, thickness, false);
		// Y-Axis Markers
		for (int i = 0; i <= yNumMarkers; i++) {
			float xPos = vStart.x - 5, yPos = vStart.y - yUnitLength * i;
			// Unit Line
			Drawing.DrawLine (new Vector2 (xPos, yPos), new Vector2 (xPos + 10, yPos), color, thickness, false);
			// Unit Label
			style.alignment = TextAnchor.UpperRight;
			if (oppGraph) {
				GUI.Label (new Rect (xPos - 85, yPos - 11, 80, 30), (i * yRangeO / 5).ToString (), style);
			} else {
				GUI.Label (new Rect (xPos - 85, yPos - 11, 80, 30), (i * yRange / 5).ToString (), style);
			}
		}
		// Y-Axis Label
		style.alignment = TextAnchor.UpperCenter;
			
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot (-90, new Vector2 (hStart.x - 35, (rect.height) / 2));
		GUI.Label (new Rect (hStart.x - 100, (rect.height - 60) / 2, 200, 30), 
		           "<color=orange>" + yTitle + "</color>", 
		           style
		           );
		GUI.matrix = matrix;

		// Draw Series Sets
//			for (int i = 0; i < Mathf.Min(seriesSets.Count, 3); i++) {
//				SeriesSet seriesSet = seriesSets[sliderValue + i];
//				DrawSeriesSet(seriesSet, i);
//			}

		// For player graph
		if ((seriesSets.Count > 0) && !oppGraph) {
			//draw default vs target
			SeriesSet seriesSet = seriesSets [0];
			DrawSeriesSet (seriesSet, 0);
			Vector2 v1 = new Vector2 (
				             seriesSet.rect.x + barWidth + (interBarWidth / 2), 
				             seriesSet.rect.y
			             );
			Vector2 v2 = new Vector2 (
				             v1.x, 
				             seriesSet.rect.y - yAxisLength
			             );
			//separate default from player attempts with a line
			Drawing.DrawLine (v1, v2, color, thickness, false);
			//draw attempts vs target
			for (int series = 1; series < Mathf.Min (perPage, seriesSets.Count); series++) {
				seriesSet = seriesSets [sliderValue + series];
				DrawSeriesSet (seriesSet, series);
			}
		} else if ((oppScores [0] != -1) && oppGraph) {
            Debug.Log ("BG: Drawing oppGraph");
			for (int i = 0; i < 5; i++) {
				if (oppScores [i] != -1) {
                    Debug.Log ("BG: i/oppScores: " + i + " " + oppScores [i]);
					DrawOppBar (oppScores [i], i);
				}
			}
		}
		GUI.EndGroup ();
	}

	private void DrawSeriesSet (SeriesSet seriesSet, int index = 0)
	{
		float xPos;

		xPos = hStart.x + interBarWidth/2 + (index * (barWidth + interBarWidth));

		seriesSet.rect.x = xPos;
		seriesSet.rect.y = hStart.y - 1;

		GUI.color = (seriesSet.label == selected) ? Color.gray : Color.white;

		// Rotate -90 degrees to allow Rect to expand upwards by adjusting the width
		Matrix4x4 matrix = GUI.matrix;
		Vector2 origin = new Vector2 (seriesSet.rect.x, seriesSet.rect.y);
		GUIUtility.RotateAroundPivot (-90, origin);
		// Draw Series Set
		GUI.BeginGroup (seriesSet.rect.GetRect ());
        Debug.Log ("seriesSet.rect: x,y,width,height: " + seriesSet.rect.x + " " + seriesSet.rect.y + " " +
            seriesSet.rect.width + " " + seriesSet.rect.height);
		float yNext = 0;

		foreach (BarSeries series in seriesSet.seriesList) {
			float barHeight = ((float) series.score / (float) yRange) * yAxisLength;  //prev series.GetSum()
			// Draw horizontally, then rotate vertically
			DynamicRect barRect = series.GetRect ();
			barRect.x = yNext;
			barRect.width = barHeight;
			// Next stacking position
			yNext += barHeight - 1f;
			
            Debug.Log ("barRect.rect: x,y,width,height: " + barRect.GetRect().x + " " + barRect.GetRect().y + " " +
                barRect.GetRect().width + " " + barRect.GetRect().height);
			Functions.DrawBackground (barRect.GetRect (), barTexture, series.color);
		}
		GUI.EndGroup ();
		// Restore Rotation
		GUI.matrix = matrix;
		// Expand "Animation"
		if (Mathf.Abs (seriesSet.rect.width - yAxisLength) > 0.1f) {
			seriesSet.rect.width = Mathf.Lerp (0, yAxisLength, seriesSet.rect.deltaTime += Time.deltaTime * 0.4f);
		} else {
			// Create labels
			GUIStyle style = new GUIStyle (GUI.skin.label);
			style.richText = true;
			style.alignment = TextAnchor.LowerCenter;

			foreach (BarSeries series in seriesSet.seriesList) {
				DynamicRect barRect = series.GetRect ();
				
				string text = series.score.ToString ();   //GetSum ().ToString ("F2");
//				GUI.Label(new Rect(seriesSet.rect.x - 65, yAxisLength - barRect.x - barRect.width / 2 - 9, 200, 60), text, style);
			}
		}

		{
			GUIStyle style = new GUIStyle (GUI.skin.label);
			style.richText = true;
			style.alignment = TextAnchor.UpperCenter;

			string xLabel = seriesSet.label;
			GUI.Label (new Rect (seriesSet.rect.x + (seriesSet.rect.height - 200) / 2, seriesSet.rect.y + 10, 200, 60), xLabel, style);
		}
	}

	private void DrawOppBar (int score, int index = 0)
	{
		float xPos;

		xPos = hStart.x + interBarWidth/2 + (index * (barWidth + interBarWidth));

		rectO.x = xPos;
		rectO.y = hStart.y - 1;
        Debug.Log ("BG: rectO: x, width, height: " + rectO.x + " " + rectO.width + " " + rectO.height);

		GUI.color = (false) ? Color.gray : Color.white;

		// Rotate -90 degrees to allow Rect to expand upwards by adjusting the width
		Matrix4x4 matrix = GUI.matrix;
		Vector2 origin = new Vector2 (rectO.x, rectO.y);
		GUIUtility.RotateAroundPivot (-90, origin);
		// Draw Series Set
		GUI.BeginGroup (rectO.GetRect ());
		float yNext = 0;


		float barHeight = ((float) score / (float) yRange) * yAxisLength;  //prev series.GetSum()
		// Draw horizontally, then rotate vertically
        DynamicRect barRect = new DynamicRect (0, 0, 0, barWidth);
		barRect.x = yNext;
		barRect.width = barHeight;
        Debug.Log ("BG: barRect: x, width, height: " + barRect.x + " " + barRect.width + " " + barRect.height);
        // barRect.height = barWidth;
		// Next stacking position

		Functions.DrawBackground (barRect.GetRect(), barTexture, Color.red);

		GUI.EndGroup ();
		// Restore Rotation
		GUI.matrix = matrix;
		// Expand "Animation"
		/*
		if (Mathf.Abs (seriesSet.rect.width - yAxisLength) > 0.1f) {
			seriesSet.rect.width = Mathf.Lerp (0, yAxisLength, seriesSet.rect.deltaTime += Time.deltaTime * 0.4f);
		} else {
			// Create labels
			GUIStyle style = new GUIStyle (GUI.skin.label);
			style.richText = true;
			style.alignment = TextAnchor.LowerCenter;

			foreach (BarSeries series in seriesSet.seriesList) {
				DynamicRect barRect = series.GetRect ();

				string text = series.score.ToString ();   //GetSum ().ToString ("F2");
				//				GUI.Label(new Rect(seriesSet.rect.x - 65, yAxisLength - barRect.x - barRect.width / 2 - 9, 200, 60), text, style);
			}
		}
		*/

		{
			GUIStyle style = new GUIStyle (GUI.skin.label);
			style.richText = true;
			style.alignment = TextAnchor.UpperCenter;

			// string xLabel = seriesSet.label;
			// GUI.Label (new Rect (seriesSet.rect.x + (seriesSet.rect.height - 200) / 2, seriesSet.rect.y + 10, 200, 60), xLabel, style);
		}
	}



	private void DrawMarker (string label, Rect rect, Color color, string text)
	{
		Color temp = GUI.color;

		if (rect.Contains (Event.current.mousePosition)) {
			mouseOverLabels.Add (label);

			if (label == selected) {
				GUIStyle style = new GUIStyle (GUI.skin.label);
				style.alignment = TextAnchor.UpperCenter;
				style.richText = true;

				GUI.Label (new Rect (
					Event.current.mousePosition.x - 100, 
					Event.current.mousePosition.y - 50, 
					200, 
					60
					), text, style);
			}
		}

		GUI.color = color;
		GUI.DrawTexture (rect, lineMarkerTex);

		GUI.color = temp;
	}
	
	private void DrawSlider ()
	{
		sliderValue = Mathf.RoundToInt (GUI.HorizontalSlider (
			sliderRect, 
			sliderValue, 
			0, 
			maxSliderValue
			));
	}

	public void HandleCollision ()
	{
		// "Collision" Detection for Lines/Markers
		if (mouseOverLabels.Count > 0) {
			// Solve Stacking Issue by Selecting Topmost
			if (mouseOverLabels.Contains (lastSeriesToDraw)) {
				selected = lastSeriesToDraw;
			} else {
				selected = mouseOverLabels [0]; // Select 1st Collision
			}
			
			if (selected != lastSeriesToDraw) {
				lastSeriesToDraw = selected;
			}
			// Clear "Collisions"
			mouseOverLabels.Clear ();
		} else {
			selected = null;
		}
	}

	public string GetSelectedSeries ()
	{
		return selected;
	}

	public void SelectSeries (string label)
	{
		selected = label;
		lastSeriesToDraw = label;
	}

	public void SetSeriesSetActive (int index, bool active)
	{
		SeriesSet seriesSet = seriesSets [index];
		seriesSet.SetActive (active);
	}

	public void UpdateData ()
	{
		CSVObject csvObject = SubtractCSVs (csvList [0], csvList [1]);
		CreateSeriesSet (csvObject);

		// Update X-Axis Labels
//		List<string> labels = csvObject.xLabels;
//		for (int i = 0; i < labels.Count; i++) {
//			int month = int.Parse(labels[i]);
//			string name = DateTimeFormatInfo.CurrentInfo.GetMonthName((month - 1) % 12 + 1).Substring(0, 3);
//			
//			if (xAxisLabels.Count != labels.Count) {
//				xAxisLabels.Add(name + ((month - 1) % 12 == 0 ? "\n'0" + (month / 12 + 1) : ""));
//			} else if (xAxisLabels[i] != name) {
//				xAxisLabels[i] = name;
//			}
//		}
	}

	public void CreateSeriesSet (CSVObject csvObject)
	{
		csvList.Add (csvObject);

		// DH - i believe the first one is the target
		if (csvList.Count == 1) {
			return;
		}

		//csvObject = SubtractCSVs (csvList [0], csvObject);
		Dictionary<string, int> diffList = csvObject.AverageDifferenceCSVs (csvList [0]);

		string label = seriesSets.Count == 0 ? "Initial" : seriesSets.Count.ToString ();
		SeriesSet seriesSet = new SeriesSet (label, new DynamicRect (0, 0, 0, barWidth));
		int seriesScore = 1;

		foreach (KeyValuePair<string, List<string>> entry in csvObject.csvList) {
			string name = entry.Key;
			// Designate color for Series (only used if no convergeManager)
			if (!colorList.ContainsKey (name)) {
				if (convergeManager == null) {
					colorList [name] = new Color (
						Random.Range (0.4f, 0.7f), 
						Random.Range (0.4f, 0.7f), 
						Random.Range (0.4f, 0.7f));
				} else {
					colorList [name] = convergeManager.seriesColors [name];
				}
			}
			// Convert values from String to Float. -1 to represent empty.
			//List<string> strValues = entry.Value;
			//List<float> values = new List<float> ();
			//for (int i = 0; i < strValues.Count; i++) {
			//	float value = (strValues [i] == "" ? -1f : float.Parse (strValues [i]));
			//	values.Add (value);
			//}
			int score = diffList [name];
			seriesScore += score;

			// Create a Series to store series
			BarSeries series = new BarSeries (
				name, 
				score, //values,                              
				new DynamicRect (0, 0, 0, seriesSet.rect.height), 
				colorList [name]);
			seriesSet.Add (series);
		}

		//sort seriesList in seriesSet
		seriesSet.Sort (convergeManager.seriesLabels);
		seriesSets.Add (seriesSet);

		// DH change
		// calculates the score for this addition and puts it in scores
		int maxValue = 1;  //float maxValue = 1;
		foreach (BarSeries series in seriesSet.seriesList) {
			maxValue += series.score;
			//foreach (float value in series.values) {
			//	if (value > 0) {
			//		maxValue += value;
			//	}
			//}
		}
		scores.Add (maxValue);
		Debug.Log ("BarGraph add score, score / count: " + maxValue + " " + scores.Count);

		
		// Adjust Max Y-Range
		//UpdateRange ();
		yRange = Mathf.Max (yRange, seriesScore);
		// Adjust init slider val; exclude initial (default) bar
		maxSliderValue = Mathf.Max (0, seriesSets.Count - perPage);
		sliderValue = maxSliderValue;
	}

	//Depricated, performed in CreateSeriesSet
	public void UpdateRange ()
	{
		int yMaxValue = 1;  //float yMaxValue = 1;
		// Sum of all values (scores) to find largest bar
		foreach (SeriesSet seriesSet in seriesSets) {
			if (!seriesSet.isActive) {
				continue;
			}

			int maxValue = 1;  //float maxValue = 1;
			foreach (BarSeries series in seriesSet.seriesList) {
				maxValue += series.score;
				//foreach (float value in series.values) {
				//	if (value > 0) {
				//		maxValue += value;
				//	}
				//}
			}

			yMaxValue = (int) Mathf.Max (maxValue, yMaxValue);
		}
		// Round sum to set max range
		int roundTo = int.Parse ("5".PadRight (((int)yMaxValue).ToString ().Length - 1, '0'));
		yRange = Mathf.CeilToInt (yMaxValue / roundTo) * roundTo;
	}

	// DH change
	// Method returns the improvement from the most recent round
	// improvement = latest score - Max(all previous scores)
	public int Improvement() {
		int numScores = scores.Count;
		if (numScores < 2) {
			return 0;
		}
		Debug.Log ("BG improvement, 0 value: " + scores[0]);
		int minValue = scores[0];
		for (int i = 1; i < (numScores - 1); i++) {
			Debug.Log ("BG: #/value: " + i + " " + scores [i]);
			if (scores[i] < minValue)
				minValue = scores[i];
		}
		return minValue - scores[numScores - 1];
		Debug.Log ("BarGraph, minValue/count/latest/improve: " + minValue + " " + numScores + " " + scores [numScores - 1] + 
			" " + (minValue - scores[numScores - 1]));
	}



	//depricated, using CSVObject::AverageDifferenceCSVs
	public CSVObject SubtractCSVs (CSVObject a, CSVObject b)
	{
		CSVObject csvObject = new CSVObject ();
		csvObject.xLabels = a.xLabels;
		
		foreach (string name in a.csvList.Keys) {
			List<string> csvList = new List<string> ();
			for (int i = 0; i < a.csvList[name].Count; i++) {
				string aVal = a.csvList [name] [i], bVal = b.csvList [name] [i];

				if (aVal == "" || bVal == "") {
					csvList.Add (aVal.Length > bVal.Length ? aVal : bVal);
				} else {
					float target = float.Parse (aVal), current = float.Parse (bVal);
					csvList.Add (Mathf.Abs (current - target).ToString ());
				}
			}
			csvObject.csvList [name] = csvList;
		}

		return csvObject;
	}

	public void Clear ()
	{
		csvList.Clear ();
		seriesSets.Clear ();
	}

	public void InputToCSVObject (string text, ConvergeManager convergeManager = null)
	{
		this.convergeManager = convergeManager;
		CSVObject csvObject = Functions.ParseCSV (text);
		CreateSeriesSet (csvObject);
	}

	// DH change
	public List<int> getScores() {
		return scores;
	}

	public void setOppGraph(bool oppGraph) {
		this.oppGraph = oppGraph;
	}

	public void setOppName(string oppName) {
		this.oppName = oppName;
	}

	public void setOppScores(List<int> oppScores) {
		this.oppScores = oppScores;
	}

	void calcOppValues() {
		// yRangeO is max of Opponent score values
		yRangeO = oppScores[0];
		for (int i = 1; i < 5; i++) {
			if (oppScores [i] != -1) {
				yRangeO = Mathf.Max(yRangeO, oppScores[i]);
			}
		}			
	}
		
}
