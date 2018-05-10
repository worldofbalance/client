using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SpeciesType = ClashSpecies.SpeciesType;

public class ClashDefenseShop : MonoBehaviour
{


	public List<string> terrainNames;
	private ClashGameManager manager;

	public GridLayoutGroup carnivoreGroup;
	public GridLayoutGroup herbivoreGroup;
	public GridLayoutGroup omnivoreGroup;
	public GridLayoutGroup plantGroup;
	public GridLayoutGroup terrainGroup;

	public HorizontalLayoutGroup selectedGroup;
	public HorizontalLayoutGroup selectedTerrain;

	public Image previewImage;
	public Text descriptionText;
	public Text statsText;
	public Button infoButton;
	public bool textSwitch;
	//true for description text; false for stats text

	public GameObject shopElementPrefab;
	public GameObject selectedUnitPrefab;
	public GameObject selectedTerrainPrefab;

	public Text cancelButton;
	public GameObject errorCanvas;
	public Text errorMessage;

	void Awake ()
	{
		manager = GameObject.Find ("MainObject").GetComponent<ClashGameManager> ();
		foreach (var species in manager.availableSpecies) {
			var item = (Instantiate (shopElementPrefab) as GameObject).GetComponent<ClashShopItem> ();
			item.displayText.text = species.name;

			var texture = Resources.Load<Texture2D> ("Images/" + species.name);
			item.displayImage.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));

			item.addButton.onClick.AddListener (() => {
				// If item exists in the list already, don't add.
				foreach (ClashSelectedUnit existing in selectedGroup.GetComponentsInChildren<ClashSelectedUnit>()) {
					if (existing.label.text == item.displayText.text) {

						return;
					}
				}

				// If the user has already selected 5 species, don't add.
				if (selectedGroup.transform.childCount == 5) {
					errorCanvas.SetActive (true);
					errorMessage.text = "A total of 5 units can be selected";
					return;
				}

				// Instantiated a selected item prefab and configure it.
				var selected = (Instantiate (selectedUnitPrefab) as GameObject).GetComponent<ClashSelectedUnit> ();
				selected.transform.SetParent (selectedGroup.transform);
				selected.image.sprite = item.displayImage.sprite;
				selected.transform.localScale = Vector3.one;
				selected.label.text = item.displayText.text;
				selected.remove.onClick.AddListener (() => {
					Destroy (selected.gameObject);
				});
			});

			var description = species.description;
			var stats = species.Stats ();
			item.previewButton.onClick.AddListener (() => {
				previewImage.sprite = item.displayImage.sprite;
				descriptionText.text = description;
				statsText.text = stats;

				infoButton.GetComponentInChildren<Text> ().text = "Get Stat Info";
				descriptionText.gameObject.SetActive (true);
				statsText.gameObject.SetActive (false);
				infoButton.interactable = true;
				textSwitch = true;

				infoButton.onClick.RemoveAllListeners ();
				infoButton.onClick.AddListener (() => {
					textSwitch = !textSwitch;
					if (textSwitch) {
						infoButton.GetComponentInChildren<Text> ().text = "Get Stat Info";
						descriptionText.gameObject.SetActive (true);
						statsText.gameObject.SetActive (false);
					} else {
						infoButton.GetComponentInChildren<Text> ().text = "Get Description";
						descriptionText.gameObject.SetActive (false);
						statsText.gameObject.SetActive (true);
					}
				});
			}); 

			switch (species.type) {
			case SpeciesType.CARNIVORE:
				item.transform.SetParent (carnivoreGroup.transform);
				break;
			case SpeciesType.HERBIVORE:
				item.transform.SetParent (herbivoreGroup.transform);
				break;
			case SpeciesType.OMNIVORE: 
				item.transform.SetParent (omnivoreGroup.transform);
				break;
			case SpeciesType.PLANT: 
				item.transform.SetParent (plantGroup.transform);
				break;
			default:
				break;
			}

