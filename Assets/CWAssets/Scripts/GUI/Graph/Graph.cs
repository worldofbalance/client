using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
namespace CW{
public class Graph : MonoBehaviour {

	private int window_id = Constants.GRAPH_WIN;
	// Window Properties
	private float left;
	private float top;
	private float width = 805;
	private float height = 400;
	private bool isActive = false;
	private bool isLegendActive = true;
	// Other
	private string title;
	private Rect windowRect;
	private List<string> xAxisLabels = new List<string>();
	private int xRange;
	private int yRange;
	private List<string> seriesLabels = new List<string>();
	private int xMin;
	private int xMax;
	private List<string> excludeList = new List<string>();
	private Texture2D lineMarkerTex;
	private Rect graphRect;
	private Rect monthSliderRect;
	private Rect legendRect;
	private Rect legendScrollRect;
	private Vector2 legendScrollPos = Vector2.zero;
	private bool isLegendContentHidden = false;
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
	private string selected;
	private Vector3 lastMousePosition = Vector3.zero;
	private List<string> mouseOverLabels = new List<string>();
	private string lastSeriesToDraw;
	private float monthSliderValue;
	private float monthSliderStart;
	private float monthSliderDT;
	private int scrollToMonth = -1;
	private Dictionary<string, Series> seriesList = new Dictionary<string, Series>();
	private Texture2D bgTexture;
	private Font font;
	
	void Awake() {
		title = "Graph";
		xTitle = "Month";
		yTitle = "Score";
		yTitle = "Biomass";

		left = (Screen.width - width) / 2;//20;
		top = 100;

		windowRect = new Rect(left, top, width, height);
		graphRect = new Rect(20, 30, 550, 325);
		monthSliderRect = new Rect(graphRect.x, graphRect.x + graphRect.height + 20, graphRect.width, 30);
		legendRect = new Rect(width - 200 - 20, 30, 200, 325);
		legendScrollRect = new Rect(legendRect.width * 0.05f, 40, legendRect.width * 0.9f, legendRect.height * 0.75f);

		hStart = new Vector2(85, graphRect.height - 75);
		hEnd = new Vector2(graphRect.width - 50, hStart.y);
		
		xAxisLength = Vector2.Distance(hStart, hEnd) * 0.95f;
		xUnitLength = xAxisLength / xNumMarkers;
		
		vStart = new Vector2(hStart.x, hStart.y);
		vEnd = new Vector2(vStart.x, 30);

		yAxisLength = Vector2.Distance(vStart, vEnd) * 0.95f;
		yUnitLength = yAxisLength / yNumMarkers;

		lineMarkerTex = Resources.Load<Texture2D>("chart_dot");

		bgTexture = Resources.Load<Texture2D>(Constants.THEME_PATH + Constants.ACTIVE_THEME + "/gui_bg");
		font = Resources.Load<Font>("Fonts/" + "Chalkboard");
	}

	// Use this for initialization
	void Start() {
		UpdateData();
//		StartCoroutine(UpdateDataRoutine(10f));
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown("1")) {
			UpdateData();
		}
		if (Input.GetKeyDown("2")) {
			ShowMonth(1);
		}
		if (Input.GetKeyDown("3")) {
			ShowLastMonth();
		}
	}
	
	void OnGUI() {
		if (GUI.Button(new Rect(10, 70, 80, 30), "Graph")) {
			isActive = !isActive;
		}

		if (isActive) {
			windowRect = GUI.Window(window_id, windowRect, MakeWindow, title);
		}
	}
	
	void MakeWindow(int id) {

        Debug.Log("this is MakeWindow calling with id: " + id);

		Functions.DrawBackground(new Rect(0, 0, width, height), bgTexture);
		
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.UpperCenter;
		style.font = font;
		style.fontSize = 16;
		
		GUI.Label(new Rect((windowRect.width - 100) / 2, 0, 100, 30), "Graph", style);

		DrawGrid();

		// "Collision" Detection for Lines/Markers
		if (mouseOverLabels.Count > 0) {
			// Solve Stacking Issue by Selecting Topmost
			if (mouseOverLabels.Contains(lastSeriesToDraw)) {
				selected = lastSeriesToDraw;
			} else {
				selected = mouseOverLabels[0]; // Select 1st Collision
			}

			if (selected != lastSeriesToDraw) {
				lastSeriesToDraw = selected;
			}
			// Clear "Collisions"
			mouseOverLabels.Clear();
		} else {
			selected = null;
		}

		DrawMonthSlider();

		if (isLegendActive) {
			DrawLegend();
		}

		GUI.DragWindow();
	}
	
	public void Show() {
		isActive = true;
	}
	
	public void Hide() {
		isActive = false;
	}

