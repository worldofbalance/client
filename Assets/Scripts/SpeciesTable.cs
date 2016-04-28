using UnityEngine;

using System;
using System.Collections.Generic;

using Mono.Data.Sqlite;

public class SpeciesTable {

	public static Dictionary<int, SpeciesData> speciesList = new Dictionary<int, SpeciesData>();
	public static Dictionary<string, SpeciesData> spNameList = new Dictionary<string, SpeciesData>();
	
	public static void Initialize() {
		SqliteConnection con = new SqliteConnection("URI=file:" + Application.dataPath + "/Database/WoB_DB.db");
		con.Open();

		SqliteCommand cmd = new SqliteCommand(con);
		cmd.CommandText = "" +
			"SELECT *" +
			" FROM `species`";

		cmd.Prepare();
		cmd.ExecuteNonQuery();

		SqliteDataReader reader = cmd.ExecuteReader();
		
		while (reader.Read()) {
			int species_id = reader.GetInt32(0);

			SpeciesData species = new SpeciesData(species_id);
			species.name = reader.GetString(1);
			species.description = reader.GetString(2);
			species.biomass = reader.GetInt32(3);
			species.diet_type = reader.GetInt16(4);
			species.trophic_level = reader.GetFloat(5);
			species.level = reader.GetInt16(6);

			SqliteCommand subcmd = new SqliteCommand(con);
			subcmd.CommandText = "" +
				"SELECT `predator_id`" +
				" FROM `pp_relations`" +
				" WHERE `prey_id` = @species_id";
			subcmd.Parameters.Add(new SqliteParameter("@species_id", species.species_id));
			
			subcmd.Prepare();
			subcmd.ExecuteNonQuery();
			SqliteDataReader subreader = subcmd.ExecuteReader();
			
			while (subreader.Read()) {
				int predator_id = subreader.GetInt32(0);

				if (!species.predatorList.ContainsKey(predator_id)) {
					species.predatorList.Add(predator_id, "");
				}
			}
			
		    subcmd = new SqliteCommand(con);
			subcmd.CommandText = "" +
				"SELECT `prey_id`" +
				" FROM `pp_relations`" +
				" WHERE `predator_id` = @species_id";
			subcmd.Parameters.Add(new SqliteParameter("@species_id", species.species_id));
			
			subcmd.Prepare();
			subcmd.ExecuteNonQuery();
			subreader = subcmd.ExecuteReader();
	
			while (subreader.Read()) {
				int prey_id = subreader.GetInt32(0);

				if (!species.preyList.ContainsKey(prey_id)) {
					species.preyList.Add(subreader.GetInt32(0), "");
				}
			}

			species.name = Functions.NormalizeSpeciesName (species.name);						
			speciesList.Add(species.species_id, species);
			spNameList.Add(species.name, species);
		}

		foreach (SpeciesData species in speciesList.Values) {
			foreach (int predator_id in new List<int>(species.predatorList.Keys)) {
				if (SpeciesTable.speciesList.ContainsKey(predator_id)) {
					species.predatorList[predator_id] = SpeciesTable.speciesList[predator_id].name;
				} else {
					species.predatorList.Remove(predator_id);
				}
			}
			
			foreach (int prey_id in new List<int>(species.preyList.Keys)) {
				if (SpeciesTable.speciesList.ContainsKey(prey_id)) {
					species.preyList[prey_id] = SpeciesTable.speciesList[prey_id].name;
				} else {
					species.preyList.Remove(prey_id);
				}
			}
		}

		reader.Close();
		con.Close();
	}
	
	public static void Update(Dictionary<int, SpeciesData> updateList) {
		SqliteConnection con = new SqliteConnection("URI=file:" + Application.dataPath + "/Database/WoB_DB.db");
		con.Open();

		SqliteCommand cmd = new SqliteCommand(con);
		cmd.CommandText = "" +
			"DELETE FROM `pp_relations`" +
			" WHERE `predator_id` > 0 OR `prey_id` > 0";

		cmd.Prepare();
		cmd.ExecuteNonQuery();

		foreach (KeyValuePair<int, SpeciesData> entry in updateList) {
			int species_id = entry.Key;
			SpeciesData species = entry.Value;

			if (speciesList.ContainsKey(species_id)) { // If Exists, Delete Record
				cmd.CommandText = "" +
					"DELETE FROM `species`" +
					" WHERE `species_id` = @species_id";
				cmd.Parameters.Add(new SqliteParameter("@species_id", species.species_id));

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			cmd.CommandText = "" +
				"INSERT INTO `species` (`species_id`, `name`, `description`, `biomass`, `diet_type`, `trophic_level`, 'level')" +
				" VALUES (@species_id, @name, @description, @biomass, @diet_type, @trophic_level, @level)";
			cmd.Parameters.Add(new SqliteParameter("@species_id", species.species_id));
			cmd.Parameters.Add(new SqliteParameter("@name", species.name));
			cmd.Parameters.Add(new SqliteParameter("@description", species.description));
			cmd.Parameters.Add(new SqliteParameter("@biomass", species.biomass));
			cmd.Parameters.Add(new SqliteParameter("@diet_type", species.diet_type));
			cmd.Parameters.Add(new SqliteParameter("@trophic_level", species.trophic_level));
			cmd.Parameters.Add(new SqliteParameter("@level", species.level));

			cmd.Prepare();
			cmd.ExecuteNonQuery();
			
			foreach (int predator_id in species.predatorList.Keys) {
				cmd.CommandText = "" +
					"INSERT INTO `pp_relations` (`predator_id`, `prey_id`)" +
					" VALUES (@predator_id, @prey_id)";
				cmd.Parameters.Add(new SqliteParameter("@predator_id", predator_id));
				cmd.Parameters.Add(new SqliteParameter("@prey_id", species.species_id));

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}
			
			foreach (int prey_id in species.preyList.Keys) {
				cmd.CommandText = "" +
					"INSERT INTO `pp_relations` (`predator_id`, `prey_id`)" +
					" VALUES (@predator_id, @prey_id)";
				cmd.Parameters.Add(new SqliteParameter("@predator_id", species.species_id));
				cmd.Parameters.Add(new SqliteParameter("@prey_id", prey_id));

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}
		}

		con.Close();
	}

	//get species based on CSV name - need to strip nodeIdx info
	public static SpeciesData GetSpecies (string name)
	{
		string spName = GetSpeciesName (name);
		//check for name
		if (spNameList.ContainsKey (spName)) {
			return spNameList [spName];
		} else {
			return null;
		}
	}
	
	//get species name based on CSV name - need to strip nodeIdx info
	public static string GetSpeciesName (string name)
	{
		string spName = null;
		if (name != null) {
			//strip node index
			int nodeIdx = name.IndexOf ("[");
			if (nodeIdx != Constants.ID_NOT_SET) {
				spName = name.Remove (nodeIdx).Trim ();
			}
		}
		return spName;
	}
}