using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ClashSpecies
{
    
    public enum SpeciesType
    {
        PLANT = 0,
        CARNIVORE,
        HERBIVORE,
        OMNIVORE
    }

    public int id;
    public string name;
    public string description;
    public int cost;
    public int hp;
    public int attack;
    public float attackSpeed;
    public float moveSpeed;
    public SpeciesType type;

    public string Stats()
    {
        string stats = "Stats:\n\n" + name;
        stats += "\nType: " + type; 
        stats += "\nCost: " + cost; 
        stats += "\nHealth: " + hp;
        stats += "\nAttack: " + attack; 
        stats += "\nAttack Speed: " + attackSpeed; 
        stats += "\nMovement Speed: " + moveSpeed;

        return stats;
    }
}

[System.Serializable]
public class ClashDefenseConfig
{
    public Player owner;
    public string terrain;
    public Dictionary<ClashSpecies, List<Vector2>> layout = new Dictionary<ClashSpecies, List<Vector2>>();
}

[System.Serializable]
public class ClashAttackConfig
{
    public Player owner;
    public List<ClashSpecies> layout;
}

public class ClashGameManager : MonoBehaviour
{
    public Player currentPlayer;
    public ClashAttackConfig attackConfig;
    public ClashDefenseConfig defenseConfig;
    public ClashDefenseConfig pendingDefenseConfig;
    public List<ClashSpecies> availableSpecies;
    public bool isRunningOnMobile = false;
    public float rotateSpeed = 0;
    public float zoomSpeed = 0;

    public ClashDefenseConfig currentTarget;

    void Awake()
    {
        DontDestroyOnLoad(this);

    }

    void Start()
    {
    }
}