	private void DrawGrid() {
		Color color = Color.white;

		GUI.BeginGroup(graphRect, GUI.skin.box);
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = Color.white;
			style.richText = true;
			
			// X-Axis
			Drawing.DrawLine(hStart, hEnd, color, thickness, false);
			// X-Axis Markers
			for (int i = 0; i < xNumMarkers; i++) {
				float xPos = hStart.x + xUnitLength * (i + 1), yPos = hStart.y - 5;
				// Unit Line
				Drawing.DrawLine(new Vector2(xPos, yPos), new Vector2(xPos, yPos + 10), color, thickness, false);
				// Unit Label
				style.alignment = TextAnchor.UpperCenter;
				if (i < xAxisLabels.Count) {
					GUI.Label(new Rect(xPos - 42, yPos + 10, 80, 70), xAxisLabels[xMin + i], style);
				}
			}
			// X-Axis Label
			style.alignment = TextAnchor.UpperCenter;
			GUI.Label(new Rect(hStart.x + (xAxisLength - 80) / 2, hStart.y + 35, 80, 30), "<color=yellow>" + xTitle + "</color>", style);
			
			// Y-Axis
			Drawing.DrawLine(vStart, vEnd, color, thickness, false);
			// Y-Axis Markers
			for (int i = 0; i <= yNumMarkers; i++) {
				float xPos = vStart.x - 5, yPos = vStart.y - yUnitLength * i;
				// Unit Line
				Drawing.DrawLine(new Vector2(xPos, yPos), new Vector2(xPos + 10, yPos), color, thickness, false);
				// Unit Label
				style.alignment = TextAnchor.UpperRight;
				GUI.Label(new Rect(xPos - 85, yPos - 11, 80, 30), (i * yRange / 5).ToString(), style);
			}
			// Y-Axis Label
			style.alignment = TextAnchor.UpperCenter;
			
			Matrix4x4 matrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(-90, new Vector2(hStart.x - 35, graphRect.height / 2));
			GUI.Label(new Rect(hStart.x - 50, graphRect.height / 2 - 35, 80, 30), "<color=orange>" + yTitle + "</color>", style);
			GUI.matrix = matrix;
			
			// Draw Behind Series Only
			foreach (string label in seriesLabels) {
				if (label == lastSeriesToDraw || excludeList.Contains(label)) {
					continue;
				}
				
				DrawSeries(seriesList[label]);
			}
			// Front Series Drawn Last
			if (!excludeList.Contains(lastSeriesToDraw)) {
				DrawSeries(seriesList[lastSeriesToDraw]);
			}
		GUI.EndGroup();
	}

	private void DrawSeries(Series series) {
		if (Mathf.Abs(series.width - graphRect.width) > 0.1f) {
			series.width = Mathf.Lerp(0, graphRect.width, series.deltaTime += Time.deltaTime * 0.5f);
		}
		
		GUI.BeginGroup(series.GetRect());
			List<float> values = series.values;
			Color color = (series.label == selected) ? Color.white : series.color;
			
			{ // Draw First Point
				string text = ("<color=#" + series.colorHex + ">" + series.label + "</color>" + '\n' + values[xMin].ToString("F2"));
				DrawMarker(series.label, new Rect(hStart.x + xUnitLength - 7, hStart.y - (values[xMin] / yRange * yAxisLength) - 7, 14, 14), color, text);
			}
			
			for (int i = 1; i < Mathf.Min(series.values.Count, xNumMarkers); i++) {
				// Previous Point
				float xPos = hStart.x + xUnitLength * i, yPos = hStart.y - (values[xMin + i - 1] / yRange * yAxisLength);
				// Current Point
				float xPosNext = hStart.x + xUnitLength * (i + 1), yPosNext = hStart.y - (values[xMin + i] / yRange * yAxisLength);
				// Connect the Points by Drawing Line
				Drawing.DrawLine(new Vector2(xPos, yPos), new Vector2(xPosNext, yPosNext), color, 1.5f, true);
				// Draw End Point
				string text = ("<color=#" + series.colorHex + ">" + series.label + "</color>" + '\n' + values[xMin + i].ToString("F2"));
				DrawMarker(series.label, new Rect(xPosNext - 7, yPosNext - 7, 14, 14), color, text);
			}
		GUI.EndGroup();
	}

	private void DrawMarker(string label, Rect rect, Color color, string text) {
		Color temp = GUI.color;

		if (rect.Contains(Event.current.mousePosition)) {
			mouseOverLabels.Add(label);

			if (label == selected) {
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.UpperCenter;
				style.richText = true;

				GUI.Label(new Rect(Event.current.mousePosition.x - 100, Event.current.mousePosition.y - 50, 200, 60), text, style);
			}
		}

		GUI.color = color;
		GUI.DrawTexture(rect, lineMarkerTex);

		GUI.color = temp;
	}

