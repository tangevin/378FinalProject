using UnityEngine;
using System.Collections;

public class Plant : Organism
{
    public Spread spread { get; private set; }
    public PlantType plantType { get; private set; }
    public Poisonous poisonous { get; private set; }
    public WaterNeeded waterNeeded { get; private set; }
    public bool canSurviveInMountains { get; private set; }
    public bool canSurviveInDesert { get; private set; }

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
