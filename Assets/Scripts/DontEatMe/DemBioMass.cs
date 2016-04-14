using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System;

// having issues with database
// going to use arrays or constants for now until I can resolve issue

public class DemDatabase : MonoBehaviour {

	private string speciesName;
    //private int speciesID;
    private float bioMass;
    private string[,] speciesList;


    public DemDatabase()
    {
        // constructor should be pulling info from DB, for now it will just initialize
        speciesName = "";
        //speciesID = 0;
        bioMass = 0;
        string[,] speciesList = new string[1, 2] { { speciesName, bioMass.ToString() }};
    }

    public void setList()
    {
        // SpeciesTable should be establishing connection and pulling these info
        // won't be needed once db works. =/
        speciesList =new string[14,2] {  { "Acacia","2400"}, {"African Marsh Owl","0.355" } ,  { "Baobab","4400"}, {"Bat-Eared Fox","3.35"},
            {"Big Tree","3200" } , { "Black Back Jackal","9.6"}, {"Bohor Reedbuck","50" } , { "Bush Hyrax","4.49"}, 
            {"Dwarf Epauletted Bat","0.029" } , { "Dwarf Mongoose", ""} , { "FruitsAndNectar","20"}, {"GrassAndHerb","40" },  
            { "Kori Buskard","13"},{ "TreesAndShrubs", "40" }  };
    }

    // give the biomass with the given species name
    public float getBiomassBySpecies(string _speciesName)
    {
        for (int i = 0; i < speciesList.GetLength(0); i++)
        {
            if (Equals(_speciesName, speciesList[i, 0]))
            {
                bioMass = Convert.ToInt32(speciesList[i,1]);
                speciesName = speciesList[i, 0];
                return bioMass;
            }
        }
        return -1;  // _speciesName not found in speciesList[,]
    }
	
}