	private void DrawMonthSlider() {
		monthSliderValue = Mathf.RoundToInt(GUI.HorizontalSlider(monthSliderRect, monthSliderValue, 0, Mathf.Max(0, xAxisLabels.Count - xNumMarkers - 1)));
		xMin = (int) monthSliderValue;
		
		if (scrollToMonth != -1) {
			if (Mathf.Abs(monthSliderValue - scrollToMonth) > 0.1f) {
				monthSliderValue = Mathf.Lerp(monthSliderStart, scrollToMonth, monthSliderDT += Time.deltaTime);
			} else {
				scrollToMonth = -1;
			}
		}
	}
	
	public void ShowMonth(int month) {
		monthSliderStart = monthSliderValue;
		monthSliderDT = 0;
		scrollToMonth = Mathf.Clamp(month, 1, xAxisLabels.Count - xNumMarkers) - 1;
	}
	
	public void ShowLastMonth() {
		ShowMonth(xAxisLabels.Count);
	}

	private void DrawLegend() {
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.richText = true;

		Texture2D markerTex = Functions.CreateTexture2D(new Color(0.85f, 0.85f, 0.85f));
		Texture2D markerHoverTex = Functions.CreateTexture2D(Color.white);

		Color temp = GUI.color;

		GUI.BeginGroup(legendRect, GUI.skin.box);
			style.alignment = TextAnchor.UpperCenter;
			GUI.color = Color.yellow;
			GUI.Label(new Rect((legendRect.width - 200) / 2, 10, 200, 30), "Legend", style);
			GUI.color = temp;

			legendScrollPos = GUI.BeginScrollView(legendScrollRect, legendScrollPos, new Rect(0, 0, legendScrollRect.width - 20, seriesLabels.Count * 30));
				style.alignment = TextAnchor.UpperLeft;
				for (int i = 0; i < seriesLabels.Count; i++) {
					Series series = seriesList[seriesLabels[i]];
					string label = series.label;

					Rect itemRect = new Rect(10, i * 30, legendScrollRect.width * 0.8f, 30);

					if (itemRect.Contains(Event.current.mousePosition)) {
						selected = label;
						lastSeriesToDraw = label;
					}
					// Marquee Effect
					if (label == selected) {
						float textWidth = GUI.skin.label.CalcSize(new GUIContent(label)).x;
						float maxViewWidth = itemRect.width - 50;

						if (textWidth > maxViewWidth) {
							series.labelX = Mathf.Lerp(0, maxViewWidth - textWidth, series.labelDT += Time.deltaTime * 0.75f);
						}
					} else {
						series.labelX = 0;
						series.labelDT = 0;
					}

					GUI.BeginGroup(itemRect);
						GUI.color = excludeList.Contains(label) ? Color.gray : series.color;
						GUI.DrawTexture(new Rect(5, 10, 30, 15), (label == selected) ? markerHoverTex : markerTex);
						GUI.color = temp;
			
						GUI.color = excludeList.Contains(label) ? Color.gray : temp;
						style.normal.textColor = (label == selected) ? series.color : Color.white;

						GUI.BeginGroup(new Rect(50, 7, 200, 30));
							GUI.Label(new Rect(series.labelX, 0, 200, 30), label, style);
						GUI.EndGroup();

						GUI.color = temp;

						if (GUI.Button(new Rect(0, 0, itemRect.width, itemRect.height), "", GUIStyle.none)) {
							SetSeriesActive(label, excludeList.Contains(label));
						}
					GUI.EndGroup();
				}
			GUI.EndScrollView();

			if (GUI.Button(new Rect((legendRect.width - 85) / 2, legendRect.height - 33, 85, 25), isLegendContentHidden ? "Show Items" : "Hide Items")) {
				isLegendContentHidden = !isLegendContentHidden;

				if (isLegendContentHidden) {
					foreach (string label in seriesLabels) {
						if (!excludeList.Contains(label)) {
							excludeList.Add(label);
						}
					}
				} else {
					foreach (string label in new List<string>(excludeList)) {
						SetSeriesActive(label, true);
					}
				}
			}
		GUI.EndGroup();
	}

	public void SetSeriesActive(string label, bool active) {
		Series series = seriesList[label];

		if (active) {
			excludeList.Remove(label);

			series.width = 0;
			series.deltaTime = 0;
		} else {
			excludeList.Add(label);
		}
	}

	public void UpdateData() {
		
	}
	
	public IEnumerator UpdateDataRoutine(float time) {
		while (true) {
			UpdateData();
			yield return new WaitForSeconds(time);
		}
	}
}
}