			item.transform.position = new Vector3 (item.transform.position.x, item.transform.position.y, 0.0f);
			item.transform.localScale = new Vector3(1, 1, 1);
		}

		// Setup the terrain items list.
		List<GameObject> terrains = new List<GameObject> (Resources.LoadAll<GameObject> ("Prefabs/ClashOfSpecies/Terrains"));
		foreach (GameObject t in terrains) {
			terrainNames.Add (t.name);
			var item = (Instantiate (shopElementPrefab) as GameObject).GetComponent<ClashShopItem> ();

			var texture = Resources.Load<Texture2D> ("Images/ClashOfSpecies/" + t.name);
			item.displayImage.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
			item.displayText.text = t.name;

			item.addButton.onClick.AddListener (() => {
				// If a terrain has already been selected, destroy it first.
				var existing = selectedTerrain.GetComponentInChildren<ClashSelectedUnit> ();
				if (existing != null) {
					Destroy (existing.gameObject);
				}

				// Add the newly selected terrain object.
				var selected = (Instantiate (selectedUnitPrefab) as GameObject).GetComponent<ClashSelectedUnit> ();
				selected.transform.SetParent (selectedTerrain.transform);
				selected.transform.position = new Vector3 (item.transform.position.x, item.transform.position.y, 0.0f);
				selected.transform.localScale = Vector3.one;
				selected.image.sprite = item.displayImage.sprite;
				selected.label.text = item.displayText.text;
				selected.remove.onClick.AddListener (() => {
					Destroy (selected.gameObject);
				});
			});

			item.previewButton.onClick.AddListener (() => {
				previewImage.sprite = item.displayImage.sprite;
				descriptionText.text = "Terrain";

				descriptionText.gameObject.SetActive (true);
				statsText.gameObject.SetActive (false);
				infoButton.GetComponentInChildren<Text> ().text = "Description";
				infoButton.interactable = false;
				textSwitch = true;
				infoButton.GetComponentInChildren<Text> ().text = "Description";
			});

			item.transform.SetParent (terrainGroup.transform);
			item.transform.localScale = new Vector3 (0.75f, 1.25f, 0.0f);
			item.transform.position = new Vector3 (item.transform.position.x, item.transform.position.y, 0.0f);
		}

		// Populate the selected unit and terrain lists if the user has a pending or existing defense configuration.
		var existingConfig = (manager.pendingDefenseConfig == null) ? 
			(manager.defenseConfig == null) ? null : manager.defenseConfig : manager.pendingDefenseConfig;

		if (existingConfig != null) {
			foreach (var pair in existingConfig.layout) {
				var selected = (Instantiate (selectedUnitPrefab) as GameObject).GetComponent<ClashSelectedUnit> ();
				var texture = Resources.Load<Texture2D> ("Images/" + pair.Key.name);

				selected.transform.SetParent (selectedGroup.transform);
				selected.image.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
				selected.transform.localScale = Vector3.one;
				selected.label.text = pair.Key.name;
				selected.remove.onClick.AddListener (() => {
					Destroy (selected.gameObject);
				});
			}

			if (existingConfig.terrain != null && !existingConfig.terrain.Equals ("")) {
				var terrainItem = (Instantiate (selectedUnitPrefab) as GameObject).GetComponent<ClashSelectedUnit> ();
				var terrainTexture = Resources.Load<Texture2D> ("Images/ClashOfSpecies/" + existingConfig.terrain);

				terrainItem.transform.SetParent (selectedTerrain.transform);
				terrainItem.transform.position = new Vector3 (terrainItem.transform.position.x, terrainItem.transform.position.y, 0.0f);
				terrainItem.transform.localScale = Vector3.one;
				terrainItem.image.sprite = Sprite.Create (terrainTexture, new Rect (0, 0, terrainTexture.width, terrainTexture.height), Vector2.one * 0.5f);
				terrainItem.label.text = existingConfig.terrain;
				terrainItem.remove.onClick.AddListener (() => {
					Destroy (terrainItem.gameObject);
				});
			}

			cancelButton.text = "Return to Main\n(Cancel)";
		}
	}

	// Use this for initialization
	void Start ()
	{
	}

	public void PlaceDefense ()
	{
		if (selectedTerrain.transform.childCount == 1 && selectedGroup.transform.childCount == 5) {
			manager.pendingDefenseConfig = new ClashDefenseConfig ();
			manager.pendingDefenseConfig.owner = manager.currentPlayer;
			manager.pendingDefenseConfig.terrain = selectedTerrain.GetComponentInChildren<ClashSelectedUnit> ().label.text;
			manager.pendingDefenseConfig.layout = new Dictionary<ClashSpecies, List<Vector2>> ();
			foreach (ClashSelectedUnit csu in selectedGroup.GetComponentsInChildren<ClashSelectedUnit>()) {
				var species = manager.availableSpecies.Single (x => x.name == csu.label.text);
				manager.pendingDefenseConfig.layout.Add (species, new List<Vector2> ());
			}
			Game.LoadScene ("ClashDefense");
		} else {
			errorCanvas.SetActive (true);
			if (selectedTerrain.transform.childCount != 1) {
				errorMessage.text += "A terrain needs to be selected\n\n";
			}
			if (selectedGroup.transform.childCount != 5) {
				errorMessage.text += "5 unique units needs to be selected";
			}
		}
	}

	public void BackToPreviousScene ()
	{
		if (manager.defenseConfig == null || manager.defenseConfig.layout.Count != 5) {
			Destroy (manager); 
			Game.LoadScene ("World");
		} else {
			Game.LoadScene ("ClashMain");
		}
	}

	public void ConfirmError ()
	{
		errorMessage.text = "";
		errorCanvas.SetActive (false);
	}
}