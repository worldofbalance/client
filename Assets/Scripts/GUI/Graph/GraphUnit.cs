using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;

//single graph
public class GraphUnit
{
	
	// Window Properties
	private float left;
	private float top;
	private float width;
	private float height;
	// Other
	public string title { get; set; }
	private List<string> xAxisLabels = new List<string> ();
	private int xRange;
	public int yRange { get; set; }
	public int yRangeActual { get; set; }
	private int xMin;
	private int xMax;
	private Texture2D lineMarkerTex;
	private Rect graphRect;
	private Rect monthSliderRect;
	private float thicknessAxis = 2f;
	private float thicknessLine = 2f;  //jtc
	private float thicknessLineSelected = 8f;  //jtc
	private Vector2 hStart;
	private Vector2 hEnd;
	private Vector2 vStart;
	private Vector2 vEnd;
	public string xTitle { get; set; }
	public string yTitle { get; set; }
	private float xAxisLength;
	private float xUnitLength;
	private float yAxisLength;
	private float yUnitLength;
	private int xNumMarkers;
	private int yNumMarkers = 5;
	private float monthSliderValue;
	private float monthSliderStart;
	private float monthSliderDT;
	private int scrollToMonth = -1;
	private Font font;
	private float bufferBorder = 10;
	private float yLabelingWidth = 65;
	public Dictionary<string, Series> seriesList { get; private set; }
	public ConvergeManager convergeManager { get; set; }
	private bool isFirstGraph;
	
	//1/28/15 - jtc - orig version gets these vars from GameState
	private List<string> labels;
	public CSVObject csv { get; set; }
		
	public GraphUnit (
			CSVObject csv,
			float left, 
			float top, 
			float width, 
			float height, 
			string title,
			int xNumMarkers,
			ConvergeManager convergeManager,
			bool isFirstGraph
	)
	{
		this.csv = csv;
		this.labels = this.csv.xLabels;
		this.left = left + (isFirstGraph ? 0 : yLabelingWidth / 2);
		this.top = top;
		this.width = width + (isFirstGraph ? yLabelingWidth / 2 : - yLabelingWidth / 2);
		this.height = height;
		this.title = title;
		this.xNumMarkers = xNumMarkers;
		this.convergeManager = convergeManager;
		this.isFirstGraph = isFirstGraph;

		xTitle = "Month";
		yTitle = "Score";
		yTitle = "Biomass";

		seriesList = new Dictionary<string, Series> ();
		
		graphRect = new Rect (this.left, this.top, this.width, this.height);

		hStart = new Vector2 ((isFirstGraph ? yLabelingWidth : 0) + bufferBorder, graphRect.height - 35);
		hEnd = new Vector2 (graphRect.width - bufferBorder, hStart.y);
		
		xAxisLength = Vector2.Distance (hStart, hEnd);  // * 0.95f;
		xUnitLength = xAxisLength / xNumMarkers;
		
		vStart = new Vector2 (hStart.x, hStart.y);
		vEnd = new Vector2 (vStart.x, bufferBorder);
		
		yAxisLength = Vector2.Distance (vStart, vEnd);  // * 0.95f;
		yUnitLength = yAxisLength / yNumMarkers;
		
		lineMarkerTex = Resources.Load<Texture2D> ("chart_dot");
		
		font = Resources.Load<Font> ("Fonts/" + "Chalkboard");

		UpdateData ();
	}

	public void DrawGraph ()
	{
		GUI.depth = -1;
		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.font = font;
		style.fontSize = 12;
		
		GUI.Label (new Rect (left + (width - 100) / 2, top, 100, 30), title, style);
		
		DrawGrid ();

		// "Collision" Detection for Lines/Markers
		if (!isFirstGraph) {
			if (convergeManager.mouseOverLabels.Count > 0) {
				// Solve Stacking Issue by Selecting Topmost
				if (convergeManager.mouseOverLabels.Contains (convergeManager.lastSeriesToDraw)) {
					convergeManager.selected = convergeManager.lastSeriesToDraw;
				} else {
					convergeManager.selected = convergeManager.mouseOverLabels [0]; // Select 1st Collision
				}
			
				if (convergeManager.selected != convergeManager.lastSeriesToDraw) {
					convergeManager.lastSeriesToDraw = convergeManager.selected;
				}
				// Clear "Collisions"
				convergeManager.mouseOverLabels.Clear ();
			} else {
				convergeManager.selected = null;
			}
		}

	}
	
