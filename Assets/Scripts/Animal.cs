using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Animal : Organism
{
    private SpriteRenderer sprite;
    private int lowGes = 2;
    private int highGes = 20;
    private int hungerMax = 100;
    private int age;
    private bool old;
    private bool fertile;

    public string name { get; private set; }

    public int speciesID { get; private set; }
    public int pregnancy { get; private set; }
    public int hunger { get; private set; }
    public bool ready { get; private set; }
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
    public HashSet<Animal> spawn = new HashSet<Animal>();

    // Use this for initialization--
    void Start()
    {

    }

    public void initialize(string pName, Aggression agr, FoodNeeded fdNd, FoodType fType, BodyType bType, AnimalSize anSize, Gender gndr, Perception perc, int gest,
                Speed spd, Babies bbies, HumidityTolerance humid, TemperatureTolerance tempTol, Lifespan lfs, int specID)
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
    }

    // Update is called once per frame
    public void breed(Animal parent)
    {
        for (int i = 0; i < (int)babies; i++)
            spawn.Add(createNew(this, parent));
        pregnant = true;
        pregnancy = gestation;
    }

    public bool isPregnant()
    {
        return pregnant;
    }

    Animal createNew(Animal father, Animal mother)
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
                          speciesID);

        return a;
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

    public bool IsFertile()
    {
        return fertile;
    }

    public bool ModelMortality()
    {
        int rand = new System.Random().Next(0, (int)((float)lifespan * (float)animalSize * .5));
        return rand <=
            (age * -.8 + (float)lifespan * (float)animalSize * .5);
    }

    public HashSet<Animal> giveBirth()
    {
        return spawn;
    }

    public bool readyForBirth()
    {
        return ready;
    }

    public int eat(Organism food)
    {
        hunger = hungerMax;
        return 5;
    }

    public int getHunger()
    {
        return hunger;
    }

    public void removeChildren()
    {
        spawn = new HashSet<Animal>();
    }

    void Update()
    {

    }

    public void UpdateAnimal()
    {
        if (pregnant)
        {
            if (pregnancy-- == 0)
            {
                ready = true;
                pregnant = false;
            }
        }
        string fneed = foodNeeded.ToString();
        switch (fneed)
        {
            case "LOW":
                hunger -= 1;
                break;
            case "MEDIUM":
                hunger -= 5;
                break;
            case "HIGH":
                hunger -= 10;
                break;
        }
    }

    public int CheckSurvive(bool food, Humidity humid, Temperature temp)
    {
        int ret = 0;

        if (food) { ret += 15; }

        if (humidityTol == HumidityTolerance.LOW && humid == Humidity.LOW) { ret += 5; };

        if (humidityTol == HumidityTolerance.MEDIUM && humid == Humidity.MEDIUM) { ret += 5; };

        if (humidityTol == HumidityTolerance.HIGH && humid == Humidity.HIGH) { ret += 5; };

        if (tempTol == TemperatureTolerance.LOW && temp == Temperature.LOW) { ret += 5; };

        if (tempTol == TemperatureTolerance.MEDIUM && temp == Temperature.MEDIUM) { ret += 5; };

        if (tempTol == TemperatureTolerance.HIGH && temp == Temperature.HIGH) { ret += 5; };

        return ret;
    }

    public int CheckDeath(bool food, Humidity humid, Temperature temp, int poison)
    {
        int ret = 0;

        if (!food) { ret -= 3; }

        if (humidityTol == HumidityTolerance.LOW && humid == Humidity.MEDIUM) { ret -= 1; };

        if (humidityTol == HumidityTolerance.LOW && humid == Humidity.HIGH) { ret -= 2; };

        if (humidityTol == HumidityTolerance.MEDIUM && humid == Humidity.LOW) { ret -= 1; };

        if (humidityTol == HumidityTolerance.MEDIUM && humid == Humidity.HIGH) { ret -= 1; };

        if (humidityTol == HumidityTolerance.HIGH && humid == Humidity.MEDIUM) { ret -= 1; };

        if (humidityTol == HumidityTolerance.HIGH && humid == Humidity.LOW) { ret -= 2; };

        if (tempTol == TemperatureTolerance.LOW && temp == Temperature.MEDIUM) { ret -= 1; };

        if (tempTol == TemperatureTolerance.LOW && temp == Temperature.HIGH) { ret -= 2; };

        if (tempTol == TemperatureTolerance.MEDIUM && temp == Temperature.LOW) { ret -= 1; };

        if (tempTol == TemperatureTolerance.MEDIUM && temp == Temperature.HIGH) { ret -= 1; };

        if (tempTol == TemperatureTolerance.HIGH && temp == Temperature.MEDIUM) { ret -= 1; };

        if (tempTol == TemperatureTolerance.HIGH && temp == Temperature.LOW) { ret -= 2; };

        ret -= ((poison + 1) * 3);

        return ret;
    }

    private int sizeCheck(Animal anm) {
        int ret = 0;
        if (animalSize == AnimalSize.HUGE)
        {
            if (anm.animalSize == AnimalSize.HUGE)
                ret = 4;
            else if (anm.animalSize == AnimalSize.LARGE)
                ret = 6;
            else if (anm.animalSize == AnimalSize.MEDIUM)
                ret = 5;
            else if (anm.animalSize == AnimalSize.SMALL)
                ret = 3;
            else
                ret = 2;
        }
        else if (animalSize == AnimalSize.LARGE)
        {
            if (anm.animalSize == AnimalSize.HUGE)
                ret = 2;
            else if (anm.animalSize == AnimalSize.LARGE)
                ret = 4;
            else if (anm.animalSize == AnimalSize.MEDIUM)
                ret = 6;
            else if (anm.animalSize == AnimalSize.SMALL)
                ret = 5;
            else
                ret = 3;
        }
        else if (animalSize == AnimalSize.MEDIUM)
        {
            if (anm.animalSize == AnimalSize.HUGE)
                ret = 1;
            else if (anm.animalSize == AnimalSize.LARGE)
                ret = 2;
            else if (anm.animalSize == AnimalSize.MEDIUM)
                ret = 4;
            else if (anm.animalSize == AnimalSize.SMALL)
                ret = 6;
            else
                ret = 7;
        }
        else if (animalSize == AnimalSize.SMALL)
        {
            if (anm.animalSize == AnimalSize.HUGE)
                ret = 0;
            else if (anm.animalSize == AnimalSize.LARGE)
                ret = 0;
            else if (anm.animalSize == AnimalSize.MEDIUM)
                ret = 1;
            else if (anm.animalSize == AnimalSize.SMALL)
                ret = 4;
            else
                ret = 8;
        }
        else {
            if (anm.animalSize == AnimalSize.HUGE)
                ret = 0;
            else if (anm.animalSize == AnimalSize.LARGE)
                ret = 0;
            else if (anm.animalSize == AnimalSize.MEDIUM)
                ret = 0;
            else if (anm.animalSize == AnimalSize.SMALL)
                ret = 1;
            else
                ret = 3;
        }
        return ret;
    }

    private int speedCheck(Animal anm) {
        int ret;
        if ((int)speed > (int)anm.speed)
            ret = 3;
        else if ((int)speed == (int)anm.speed)
            ret = 2;
        else
            ret = 1;
        return ret;
    }

    private int agroCheck(Animal anm) {
        int ret = 0;
        if (anm.aggression == Aggression.HIGH)
            ret += 1;
        else if (anm.aggression == Aggression.MEDIUM)
            ret += 3;
        else
            ret += 5;
        if (aggression == Aggression.HIGH)
            ret += 3;
        else if (aggression == Aggression.MEDIUM)
            ret += 2;
        else
            ret += 1;
        return ret;
    }

    private int poisonCheck(Plant plant) {
        int ret = 0;
        if (plant.poisonous == Poisonous.DEADLY)
            ret = -2;
        else if (plant.poisonous == Poisonous.MAJOR)
            ret = -1;
        else if (plant.poisonous == Poisonous.MINOR)
            ret = 1;
        else
            ret = 3;
        return ret;
    }

    private int foodHeuristic(Organism org)
    {
        //For animals take into account a's size, speed, aggression and b's, body type, speed, size, and aggression 
        //For plants take into account b's poison, SpaceRequired, 
        Plant plnt = org.GetComponent<Plant>();
        Animal anm = org.GetComponent<Animal>();
        if (anm != null)
            return sizeCheck(anm) + (((int)anm.bodyType + 1) * 2) + speedCheck(anm) + agroCheck(anm);
        else if (plnt != null)
            return poisonCheck(plnt) + (int)plnt.spaceNeeded + 1;
        return 0;
    }

    private int breedHeuristic(Animal anm)
    {
        return getSpeedHuer(anm) + getSizeHeur(anm) + getAgroHeur(anm) + (int)anm.babies + anm.gestation;
    }

    private int getAgroHeur(Animal anm) {
        int ret = 0;
        if (anm.aggression == Aggression.HIGH)
        {
            if (anm.foodType == FoodType.CARNIVORE || anm.foodType == FoodType.OMNIVORE)
                ret = 3;
        }
        else if (anm.aggression == Aggression.MEDIUM)
        {
            ret = 2;
        }
        if (anm.aggression == Aggression.LOW)
        {
            if (anm.foodType == FoodType.CARNIVORE || anm.foodType == FoodType.OMNIVORE)
                ret = 1;
            else
                ret = 3;
        }
        return ret;
    }

    private int getSizeHeur(Animal anm) {
        return (int)anm.animalSize + 1;
    }

    private int getSpeedHuer(Animal anm) {
        return (int)anm.speed + 1;
    }

    private int checkProps(int specCount, List<Animal> animalList) {
        if (animalList.Count > 0)
        {
            if (specCount / animalList.Count < .1)
            {
                if (foodType == FoodType.CARNIVORE)
                    return 4;
                else if (foodType == FoodType.OMNIVORE || foodType == FoodType.HERBIVORE)
                    return 1;
            }
            else if (specCount / animalList.Count < .25)
            {
                if (foodType == FoodType.CARNIVORE)
                    return 3;
                else if (foodType == FoodType.OMNIVORE || foodType == FoodType.HERBIVORE)
                    return 2;
            }
            else if (specCount / animalList.Count < .33)
            {
                if (foodType == FoodType.CARNIVORE)
                    return 2;
                else if (foodType == FoodType.OMNIVORE || foodType == FoodType.HERBIVORE)
                    return 3;
            }
            else {
                if (foodType == FoodType.OMNIVORE || foodType == FoodType.HERBIVORE)
                    return 4;
            }
        }
        return 1;
    }

    private int rightHumid(Tile t) {
        Humidity humid = t.biome.humidity;
        if (humidityTol == HumidityTolerance.LOW && humid == Humidity.MEDIUM) { return 1; };

        if (humidityTol == HumidityTolerance.LOW && humid == Humidity.HIGH) { return -1; };

        if (humidityTol == HumidityTolerance.LOW && humid == Humidity.LOW) { return 3; };

        if (humidityTol == HumidityTolerance.MEDIUM && humid == Humidity.LOW) { return 1; };

        if (humidityTol == HumidityTolerance.MEDIUM && humid == Humidity.HIGH) { return 1; };

        if (humidityTol == HumidityTolerance.MEDIUM && humid == Humidity.MEDIUM) { return 3; };

        if (humidityTol == HumidityTolerance.HIGH && humid == Humidity.MEDIUM) { return 1; };

        if (humidityTol == HumidityTolerance.HIGH && humid == Humidity.LOW) { return -1; };

        if (humidityTol == HumidityTolerance.HIGH && humid == Humidity.HIGH) { return 3; };

        return 1;
    }

    private int rightTemp(Tile t) {
        Temperature temp = t.biome.temperature;
        if (tempTol == TemperatureTolerance.LOW && temp == Temperature.LOW) { return 3; };

        if (tempTol == TemperatureTolerance.LOW && temp == Temperature.HIGH) { return -1; };

        if (tempTol == TemperatureTolerance.LOW && temp == Temperature.MEDIUM) { return 1; };

        if (tempTol == TemperatureTolerance.MEDIUM && temp == Temperature.LOW) { return 1; };

        if (tempTol == TemperatureTolerance.MEDIUM && temp == Temperature.MEDIUM) { return 3; };

        if (tempTol == TemperatureTolerance.MEDIUM && temp == Temperature.HIGH) { return 1; };

        if (tempTol == TemperatureTolerance.HIGH && temp == Temperature.LOW) { return -1; };

        if (tempTol == TemperatureTolerance.HIGH && temp == Temperature.MEDIUM) { return 1; };

        if (tempTol == TemperatureTolerance.HIGH && temp == Temperature.HIGH) { return 3; };

        return 1;
    }

    private int countSpec(List<Animal> animalList) {
        int specCount = 0;
        foreach (Animal anm in animalList)
        {
            if (anm.speciesID == speciesID)
                specCount++;
        }
        return specCount;
    }

    private int foodBounty(List<Animal> animalList, List<Plant> plantList, Tile current) {
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
        foreach (Animal a in currentTile.animals.Keys)
            animalsInTile.Add(a, 1);

        Dictionary<Plant, int> plantsInTile = new Dictionary<Plant, int>();
        foreach (Plant p in currentTile.plants.Keys)
            plantsInTile.Add(p, 1);


        Dictionary<int, Organism> toEat = new Dictionary<int, Organism>();
        Dictionary<int, int> heuristic = new Dictionary<int, int>();
        List<Animal> toRemove = new List<Animal>();

        animalsInTile.Remove(this);

        foreach (Animal anm in animalsInTile.Keys)
        {
            if (anm.speciesID == speciesID)
                toRemove.Add(anm);
        }

        foreach (Animal anm in toRemove)
            animalsInTile.Remove(anm);

        if (foodType == FoodType.CARNIVORE || foodType == FoodType.OMNIVORE) 
            animalsInTile.Keys.ToList().ForEach(meat => toEat.Add(++id, meat));

        if (foodType == FoodType.HERBIVORE || foodType == FoodType.OMNIVORE)
            plantsInTile.Keys.ToList().ForEach(vegetable => toEat.Add(++id, vegetable));

        toEat.Keys.ToList().ForEach(creature => heuristic.Add(creature, foodHeuristic(toEat[creature])));

        if (heuristic.Count > 0)
            return toEat[heuristic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
        else
            return null;
    }

    public Animal FindBreedingInRange(Tile currentTile)
    {
        int id = 0;

        Dictionary<Animal, int> animalsInTile = new Dictionary<Animal, int>();
        foreach (Animal a in currentTile.animals.Keys)
            animalsInTile.Add(a, 1);
        Dictionary<int, Animal> toBreed = new Dictionary<int, Animal>();
        Dictionary<int, int> heuristic = new Dictionary<int, int>();
        List<Animal> toRem = new List<Animal>();
        foreach(Animal anm in animalsInTile.Keys)
        {
            if (anm.speciesID != speciesID || anm.gender == gender)
                toRem.Add(anm);
        }

        foreach (Animal anm in toRem)
            animalsInTile.Remove(anm);

        if (animalsInTile.Count > 0)
            animalsInTile.Keys.ToList().ForEach(anm => toBreed.Add(++id, anm));

        if (toBreed.Count > 0)
            toBreed.Keys.ToList().ForEach(an => heuristic.Add(an, breedHeuristic(toBreed[an])));

        if (heuristic.Count > 0)
            return toBreed[heuristic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
        else
            return null;
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
            Tile t = toMove[heuristic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key];
            return t;
        }
        else
            return null;
    }
}
