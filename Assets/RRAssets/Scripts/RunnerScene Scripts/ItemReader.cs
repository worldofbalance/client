using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RR {
	public class ItemReader : MonoBehaviour {
		public static Dictionary<string, string> ReadFile(string filename) {
			Dictionary<string, string> items = new Dictionary<string, string>();
	
			try {
				TextAsset txt = (TextAsset) Resources.Load("ItemLocationFiles/1", typeof(TextAsset));
				string content = txt.text;
				DebugConsole.Log("ItemReader content: " + content);
				string[] sa = content.Split('\n');
				try {
					int num = int.Parse(sa[0]);
					for (int i = 1; i <= num; i++) {
						string speciesId = sa[i*2-1];
						string speciesX = sa[i*2];
						items.Add(speciesX, speciesId);
					}
				} catch {
					Debug.Log("file format error!!!!");
				}
				
			} catch {
				Debug.Log("file read error!!!!");
			}
			return items;
		}
	}
}