	private void DrawGrid ()
	{
		Color color = Color.white;
		
		GUI.BeginGroup (graphRect, GUI.skin.box);
		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.normal.textColor = Color.white;
		style.richText = true;
		
		// X-Axis
		Drawing.DrawLine (hStart, hEnd, color, thicknessAxis, false);
		// X-Axis Markers
		if (false) {  //jtc - disabled; too many
			for (int i = 0; i < xNumMarkers; i++) {
				float xPos = hStart.x + xUnitLength * (i + 1), yPos = hStart.y - 5;
				// Unit Line
				Drawing.DrawLine (new Vector2 (xPos, yPos), new Vector2 (xPos, yPos + 10), color, thicknessAxis, false);
				// Unit Label
				style.alignment = TextAnchor.UpperCenter;
				if (i < xAxisLabels.Count) {
					GUI.Label (new Rect (xPos - 42, yPos + 10, 80, 70), xAxisLabels [xMin + i], style);
				}
			}
		}
		// X-Axis Label
		style.alignment = TextAnchor.UpperCenter;
		GUI.Label (new Rect (hStart.x + (xAxisLength - 80) / 2, hStart.y + 5, 80, 20), "<color=yellow>" + xTitle + "</color>", style);
		
		// Y-Axis
		Drawing.DrawLine (vStart, vEnd, color, thicknessAxis, false);
		// Y-Axis Markers
		if (isFirstGraph) {
			for (int i = 0; i <= yNumMarkers; i++) {
				float xPos = vStart.x - 5, yPos = vStart.y - yUnitLength * i;
				// Unit Line
				Drawing.DrawLine (new Vector2 (xPos, yPos), new Vector2 (xPos + 10, yPos), color, thicknessAxis, false);
				// Unit Label
				style.alignment = TextAnchor.UpperRight;
				GUI.Label (new Rect (xPos - 85, yPos - 11, 80, 30), (i * yRange / 5).ToString (), style);
			}
		}
		// Y-Axis Label
		style.alignment = TextAnchor.UpperCenter;
		
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot (-90, new Vector2 (hStart.x - 35, graphRect.height / 2));
		if (isFirstGraph) {
			GUI.Label (new Rect (hStart.x - 40, graphRect.height / 2 - 35, 80, 30), "<color=orange>" + yTitle + "</color>", style);
		}
		GUI.matrix = matrix;
		
		// Draw Behind Series Only
		foreach (string label in convergeManager.seriesLabels) {
			if (label == convergeManager.lastSeriesToDraw || convergeManager.excludeList.Contains (label)) {
				continue;
			}

			Debug.Log ("label " + label);
			DrawSeries (seriesList [label]);
		}
		// Front Series Drawn Last
		if (!convergeManager.excludeList.Contains (convergeManager.lastSeriesToDraw)) {
			DrawSeries (seriesList [convergeManager.lastSeriesToDraw]);
		}
		GUI.EndGroup ();
	}
	
	private void DrawSeries (Series series)
	{

		if (Mathf.Abs (series.width - graphRect.width) > 0.1f) {
			series.width = Mathf.Lerp (0, graphRect.width, series.deltaTime += Time.deltaTime * 0.5f);
		}
		
		GUI.BeginGroup (series.GetRect ());
		List<float> values = series.values;
		//Color color = (series.label == convergeManager.selected) ? Color.white : convergeManager.seriesColors [series.label];
		Color color = convergeManager.seriesColors [series.label];
		float thickness = (series.label == convergeManager.selected) ? thicknessLineSelected : thicknessLine;  //jtc - new
		
		{ // Draw First Point
			string text = ("<color=#" + convergeManager.seriesColorsHex [series.label] + ">" + series.label + "</color>" + '\n' + values [xMin].ToString ("F2"));
			//DrawMarker(series.label, new Rect(hStart.x + xUnitLength - 7, hStart.y - (values[xMin] / yRange * yAxisLength) - 7, 14, 14), color, text);
		}
		
		for (int i = 1; i < Mathf.Min(series.values.Count, xNumMarkers); i++) {
			// Previous Point
			float xPos = hStart.x + xUnitLength * i;
			float yPos = hStart.y - (values [xMin + i - 1] / yRange * yAxisLength);
			// Current Point
			float xPosNext = hStart.x + xUnitLength * (i + 1);
			float yPosNext = hStart.y - (values [xMin + i] / yRange * yAxisLength);
			// Connect the Points by Drawing Line
			Drawing.DrawLine (new Vector2 (xPos, yPos), new Vector2 (xPosNext, yPosNext), color, thickness, true);
			// Draw End Point
			string text = ("<color=#" + convergeManager.seriesColorsHex [series.label] + ">" + series.label + "</color>" + '\n' + values [xMin + i].ToString ("F2"));
			//DrawMarker(series.label, new Rect(xPosNext - 7, yPosNext - 7, 14, 14), color, text);
		}
		GUI.EndGroup ();
	}
	
