using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Plant : Organism
{
    private const int evolveChance = 1;
    private const int mutateChance = 1;

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
            toReturn += 8;
        }
        else if (spaceNeeded == SpaceNeeded.MEDIUM && numberInTile > 15) {
            toReturn += 5;
        }
        else if (spaceNeeded == SpaceNeeded.LARGE && numberInTile > 10) {
            toReturn += 3;
        }
        else if (spaceNeeded == SpaceNeeded.EXTRALARGE && numberInTile > 5) {
            toReturn += 1;
        }

        if (water == AmountOfWater.LOW) {
            if (waterNeeded == WaterNeeded.MEDIUM) {
                toReturn += 1;
            }
            else if (waterNeeded == WaterNeeded.HIGH) {
                toReturn += 2;
            }
        }
        else if (water == AmountOfWater.MEDIUM && (waterNeeded == WaterNeeded.LOW || waterNeeded == WaterNeeded.HIGH)) {
            toReturn += 1;
        }
        else if (water == AmountOfWater.HIGH && waterNeeded == WaterNeeded.MEDIUM) {
            if (waterNeeded == WaterNeeded.LOW) {
                toReturn += 2;
            }
            else if (waterNeeded == WaterNeeded.MEDIUM) {
                toReturn += 1;
            }
        }

        if (humidity == Humidity.LOW) {
            if (humidityTol == HumidityTolerance.MEDIUM) {
                toReturn += 1;
            }
            else if (humidityTol == HumidityTolerance.HIGH) {
                toReturn += 2;
            }
        }
        else if (humidity == Humidity.MEDIUM && (humidityTol == HumidityTolerance.LOW || humidityTol == HumidityTolerance.HIGH)) {
            toReturn += 1;
        }
        else if (humidity == Humidity.HIGH && humidityTol == HumidityTolerance.MEDIUM) {
            if (humidityTol == HumidityTolerance.LOW) {
                toReturn += 2;
            }
            else if (humidityTol == HumidityTolerance.MEDIUM) {
                toReturn += 1;
            }
        }

        if (temperature == Temperature.LOW) {
            if (tempTol == TemperatureTolerance.MEDIUM) {
                toReturn += 1;
            }
            else if (tempTol == TemperatureTolerance.HIGH) {
                toReturn += 2;
            }
        }
        else if (temperature == Temperature.MEDIUM && (tempTol == TemperatureTolerance.LOW || tempTol == TemperatureTolerance.HIGH)) {
            toReturn += 1;
        }
        else if (temperature == Temperature.HIGH && tempTol == TemperatureTolerance.MEDIUM) {
            if (tempTol == TemperatureTolerance.LOW) {
                toReturn += 2;
            }
            else if (tempTol == TemperatureTolerance.MEDIUM) {
                toReturn += 1;
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

    public Plant evolve(int numGrown, Biome currentBiome) {
        System.Random random = new System.Random();

        if (random.Next(100) < Plant.evolveChance * numGrown) {
            List<System.Type> parameters = new List<System.Type>() {typeof(WaterNeeded), typeof(SpaceNeeded), typeof(TemperatureTolerance),
                typeof(HumidityTolerance), typeof(bool), typeof(bool)};
            string booleanChecked = "";

            while (parameters.Count != 0) {
                int paramToChange = random.Next(parameters.Count);
                System.Type paramType = parameters[paramToChange];
                parameters.RemoveAt(paramToChange);

                if (paramType.Equals(typeof(WaterNeeded))) {
                    if (this.waterNeeded != WaterNeeded.LOW && (int)this.waterNeeded != (int)currentBiome.amountOfWater - 1) {
                        if ((int)this.waterNeeded > (int)currentBiome.amountOfWater - 1) {
                            GameObject newPlant = (GameObject)Instantiate(this.gameObject,
                                new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
                            Plant p = newPlant.GetComponent<Plant>();

                            p.initialize(this.name + "*", this.spread, this.plantType, this.poisonous, this.waterNeeded - 1,
                                this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                                this.tempTol, this.lifespan);

                            return p;
                        }
                    }
                }
                else if (paramType.Equals(typeof(SpaceNeeded))) {
                    if (this.spaceNeeded != SpaceNeeded.SMALL) {
                        GameObject newPlant = (GameObject)Instantiate(this.gameObject,
                                new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(this.name + "*", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded - 1, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);

                        return p;
                    }
                }
                else if (paramType.Equals(typeof(TemperatureTolerance))) {
                    if ((int)this.tempTol != (int)currentBiome.temperature) {
                        TemperatureTolerance tempTolerance;

                        if (this.tempTol == TemperatureTolerance.LOW || this.tempTol == TemperatureTolerance.HIGH) {
                            tempTolerance = TemperatureTolerance.MEDIUM;
                        }
                        else {
                            if (currentBiome.temperature == Temperature.LOW) {
                                tempTolerance = TemperatureTolerance.LOW;
                            }
                            else {
                                tempTolerance = TemperatureTolerance.HIGH;
                            }
                        }

                        GameObject newPlant = (GameObject)Instantiate(this.gameObject,
                                new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(this.name + "*", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            tempTolerance, this.lifespan);

                        return p;
                    }
                }
                else if (paramType.Equals(typeof(HumidityTolerance))) {
                    if ((int)this.humidityTol != (int)currentBiome.humidity) {
                        HumidityTolerance humidityTolerance;

                        if (this.humidityTol == HumidityTolerance.LOW || this.humidityTol == HumidityTolerance.HIGH) {
                            humidityTolerance = HumidityTolerance.MEDIUM;
                        }
                        else {
                            if (currentBiome.humidity == Humidity.LOW) {
                                humidityTolerance = HumidityTolerance.LOW;
                            }
                            else {
                                humidityTolerance = HumidityTolerance.HIGH;
                            }
                        }

                        GameObject newPlant = (GameObject)Instantiate(this.gameObject,
                                new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(this.name + "*", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, humidityTolerance,
                            this.tempTol, this.lifespan);

                        return p;
                    }
                }
                else if (paramType.Equals(typeof(bool))) {
                    bool evolved = false;
                    bool mountainSurvival = this.canSurviveInMountains;
                    bool desertSurvival = this.canSurviveInDesert;

                    if (booleanChecked.Equals("")) {
                        if (random.Next(2) == 0) {
                            booleanChecked = "Mountain";

                            if (!this.canSurviveInMountains && currentBiome.biomeType == BiomeType.MOUNTAIN) {
                                mountainSurvival = true;
                                evolved = true;
                            }
                        }
                        else {
                            booleanChecked = "Desert";

                            if (!this.canSurviveInDesert && currentBiome.biomeType == BiomeType.DESERT) {
                                desertSurvival = true;
                                evolved = true;
                            }
                        }
                    }
                    else if (booleanChecked.Equals("Mountain")) {
                        if (!this.canSurviveInDesert && currentBiome.biomeType == BiomeType.DESERT) {
                            desertSurvival = true;
                            evolved = true;
                        }
                    }
                    else {
                        if (!this.canSurviveInMountains && currentBiome.biomeType == BiomeType.MOUNTAIN) {
                            mountainSurvival = true;
                            evolved = true;
                        }
                    }

                    if (evolved) {
                        GameObject newPlant = (GameObject)Instantiate(this.gameObject, 
                            new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(this.name + "*", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, mountainSurvival, desertSurvival, this.humidityTol,
                            this.tempTol, this.lifespan);

                        return p;
                    }
                }
            }
        }

        return null;
    }

    public Plant mutate(int numGrown) {
        System.Random random = new System.Random();

        if (random.Next(100) < mutateChance * numGrown) {
            List<System.Type> parameters = new List<System.Type>() {typeof(Spread), typeof(PlantType), typeof(Poisonous), typeof(WaterNeeded), 
            typeof(SpaceNeeded), typeof(TemperatureTolerance), typeof(HumidityTolerance), typeof(bool), typeof(bool)};
            
            int mutateParameter = random.Next(9);
            System.Type paramType = parameters[mutateParameter];

            GameObject newPlant = (GameObject)Instantiate(this.gameObject,
                new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
            Plant p = newPlant.GetComponent<Plant>();

            if (paramType.Equals(typeof(Spread))) {
                List<Spread> spreadValues = Enum.GetValues(typeof(Spread)).Cast<Spread>().ToList();

                spreadValues.RemoveAt((int)this.spread);

                Spread newSpread = spreadValues[random.Next(spreadValues.Count)];

                p.initialize(this.name + "^", newSpread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);
            }
            else if (paramType.Equals(typeof(PlantType))) {
                List<PlantType> plantTypeValues = Enum.GetValues(typeof(PlantType)).Cast<PlantType>().ToList();

                plantTypeValues.RemoveAt((int)this.plantType);

                PlantType newPlantType = plantTypeValues[random.Next(plantTypeValues.Count)];

                p.initialize(this.name + "^", this.spread, newPlantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);
            }
            else if (paramType.Equals(typeof(Poisonous))) {
                List<Poisonous> poisonousValues = Enum.GetValues(typeof(Poisonous)).Cast<Poisonous>().ToList();

                poisonousValues.RemoveAt((int)this.poisonous);

                Poisonous newPoisonous = poisonousValues[random.Next(poisonousValues.Count)];

                p.initialize(this.name + "^", this.spread, this.plantType, newPoisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);
            }
            else if (paramType.Equals(typeof(WaterNeeded))) {
                List<WaterNeeded> waterNeededValues = Enum.GetValues(typeof(WaterNeeded)).Cast<WaterNeeded>().ToList();

                waterNeededValues.RemoveAt((int)this.waterNeeded);

                WaterNeeded newWaterNeeded = waterNeededValues[random.Next(waterNeededValues.Count)];

                p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, newWaterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);
            }
            else if (paramType.Equals(typeof(SpaceNeeded))) {
                List<SpaceNeeded> spaceNeededValues = Enum.GetValues(typeof(SpaceNeeded)).Cast<SpaceNeeded>().ToList();

                spaceNeededValues.RemoveAt((int)this.spaceNeeded);

                SpaceNeeded newSpaceNeeded = spaceNeededValues[random.Next(spaceNeededValues.Count)];

                p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            newSpaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);
            }
            else if (paramType.Equals(typeof(TemperatureTolerance))) {
                List<TemperatureTolerance> tempToleranceValues = 
                    Enum.GetValues(typeof(TemperatureTolerance)).Cast<TemperatureTolerance>().ToList();

                tempToleranceValues.RemoveAt((int)this.tempTol);

                TemperatureTolerance newTempTolerance = tempToleranceValues[random.Next(tempToleranceValues.Count)];

                p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, this.humidityTol,
                            newTempTolerance, this.lifespan);
            }
            else if (paramType.Equals(typeof(HumidityTolerance))) {
                List<HumidityTolerance> humidityToleranceValues =
                    Enum.GetValues(typeof(HumidityTolerance)).Cast<HumidityTolerance>().ToList();

                humidityToleranceValues.RemoveAt((int)this.humidityTol);

                HumidityTolerance newHumidityTolerance = humidityToleranceValues[random.Next(humidityToleranceValues.Count)];

                p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, this.canSurviveInDesert, newHumidityTolerance,
                            this.tempTol, this.lifespan);
            }
            else {
                if (random.Next(2) == 0) {
                    if (this.canSurviveInMountains) {
                        p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, false, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);
                    }
                    else {
                        p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, true, this.canSurviveInDesert, this.humidityTol,
                            this.tempTol, this.lifespan);
                    }
                }
                else {
                    if (this.canSurviveInDesert) {
                        p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, false, this.humidityTol,
                            this.tempTol, this.lifespan);
                    }
                    else {
                        p.initialize(this.name + "^", this.spread, this.plantType, this.poisonous, this.waterNeeded,
                            this.spaceNeeded, this.canSurviveInMountains, true, this.humidityTol,
                            this.tempTol, this.lifespan);
                    }
                }
            }

            return p;
        }
        else {
            return null;
        }
    }

    public override bool Equals(System.Object obj) {
        if (obj == null) {
            return false;
        }

        if (!(obj is Plant)) {
            return false;
        }

        return this.Equals(obj as Plant);
    }

    public bool Equals(Plant p) {
        if ((object)p == null) {
            return false;
        }

        if (this.name.Equals(p.name) && this.spread == p.spread && this.plantType == p.plantType && this.poisonous == p.poisonous &&
            this.waterNeeded == p.waterNeeded && this.spaceNeeded == p.spaceNeeded && this.canSurviveInMountains == p.canSurviveInMountains &&
            this.canSurviveInDesert == p.canSurviveInDesert && this.humidityTol == p.humidityTol && this.tempTol == p.tempTol &&
            this.lifespan == p.lifespan) {
            return true;
        }

        return false;
    }

    public override int GetHashCode() {
        return 0;
    }
}
