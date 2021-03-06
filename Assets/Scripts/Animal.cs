﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Animal : Organism
{
    public static int nextID = 0;
    public static Dictionary<string, int> namesToIDs = new Dictionary<string,int>();

    private SpriteRenderer sprite;
    private int lowGes = 2;
    private int highGes = 20;
    private int hungerMax = 100;
    private int age;
    private bool old;
    private bool fertile;

    new public string name { get; private set; }

    public int speciesID { get; private set; }
    public int pregnancy { get; private set; }
    public int hunger { get; private set; }
    public bool readyForBirth { get; private set; }
    public bool moved = false;

    public Aggression aggression { get; private set; }
    public FoodNeeded foodNeeded { get; private set; }
    public FoodType foodType { get; private set; }
    public AnimalSize animalSize { get; private set; }
    public BodyType bodyType { get; private set; }
    public Gender gender { get; private set; }
    public int gestation { get; private set; }
    public Perception perception { get; private set; }
    public Speed speed { get; private set; }
    public Babies babies { get; private set; }
    public bool pregnant { get; private set; }
    public HashSet<Animal> spawn { get; private set; }

    public Sprite typeSprite { get; private set; }

    // Use this for initialization--
    void Start()
    {
        removeChildren();
    }

    public void initialize(string pName, Aggression agr, FoodNeeded fdNd, FoodType fType, BodyType bType, AnimalSize anSize, Gender gndr, Perception perc, int gest,
                Speed spd, Babies bbies, HumidityTolerance humid, TemperatureTolerance tempTol, Lifespan lfs, int specID, Sprite dispImage)
    {
        name = pName;
        age = 0;
        old = false;
        pregnant = false;
        speciesID = specID;
        aggression = agr;
        foodNeeded = fdNd;
        foodType = fType;
        animalSize = anSize;
        bodyType = bType;
        gender = gndr;
        gestation = gest;
        perception = perc;
        speed = spd;
        babies = bbies;
        hunger = hungerMax;
        base.initialize(humid, tempTol, lfs);
        typeSprite = dispImage;
    }

    // Update is called once per frame
    public void breed(Animal parent)
    {
        for (int i = 0; i < (int)babies; i++)
        {
            spawn.Add(createNew(this, parent));
        }
        pregnant = true;
        pregnancy = gestation;
    }

    public Animal createNew(Animal father, Animal mother)
    {
        System.Random random = new System.Random();

        System.Array agg = new Aggression[2] { father.aggression, mother.aggression };
        System.Array foodNeed = new FoodNeeded[2] { father.foodNeeded, mother.foodNeeded };
        System.Array fType = new FoodType[2] { father.foodType, mother.foodType };
        System.Array aSize = new AnimalSize[2] { father.animalSize, mother.animalSize };
        System.Array bType = new BodyType[2] { father.bodyType, mother.bodyType };
        System.Array gest = new int[2] { father.gestation, mother.gestation };
        System.Array perc = new Perception[2] { father.perception, mother.perception };
        System.Array spd = new Speed[2] { father.speed, mother.speed };
        System.Array babes = new Babies[2] { father.babies, mother.babies };
        System.Array gnd = new Gender[2] { father.gender, mother.gender };
        System.Array dispImage = new Sprite[2] { father.typeSprite, mother.typeSprite };

        GameObject newAnimal = (GameObject)Instantiate(mother.gameObject, mother.transform.position, Quaternion.identity);
        Animal a = newAnimal.GetComponent<Animal>();
        a.initialize(father.name,
                          (Aggression)agg.GetValue(random.Next(agg.Length)),
                          (FoodNeeded)foodNeed.GetValue(random.Next(foodNeed.Length)),
                          (FoodType)fType.GetValue(random.Next(fType.Length)),
                          (BodyType)bType.GetValue(random.Next(bType.Length)),
                          (AnimalSize)aSize.GetValue(random.Next(aSize.Length)),
                          (Gender)gnd.GetValue(random.Next(gnd.Length)),
                          (Perception)perc.GetValue(random.Next(perc.Length)),
                          (int)gest.GetValue(random.Next(gest.Length)),
                          (Speed)spd.GetValue(random.Next(spd.Length)),
                          (Babies)babes.GetValue(random.Next(babes.Length)),
                          base.humidityTol,
                          base.tempTol,
                          base.lifespan,
                          speciesID,
                          (Sprite)dispImage.GetValue(random.Next(dispImage.Length)));

        return a;
    }

    public int GetGender()
    {
        return (int)gender;
    }

    public int GetBabies()
    {
        return (int)babies;
    }

    public int GetSpeed()
    {
        return (int)speed;
    }

    public int GetPerception()
    {
        return (int)perception;
    }

    public int GetGestation()
    {
        return (int)gestation;
    }

    public int GetAgg()
    {
        return (int)aggression;
    }

    public int GetFoodNeeded()
    {
        return (int)foodNeeded;
    }

    public int GetFoodType()
    {
        return (int)foodType;
    }

    public int GetAnimalSize()
    {
        return (int)animalSize;
    }

    public int GetBodyType()
    {
        return (int)bodyType;
    }

    public void AgeAnimal()
    {
        if (++age / (float)lifespan > .5 && !old)
        {
            if (perception == Perception.FAR)
            {
                perception = Perception.SHORT;
            }

            if (speed > Speed.SLOW)
            {
                speed--;
            }

            if (aggression > Aggression.LOW)
            {
                aggression--;
            }
        }

        if (age / (float)lifespan > .75 && fertile)
        {
            fertile = false;
        }
    }

    public bool ModelMortality()
    {
        int rand = new System.Random().Next(0, (int)((float)lifespan * (float)animalSize * .5)) / 2;
        return rand <= (age * -.8 + (float)lifespan * (float)animalSize * .5);
    }

    public int eat(Organism food)
    {
        hunger = hungerMax;
        return 5;
    }

    public void removeChildren()
    {
        spawn = new HashSet<Animal>();
    }

    public void UpdateAnimal()
    {
        if (pregnant)
        {
            pregnancy -= 4;

            if (pregnancy <= 0)
            {
                readyForBirth = true;
                pregnant = false;
            }
        }

        hunger -= (int)foodNeeded;
    }

    public int CheckSurvive(bool food, Humidity humid, Temperature temp)
    {
        int ret = 0;

        if (food)
        {
            ret += 15;
        }

        if ((int)humidityTol == (int)humid)
        {
            ret += 7;
        }

        if ((int)tempTol == (int)temp)
        {
            ret += 7;
        }

        return ret;
    }

    public int CheckDeath(bool food, Humidity humid, Temperature temp, int poison)
    {
        int ret = 0;

        if (!food)
        {
            ret -= 3;
        }

        ret -= Mathf.Abs((int)humidityTol - (int)humid);
        ret -= Mathf.Abs((int)tempTol - (int)temp);
        ret -= ((poison + 1) * 3);

        return ret;
    }

    private int sizeCheck(Animal an)
    {
        int ret = 0;

        if (animalSize == AnimalSize.HUGE)
        {
            if (an.animalSize == AnimalSize.HUGE)
            {
                return 4;
            }
            else if (an.animalSize == AnimalSize.LARGE)
            {
                return 6;
            }
            else if (an.animalSize == AnimalSize.MEDIUM)
            {
                return 5;
            }
            else if (an.animalSize == AnimalSize.SMALL)
            {
                return 3;
            }
            else
            {
                return 2;
            }
        }
        else if (animalSize == AnimalSize.LARGE)
        {
            if (an.animalSize == AnimalSize.HUGE)
            {
                return 2;
            }
            else if (an.animalSize == AnimalSize.LARGE)
            {
                return 4;
            }
            else if (an.animalSize == AnimalSize.MEDIUM)
            {
                return 6;
            }
            else if (an.animalSize == AnimalSize.SMALL)
            {
                return 5;
            }
            else
            {
                return 3;
            }
        }
        else if (animalSize == AnimalSize.MEDIUM)
        {
            if (an.animalSize == AnimalSize.HUGE)
            {
                return 1;
            }
            else if (an.animalSize == AnimalSize.LARGE)
            {
                return 2;
            }
            else if (an.animalSize == AnimalSize.MEDIUM)
            {
                return 4;
            }
            else if (an.animalSize == AnimalSize.SMALL)
            {
                return 6;
            }
            else
            {
                return 7;
            }
        }
        else if (animalSize == AnimalSize.SMALL)
        {
            if (an.animalSize == AnimalSize.MEDIUM)
            {
                return 1;
            }
            else if (an.animalSize == AnimalSize.SMALL)
            {
                return 4;
            }
            else if (an.animalSize == AnimalSize.TINY)
            {
                return 8;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (an.animalSize == AnimalSize.SMALL)
            {
                return 1;
            }
            else if (an.animalSize == AnimalSize.TINY)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
    }

    private int speedCheck(Animal an)
    {
        if ((int)speed > (int)an.speed)
        {
            return 3;
        }
        else if ((int)speed == (int)an.speed)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private int agroCheck(Animal an)
    {
        return (int)aggression + (3 - (int)an.aggression) * 2 + 1;
    }

    private int poisonCheck(Plant plant)
    {
        return (int)plant.poisonous;
    }

    private int foodHeuristic(Organism org)
    {
        //For animals take into account a's size, speed, aggression and b's, body type, speed, size, and aggression 
        //For plants take into account b's poison, SpaceRequired, 
        Plant p = org.GetComponent<Plant>();
        Animal an = org.GetComponent<Animal>();

        if (an != null)
        {
            return sizeCheck(an) + (((int)an.bodyType + 1) * 2) + speedCheck(an) + agroCheck(an);
        }
        else if (p != null)
        {
            return poisonCheck(p) + (int)p.spaceNeeded + 1;
        }
        else
        {
            return 0;
        }
    }

    private int breedHeuristic(Animal anm)
    {
        return getSpeedHuer(anm) + getSizeHeur(anm) + getAgroHeur(anm) + (int)anm.babies + anm.gestation;
    }

    private int getAgroHeur(Animal an)
    {
        if (an.aggression == Aggression.HIGH && an.foodType != FoodType.HERBIVORE)
        {
            return 3;
        }
        else if (an.aggression == Aggression.MEDIUM)
        {
            return 2;
        }
        else if (an.aggression == Aggression.LOW)
        {
            return an.foodType != FoodType.HERBIVORE ? 1 : 3;
        }
        else
        {
            return 0;
        }
    }

    private int getSizeHeur(Animal an)
    {
        return (int)an.animalSize + 1;
    }

    private int getSpeedHuer(Animal an)
    {
        return (int)an.speed + 1;
    }

    private int checkProps(int specCount, List<Animal> animalList)
    {
        float avgSpec = (float)specCount / (float)animalList.Count;

        if (animalList.Count > 0)
        {
            if (avgSpec < .1)
            {
                return (int)foodType == 0 ? 4 : 1;
            }
            else if (avgSpec < .25)
            {
                return (int)foodType == 0 ? 3 : 2;
            }
            else if (avgSpec < .33)
            {
                return (int)foodType == 0 ? 2 : 3;
            }
            else
            {
                if (foodType != FoodType.CARNIVORE)
                {
                    return 4;
                }
            }
        }
        return 1;
    }

    private int rightHumid(Tile t)
    {
        int diff = Mathf.Abs((int)humidityTol - (int)t.biome.humidity);

        if (diff == 0)
        {
            return 3;
        }
        else if (diff == 1)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    private int rightTemp(Tile t)
    {
        int diff = Mathf.Abs((int)tempTol - (int)t.biome.temperature);

        if (diff == 0)
        {
            return 3;
        }
        else if (diff == 1)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    private int countSpec(List<Animal> animalList)
    {
        int specCount = 0;

        foreach (Animal an in animalList)
        {
            if (an.speciesID == speciesID)
            {
                specCount++;
            }
        }

        return specCount;
    }

    private int foodBounty(List<Animal> animalList, List<Plant> plantList, Tile current)
    {
        List<Animal> curAnimalList = current.animals.Keys.ToList<Animal>();
        List<Plant> curPlantList = current.plants.Keys.ToList<Plant>();
        int ret = 0;

        if (foodType == FoodType.CARNIVORE)
        {
            if (curAnimalList.Count > animalList.Count)
            {
                ret = 1;
            }
            else if (curAnimalList.Count == animalList.Count)
            {
                ret = 2;
            }
            ret = 3;
        }
        if (foodType == FoodType.HERBIVORE)
        {
            if (curPlantList.Count > plantList.Count)
            {
                ret = 1;
            }
            else if (curPlantList.Count == plantList.Count)
            {
                ret = 2;
            }
            ret = 3;
        }
        if (foodType == FoodType.OMNIVORE)
        {
            if (curPlantList.Count + curAnimalList.Count > plantList.Count + animalList.Count)
            {
                ret = 1;
            }
            else if (curPlantList.Count + curAnimalList.Count == plantList.Count + animalList.Count)
            {
                ret = 2;
            }
            ret = 3;
        }
        return ret;
    }

    private int tileHeuristic(Tile t, Tile cur)
    {
        //Take into account number of edibles (heh), number of same species animals, humidity levels, and temperature levels
        List<Animal> animalList = t.animals.Keys.ToList<Animal>();
        List<Plant> plantList = t.plants.Keys.ToList<Plant>();
        
        return checkProps(countSpec(animalList), animalList) + rightHumid(t) + rightTemp(t) + foodBounty(animalList, plantList, cur);
    }

    public Organism findFoodInRange(Tile currentTile)
    {
        int id = 0;

        Dictionary<Animal, int> animalsInTile = new Dictionary<Animal, int>();
        foreach (Animal an in currentTile.animals.Keys)
        {
            animalsInTile.Add(an, 1);
        }

        Dictionary<Plant, int> plantsInTile = new Dictionary<Plant, int>();
        foreach (Plant p in currentTile.plants.Keys)
        {
            plantsInTile.Add(p, 1);
        }


        Dictionary<int, Organism> toEat = new Dictionary<int, Organism>();
        Dictionary<int, int> heuristic = new Dictionary<int, int>();
        List<Animal> toRemove = new List<Animal>();

        animalsInTile.Remove(this);

        foreach (Animal an in animalsInTile.Keys)
        {
            if (an.speciesID == speciesID)
            {
                toRemove.Add(an);
            }
        }

        foreach (Animal an in toRemove)
        {
            animalsInTile.Remove(an);
        }

        if (foodType == FoodType.CARNIVORE || foodType == FoodType.OMNIVORE)
        {
            animalsInTile.Keys.ToList().ForEach(meat => toEat.Add(++id, meat));
        }

        if (foodType == FoodType.HERBIVORE || foodType == FoodType.OMNIVORE)
        {
            plantsInTile.Keys.ToList().ForEach(vegetable => toEat.Add(++id, vegetable));
        }

        toEat.Keys.ToList().ForEach(creature => heuristic.Add(creature, foodHeuristic(toEat[creature])));

        if (heuristic.Count > 0)
        {
            return toEat[heuristic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
        }
        else
        {
            return null;
        }
    }

    public Animal FindBreedingInRange(Tile currentTile)
    {
        int id = 0;

        Dictionary<Animal, int> animalsInTile = new Dictionary<Animal, int>();
        Dictionary<int, Animal> toBreed = new Dictionary<int, Animal>();
        Dictionary<int, int> heuristic = new Dictionary<int, int>();
        List<Animal> toRem = new List<Animal>();

        foreach (Animal an in currentTile.animals.Keys)
        {
            animalsInTile.Add(an, 1);
        }

        foreach(Animal an in animalsInTile.Keys)
        {
            if (an.speciesID != speciesID || an.gender == gender)
            {
                toRem.Add(an);
            }
        }

        foreach (Animal an in toRem)
        {
            animalsInTile.Remove(an);
        }

        if (animalsInTile.Count > 0)
        {
            animalsInTile.Keys.ToList().ForEach(an => toBreed.Add(++id, an));
        }

        if (toBreed.Count > 0)
        {
            toBreed.Keys.ToList().ForEach(an => heuristic.Add(an, breedHeuristic(toBreed[an])));
        }

        if (heuristic.Count > 0)
        {
            return toBreed[heuristic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
        }
        else
        {
            return null;
        }
    }

    public Tile FindTileInRange(List<GameObject> surrounding, Tile currentTile)
    {
        int id = 0;

        Dictionary<int, Tile> toMove = new Dictionary<int, Tile>();
        Dictionary<int, int> heuristic = new Dictionary<int, int>();

        foreach (GameObject t in surrounding)
        {
            toMove.Add(++id, t.GetComponent<Tile>());
        }

        toMove.Keys.ToList().ForEach(tile => heuristic.Add(tile, tileHeuristic(toMove[tile], currentTile)));

        if (heuristic.Count > 0)
        {
            return toMove[heuristic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
        }
        else
        {
            return null;
        }
    }

    public override bool Equals(object obj)
    {
        //Debug.Log("Calling overridden method.");
        return obj != null && obj is Animal && Equals(obj as Animal);
    }

    public bool Equals(Animal a)
    {
        //Debug.Log("Checking equality... " + speciesID.Equals(a.speciesID));
        return a != null && speciesID.Equals(a.speciesID) && aggression.Equals(a.aggression) && animalSize.Equals(a.animalSize) && babies.Equals(a.babies)
            && bodyType.Equals(bodyType) && foodNeeded.Equals(a.foodNeeded) && foodType.Equals(a.foodType) && gender.Equals(a.gender) && gestation.Equals(a.gestation)
            && perception.Equals(a.perception) && speed.Equals(a.speed);
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
