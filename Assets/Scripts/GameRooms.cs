using UnityEngine;
using System;
using System.Collections.Generic;

public class Room {
	public int id = -1;
	public int game_id = -1;
	public List<int> players = new List<int>();
	public string host;

	public int numPlayers() {
		return players.Count;
	}

	public string status() {
		return players.Count == 2 ? "In game" : "Waiting";
	}

	public void addPlayer(int userid) {
		players.Add (userid);
	}

	public void removePlayer(int userid) {
		players.Remove (userid);
	}

	public bool containsPlayer(int usrid) {
		return players.Contains (usrid);
	}

	public static string getGameName(int type) {
        if (type == Constants.MINIGAME_CARDS_OF_WILD) {
            return "Cards of Wild";
        } else if (type == Constants.MINIGAME_CLASH_OF_SPECIES) {
            return "Clash of Species";
        } else if (type == Constants.MINIGAME_DONT_EAT_ME) {
            return "Don't Eat Me";
        } else if (type == Constants.MINIGAME_RUNNING_RHINO) {
            return "Running Rhino";
        } else if (type == Constants.MINIGAME_SEA_DIVIDED) {
            return "Sea Divided";
        } else {
			return "Unknown Game";
		}
	}
}

public class RoomManager {
	private static RoomManager instance;

	private Dictionary<int,Room> rooms = new Dictionary<int,Room>();

	public static RoomManager getInstance() {
		if (instance == null) {
			instance = new RoomManager();
		}
		return instance;
	}

	public Dictionary<int,Room> getRooms() {
		return rooms;
	}

	public Room getRoom(int id) {
		return rooms [id];
	}

	public Room addRoom(int id, int gameID) {
		var room = new Room ();
		rooms [id] = room;

		room.id = id;
		room.game_id = gameID;
		return room;
	}

	public void removePlayer(int player) {
		foreach (var r in rooms) {
			if(r.Value.containsPlayer(player)) {
				r.Value.removePlayer(player);
				if(r.Value.numPlayers() == 0) {
					rooms.Remove(r.Key);
				}
				return;
			}
		}
		Debug.Log("Can't find the player [player id=" + player + "]");
	}

	public void clear() {
		rooms.Clear ();
	}
}