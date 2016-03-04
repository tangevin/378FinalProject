using UnityEngine;
using System.Collections;

public class Plant : Organism
{
    public string name { get; private set; }
    public Spread spread { get; private set; }
    public PlantType plantType { get; private set; }
    public Poisonous poisonous { get; private set; }
    public WaterNeeded waterNeeded { get; private set; }
    public SpaceNeeded spaceNeeded { get; private set; }
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

    public void initialize(string name, Spread spread, PlantType plantType, Poisonous poisonous, WaterNeeded waterNeeded, 
        SpaceNeeded spaceNeeded, bool canSurviveInMountains, bool canSurviveInDeserts, HumidityTolerance humidityTolerance, 
        TemperatureTolerance tempTolerance, Lifespan lifespan) 
    {
        this.name = name;
        this.spread = spread;
        this.plantType = plantType;
        this.poisonous = poisonous;
        this.waterNeeded = waterNeeded;
        this.spaceNeeded = spaceNeeded;
        this.canSurviveInMountains = canSurviveInMountains;
        this.canSurviveInDesert = canSurviveInDesert;

        base.initialize(humidityTolerance, tempTolerance, lifespan);
    }

    public int checkInTileGrowth(int numberInTile, AmountOfWater water, Humidity humidity, Temperature temperature)
    {
        int toReturn = 0;

        if (spaceNeeded == SpaceNeeded.SMALL && numberInTile < 20) {
            toReturn += 5;
        }
        else if (spaceNeeded == SpaceNeeded.MEDIUM && numberInTile < 15) {
            toReturn += 3;
        }
        else if (spaceNeeded == SpaceNeeded.LARGE && numberInTile < 10) {
            toReturn += 2;
        }
        else if (spaceNeeded == SpaceNeeded.EXTRALARGE && numberInTile < 5) {
            toReturn += 1;
        }

        if (water == AmountOfWater.LOW && waterNeeded == WaterNeeded.LOW) {
            toReturn += 1;
        }
        else if (water == AmountOfWater.MEDIUM && waterNeeded == WaterNeeded.MEDIUM) {
            toReturn += 1;
        }
        else if (water == AmountOfWater.HIGH && waterNeeded == WaterNeeded.HIGH) {
            toReturn += 1;
        }

        if (humidity == Humidity.LOW && humidityTol == HumidityTolerance.LOW) {
            toReturn += 1;
        }
        else if (humidity == Humidity.MEDIUM && humidityTol == HumidityTolerance.MEDIUM) {
            toReturn += 1;
        }
        else if (humidity == Humidity.HIGH && humidityTol == HumidityTolerance.HIGH) {
            toReturn += 1;
        }

        if (temperature == Temperature.LOW && tempTol == TemperatureTolerance.LOW) {
            toReturn += 1;
        }
        else if (temperature == Temperature.MEDIUM && tempTol == TemperatureTolerance.MEDIUM) {
            toReturn += 1;
        }
        else if (temperature == Temperature.HIGH && tempTol == TemperatureTolerance.HIGH) {
            toReturn += 1;
        }

        return toReturn;
    }

    public int checkInTileDeath(int numberInTile, AmountOfWater water, Humidity humidity, Temperature temperature) {
        int toReturn = 0;

        if (spaceNeeded == SpaceNeeded.SMALL && numberInTile > 20) {
            toReturn -= 8;
        }
        else if (spaceNeeded == SpaceNeeded.MEDIUM && numberInTile > 15) {
            toReturn -= 5;
        }
        else if (spaceNeeded == SpaceNeeded.LARGE && numberInTile > 10) {
            toReturn -= 3;
        }
        else if (spaceNeeded == SpaceNeeded.EXTRALARGE && numberInTile > 5) {
            toReturn -= 1;
        }

        if (water == AmountOfWater.LOW) {
            if (waterNeeded == WaterNeeded.MEDIUM) {
                toReturn -= 1;
            }
            else if (waterNeeded == WaterNeeded.HIGH) {
                toReturn -= 2;
            }
        }
        else if (water == AmountOfWater.MEDIUM && (waterNeeded == WaterNeeded.LOW || waterNeeded == WaterNeeded.HIGH)) {
            toReturn -= 1;
        }
        else if (water == AmountOfWater.HIGH && waterNeeded == WaterNeeded.MEDIUM) {
            if (waterNeeded == WaterNeeded.LOW) {
                toReturn -= 2;
            }
            else if (waterNeeded == WaterNeeded.MEDIUM) {
                toReturn -= 1;
            }
        }

        if (humidity == Humidity.LOW) {
            if (humidityTol == HumidityTolerance.MEDIUM) {
                toReturn -= 1;
            }
            else if (humidityTol == HumidityTolerance.HIGH) {
                toReturn -= 2;
            }
        }
        else if (humidity == Humidity.MEDIUM && (humidityTol == HumidityTolerance.LOW || humidityTol == HumidityTolerance.HIGH)) {
            toReturn -= 1;
        }
        else if (humidity == Humidity.HIGH && humidityTol == HumidityTolerance.MEDIUM) {
            if (humidityTol == HumidityTolerance.LOW) {
                toReturn -= 2;
            }
            else if (humidityTol == HumidityTolerance.MEDIUM) {
                toReturn -= 1;
            }
        }

        if (temperature == Temperature.LOW) {
            if (tempTol == TemperatureTolerance.MEDIUM) {
                toReturn -= 1;
            }
            else if (tempTol == TemperatureTolerance.HIGH) {
                toReturn -= 2;
            }
        }
        else if (temperature == Temperature.MEDIUM && (tempTol == TemperatureTolerance.LOW || tempTol == TemperatureTolerance.HIGH)) {
            toReturn -= 1;
        }
        else if (temperature == Temperature.HIGH && tempTol == TemperatureTolerance.MEDIUM) {
            if (tempTol == TemperatureTolerance.LOW) {
                toReturn -= 2;
            }
            else if (tempTol == TemperatureTolerance.MEDIUM) {
                toReturn -= 1;
            }
        }

        return toReturn;
    }

    public bool checkCanSpread(int numberInTile)
    {
        bool toReturn = false;

        if (spaceNeeded == SpaceNeeded.SMALL && numberInTile >= 20) {
            toReturn = true;
        }
        else if (spaceNeeded == SpaceNeeded.MEDIUM && numberInTile >= 15) {
            toReturn = true;
        }
        else if (spaceNeeded == SpaceNeeded.LARGE && numberInTile >= 10) {
            toReturn = true;
        }
        else if (spaceNeeded == SpaceNeeded.EXTRALARGE && numberInTile >= 5) {
            toReturn = true;
        }

        return toReturn;
    }
}