	private void DrawMarker (string label, Rect rect, Color color, string text)
	{
		Color temp = GUI.color;
		
		if (rect.Contains (Event.current.mousePosition)) {
			convergeManager.mouseOverLabels.Add (label);
			
			if (label == convergeManager.selected) {
				GUIStyle style = new GUIStyle (GUI.skin.label);
				style.alignment = TextAnchor.UpperCenter;
				style.richText = true;
				
				GUI.Label (new Rect (Event.current.mousePosition.x - 100, Event.current.mousePosition.y - 50, 200, 60), text, style);
			}
		}
		
		GUI.color = color;
		GUI.DrawTexture (rect, lineMarkerTex);
		
		GUI.color = temp;
	}
	
	private void DrawMonthSlider ()
	{
		monthSliderValue = Mathf.RoundToInt (GUI.HorizontalSlider (monthSliderRect, monthSliderValue, 0, Mathf.Max (0, xAxisLabels.Count - xNumMarkers - 1)));
		xMin = (int)monthSliderValue;
		
		if (scrollToMonth != -1) {
			if (Mathf.Abs (monthSliderValue - scrollToMonth) > 0.1f) {
				monthSliderValue = Mathf.Lerp (monthSliderStart, scrollToMonth, monthSliderDT += Time.deltaTime);
			} else {
				scrollToMonth = -1;
			}
		}
	}
	
	public void ShowMonth (int month)
	{
		monthSliderStart = monthSliderValue;
		monthSliderDT = 0;
		scrollToMonth = Mathf.Clamp (month, 1, xAxisLabels.Count - xNumMarkers) - 1;
	}
	
	public void ShowLastMonth ()
	{
		ShowMonth (xAxisLabels.Count);
	}	

	public void UpdateData ()
	{
		// Update X-Axis Labels
		//List<string> labels = GameState.csvList.xLabels;
		if (labels == null) {
			Debug.LogError ("Invalid data submitted to GraphUnit::UpdateDate()");
			return;
		}
		for (int i = 0; i < labels.Count; i++) {
			int month = int.Parse (labels [i]);
			string name = DateTimeFormatInfo.CurrentInfo.GetMonthName ((month - 1) % 12 + 1).Substring (0, 3);
			
			if (xAxisLabels.Count != labels.Count) {
				xAxisLabels.Add (name + ((month - 1) % 12 == 0 ? "\n'0" + (month / 12 + 1) : ""));
			} else if (xAxisLabels [i] != name) {
				xAxisLabels [i] = name;
			}
		}
		
		// Update Values
		float yMaxValue = 1;
		
		//CSVObject csv = GameState.csvList;
		foreach (KeyValuePair<string, List<string>> entry in csv.csvList) {
			string name = entry.Key;
			List<string> values = entry.Value;
			
			if (name == ".xLabels") {
				continue;
			}
			
			if (!seriesList.ContainsKey (name)) {
				Rect seriesRect = new Rect (0, 0, 0, graphRect.height);
				//moved color setting to ConvergeManager.SetColors();
				//Color color = new Color (Random.Range (0.2f, 1.0f), Random.Range (0.2f, 1.0f), Random.Range (0.2f, 1.0f));
				
				seriesList [name] = new Series (name, seriesRect, Color.white);  //not using this color var
				if (!isFirstGraph) {
					convergeManager.seriesLabels.Add (name);
					//convergeManager.seriesColors [name] = color;
					//convergeManager.seriesColorsHex [name] = Functions.ColorToHex(color);

					if (convergeManager.lastSeriesToDraw == null) {
						convergeManager.lastSeriesToDraw = name;
					}
				}
			}
			
			List<float> temp = seriesList [name].values;
			for (int i = 0; i < values.Count - 1; i++) { // Exclude Last Point
				float value = (values [i] == "" ? -1f : float.Parse (values [i]));
				
				if (temp.Count != values.Count) {
					temp.Add (value);
				} else if (!Mathf.Approximately (temp [i], value)) {
					temp [i] = value;
				}
				
				yMaxValue = Mathf.Max (value, yMaxValue);
			}
		}
	
		if (!isFirstGraph) {
			convergeManager.SortLabelsAndNodes ();
			convergeManager.SetColors ();
		}
		
		int roundTo = int.Parse ("5".PadRight (((int)yMaxValue).ToString ().Length - 1, '0'));
		yRangeActual = Mathf.CeilToInt (yMaxValue / roundTo) * roundTo;
		yRange = yRangeActual;
	}
	
}
