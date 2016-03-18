using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Plant : Organism
{
    private const int evolveChance = 1;
    private const int evolveOutOf = 1000;
    private const int mutateChance = 1;
    private const int mutateOutOf = 1000;

    new public string name;
    public Spread spread { get; private set; }
    public PlantType plantType { get; private set; }
    public Poisonous poisonous { get; private set; }
    public WaterNeeded waterNeeded { get; private set; }
    public SpaceNeeded spaceNeeded { get; private set; }
    public bool canSurviveInMountains { get; private set; }
    public bool canSurviveInDesert { get; private set; }

    public Sprite typeSprite { get; private set; }

    public void initialize(string name, Spread spread, PlantType plantType, Poisonous poisonous, WaterNeeded waterNeeded, 
        SpaceNeeded spaceNeeded, bool canSurviveInMountains, bool canSurviveInDesert, HumidityTolerance humidityTolerance, 
        TemperatureTolerance tempTolerance, Lifespan lifespan, Sprite dispImage) 
    {
        this.name = name;
        this.spread = spread;
        this.plantType = plantType;
        this.poisonous = poisonous;
        this.waterNeeded = waterNeeded;
        this.spaceNeeded = spaceNeeded;
        this.canSurviveInMountains = canSurviveInMountains;
        this.canSurviveInDesert = canSurviveInDesert;

        typeSprite = dispImage;

        initialize(humidityTolerance, tempTolerance, lifespan);
    }

    public int checkInTileGrowth(int numberInTile, AmountOfWater water, Humidity humidity, Temperature temperature)
    {
        int toReturn = 0;

        if (spaceNeeded == SpaceNeeded.SMALL && numberInTile < 20)
        {
            toReturn += 5;
        }
        else if (spaceNeeded == SpaceNeeded.MEDIUM && numberInTile < 15)
        {
            toReturn += 3;
        }
        else if (spaceNeeded == SpaceNeeded.LARGE && numberInTile < 10)
        {
            toReturn += 2;
        }
        else if (spaceNeeded == SpaceNeeded.EXTRALARGE && numberInTile < 5)
        {
            toReturn += 1;
        }

        if ((int)water == (int)waterNeeded)
        {
            toReturn += 1;
        }

        if ((int)humidity == (int)humidityTol)
        {
            toReturn += 1;
        }

        if ((int)temperature == (int)tempTol)
        {
            toReturn += 1;
        }

        return toReturn;
    }

    public int checkInTileDeath(int numberInTile, AmountOfWater water, Humidity humidity, Temperature temperature)
    {
        int toReturn = 0;

        if (spaceNeeded == SpaceNeeded.SMALL && numberInTile > 20)
        {
            toReturn += 8;
        }
        else if (spaceNeeded == SpaceNeeded.MEDIUM && numberInTile > 15)
        {
            toReturn += 5;
        }
        else if (spaceNeeded == SpaceNeeded.LARGE && numberInTile > 10)
        {
            toReturn += 3;
        }
        else if (spaceNeeded == SpaceNeeded.EXTRALARGE && numberInTile > 5)
        {
            toReturn += 1;
        }

        toReturn += Mathf.Abs((int)water - (int)waterNeeded);

        toReturn += Mathf.Abs((int)humidity - (int)humidityTol);

        toReturn += Mathf.Abs((int)temperature - (int)tempTol);

        return toReturn;
    }

    public bool checkCanSpread(int numberInTile)
    {
        return ((spaceNeeded == SpaceNeeded.SMALL && numberInTile >= 20) ||
                (spaceNeeded == SpaceNeeded.MEDIUM && numberInTile >= 15) ||
                (spaceNeeded == SpaceNeeded.LARGE && numberInTile >= 10) ||
                (spaceNeeded == SpaceNeeded.EXTRALARGE && numberInTile >= 5));
    }

    public Plant evolve(int numGrown, Biome currentBiome)
    {
        System.Random random = new System.Random();

        if (random.Next(evolveOutOf) < evolveChance * numGrown)
        {
            List<System.Type> parameters = new List<System.Type>() {typeof(WaterNeeded), typeof(SpaceNeeded), typeof(TemperatureTolerance),
                typeof(HumidityTolerance), typeof(bool), typeof(bool)};
            string booleanChecked = "";

            while (parameters.Count != 0)
            {
                int paramToChange = random.Next(parameters.Count);
                Type paramType = parameters[paramToChange];
                parameters.RemoveAt(paramToChange);

                if (paramType.Equals(typeof(WaterNeeded)))
                {
                    if (waterNeeded != WaterNeeded.LOW && (int)waterNeeded != (int)currentBiome.amountOfWater - 1)
                    {
                        if ((int)waterNeeded > (int)currentBiome.amountOfWater - 1)
                        {
                            GameObject newPlant = (GameObject)Instantiate(gameObject,
                                new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                            Plant p = newPlant.GetComponent<Plant>();

                            p.initialize(name, spread, plantType, poisonous, waterNeeded - 1,
                                spaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTol,
                                tempTol, lifespan, typeSprite);

                            return p;
                        }
                    }
                }
                else if (paramType.Equals(typeof(SpaceNeeded)))
                {
                    if (spaceNeeded != SpaceNeeded.SMALL)
                    {
                        GameObject newPlant = (GameObject)Instantiate(gameObject,
                                new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(name, spread, plantType, poisonous, waterNeeded,
                            spaceNeeded - 1, canSurviveInMountains, canSurviveInDesert, humidityTol,
                            tempTol, lifespan, typeSprite);

                        return p;
                    }
                }
                else if (paramType.Equals(typeof(TemperatureTolerance)))
                {
                    if ((int)tempTol != (int)currentBiome.temperature)
                    {
                        TemperatureTolerance tempTolerance;

                        if (tempTol != TemperatureTolerance.MEDIUM)
                        {
                            tempTolerance = TemperatureTolerance.MEDIUM;
                        }
                        else if (currentBiome.temperature == Temperature.LOW)
                        {
                            tempTolerance = TemperatureTolerance.LOW;
                        }
                        else
                        {
                            tempTolerance = TemperatureTolerance.HIGH;
                        }

                        GameObject newPlant = (GameObject)Instantiate(gameObject,
                                new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(name, spread, plantType, poisonous, waterNeeded,
                            spaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTol,
                            tempTolerance, lifespan, typeSprite);

                        return p;
                    }
                }
                else if (paramType.Equals(typeof(HumidityTolerance)))
                {
                    if ((int)humidityTol != (int)currentBiome.humidity)
                    {
                        HumidityTolerance humidityTolerance;

                        if (humidityTol != HumidityTolerance.MEDIUM)
                        {
                            humidityTolerance = HumidityTolerance.MEDIUM;
                        }
                        else if (currentBiome.humidity == Humidity.LOW)
                        {
                            humidityTolerance = HumidityTolerance.LOW;
                        }
                        else
                        {
                            humidityTolerance = HumidityTolerance.HIGH;
                        }

                        GameObject newPlant = (GameObject)Instantiate(gameObject,
                                new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(name, spread, plantType, poisonous, waterNeeded,
                            spaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTolerance,
                            tempTol, lifespan, typeSprite);

                        return p;
                    }
                }
                else if (paramType.Equals(typeof(bool)))
                {
                    bool evolved = false;
                    bool mountainSurvival = canSurviveInMountains;
                    bool desertSurvival = canSurviveInDesert;

                    if (booleanChecked.Equals(""))
                    {
                        if (random.Next(2) == 0)
                        {
                            booleanChecked = "Mountain";

                            if (!canSurviveInMountains && currentBiome.biomeType == BiomeType.MOUNTAIN)
                            {
                                mountainSurvival = true;
                                evolved = true;
                            }
                        }
                        else
                        {
                            booleanChecked = "Desert";

                            if (!canSurviveInDesert && currentBiome.biomeType == BiomeType.DESERT)
                            {
                                desertSurvival = true;
                                evolved = true;
                            }
                        }
                    }
                    else if (booleanChecked.Equals("Mountain"))
                    {
                        if (!canSurviveInDesert && currentBiome.biomeType == BiomeType.DESERT)
                        {
                            desertSurvival = true;
                            evolved = true;
                        }
                    }
                    else
                    {
                        if (!canSurviveInMountains && currentBiome.biomeType == BiomeType.MOUNTAIN)
                        {
                            mountainSurvival = true;
                            evolved = true;
                        }
                    }

                    if (evolved)
                    {
                        GameObject newPlant = (GameObject)Instantiate(gameObject, 
                            new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        p.initialize(name, spread, plantType, poisonous, waterNeeded,
                            spaceNeeded, mountainSurvival, desertSurvival, humidityTol,
                            tempTol, lifespan, typeSprite);

                        return p;
                    }
                }
            }
        }

        return null;
    }

    public Plant mutate(int numGrown)
    {
        System.Random random = new System.Random();

        if (random.Next(mutateOutOf) < mutateChance * numGrown)
        {
            List<System.Type> parameters = new List<System.Type>() {typeof(Spread), typeof(Poisonous), typeof(WaterNeeded), 
            typeof(SpaceNeeded), typeof(TemperatureTolerance), typeof(HumidityTolerance), typeof(bool), typeof(bool)};
            
            int mutateParameter = random.Next(9);
            System.Type paramType = parameters[mutateParameter];

            GameObject newPlant = (GameObject)Instantiate(gameObject,
                new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            Plant p = newPlant.GetComponent<Plant>();

            if (paramType.Equals(typeof(Spread)))
            {
                List<Spread> spreadValues = Enum.GetValues(typeof(Spread)).Cast<Spread>().ToList();

                spreadValues.RemoveAt((int)spread - 1);

                Spread newSpread = spreadValues[random.Next(spreadValues.Count)];

                p.initialize(name, newSpread, plantType, poisonous, waterNeeded,
                            spaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTol,
                            tempTol, lifespan, typeSprite);
            }
            else if (paramType.Equals(typeof(Poisonous)))
            {
                List<Poisonous> poisonousValues = Enum.GetValues(typeof(Poisonous)).Cast<Poisonous>().ToList();

                switch (poisonous) {
                    case Poisonous.NONE:
                        poisonousValues.RemoveAt(0);
                        break;
                    case Poisonous.MINOR:
                        poisonousValues.RemoveAt(1);
                        break;
                    case Poisonous.MAJOR:
                        poisonousValues.RemoveAt(2);
                        break;
                    case Poisonous.DEADLY:
                        poisonousValues.RemoveAt(3);
                        break;
                }

                Poisonous newPoisonous = poisonousValues[random.Next(poisonousValues.Count)];

                p.initialize(name, spread, plantType, newPoisonous, waterNeeded,
                            spaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTol,
                            tempTol, lifespan, typeSprite);
            }
            else if (paramType.Equals(typeof(WaterNeeded)))
            {
                List<WaterNeeded> waterNeededValues = Enum.GetValues(typeof(WaterNeeded)).Cast<WaterNeeded>().ToList();

                waterNeededValues.RemoveAt((int)waterNeeded - 1);

                WaterNeeded newWaterNeeded = waterNeededValues[random.Next(waterNeededValues.Count)];

                p.initialize(name, spread, plantType, poisonous, newWaterNeeded,
                            spaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTol,
                            tempTol, lifespan, typeSprite);
            }
            else if (paramType.Equals(typeof(SpaceNeeded)))
            {
                List<SpaceNeeded> spaceNeededValues = Enum.GetValues(typeof(SpaceNeeded)).Cast<SpaceNeeded>().ToList();

                spaceNeededValues.RemoveAt((int)spaceNeeded);

                SpaceNeeded newSpaceNeeded = spaceNeededValues[random.Next(spaceNeededValues.Count)];

                p.initialize(name, spread, plantType, poisonous, waterNeeded,
                            newSpaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTol,
                            tempTol, lifespan, typeSprite);
            }
            else if (paramType.Equals(typeof(TemperatureTolerance)))
            {
                List<TemperatureTolerance> tempToleranceValues = 
                    Enum.GetValues(typeof(TemperatureTolerance)).Cast<TemperatureTolerance>().ToList();

                tempToleranceValues.RemoveAt((int)tempTol - 1);

                TemperatureTolerance newTempTolerance = tempToleranceValues[random.Next(tempToleranceValues.Count)];

                p.initialize(name, spread, plantType, poisonous, waterNeeded,
                            spaceNeeded, canSurviveInMountains, canSurviveInDesert, humidityTol,
                            newTempTolerance, lifespan, typeSprite);
            }
            else if (paramType.Equals(typeof(HumidityTolerance)))
            {
                List<HumidityTolerance> humidityToleranceValues =
                    Enum.GetValues(typeof(HumidityTolerance)).Cast<HumidityTolerance>().ToList();

                humidityToleranceValues.RemoveAt((int)humidityTol - 1);

                HumidityTolerance newHumidityTolerance = humidityToleranceValues[random.Next(humidityToleranceValues.Count)];

                p.initialize(name, spread, plantType, poisonous, waterNeeded,
                    spaceNeeded, canSurviveInMountains, canSurviveInDesert, newHumidityTolerance,
                    tempTol, lifespan, typeSprite);
            }
            else
            {
                if (random.Next(2) == 0)
                {
                    p.initialize(name, spread, plantType, poisonous, waterNeeded,
                        spaceNeeded, !canSurviveInMountains, canSurviveInDesert, humidityTol,
                        tempTol, lifespan, typeSprite);
                }
                else
                {
                    p.initialize(name, spread, plantType, poisonous, waterNeeded,
                        spaceNeeded, canSurviveInMountains, !canSurviveInDesert, humidityTol,
                        tempTol, lifespan, typeSprite);
                }
            }

            return p;
        }
        else
        {
            return null;
        }
    }

    public override bool Equals(System.Object obj)
    {
        return obj != null && obj is Plant && Equals(obj as Plant);
    }

    public bool Equals(Plant p)
    {
        return p != null && name.Equals(p.name) && spread == p.spread && plantType == p.plantType && poisonous == p.poisonous &&
            waterNeeded == p.waterNeeded && spaceNeeded == p.spaceNeeded && canSurviveInMountains == p.canSurviveInMountains &&
            canSurviveInDesert == p.canSurviveInDesert && humidityTol == p.humidityTol && tempTol == p.tempTol &&
            lifespan == p.lifespan;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
