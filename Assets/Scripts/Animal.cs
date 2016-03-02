using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    void Start ( )
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
    }

    public bool isPregnant()
    {
        return pregnant;
    }

    Animal createNew(Animal father, Animal mother)
    {
        System.Random random = new System.Random();

        System.Array agg = new Aggression[2] {father.aggression, mother.aggression};
        System.Array foodNeed = new FoodNeeded[2] {father.foodNeeded, mother.foodNeeded};
        System.Array fType = new FoodType[2] {father.foodType, mother.foodType};
        System.Array aSize = new AnimalSize[2] {father.animalSize, mother.animalSize};
        System.Array bType = new BodyType[2] {father.bodyType, mother.bodyType};
        System.Array gest = new int[2] {father.gestation, mother.gestation };
        System.Array perc = new Perception[2] {father.perception, mother.perception};
        System.Array spd = new Speed[2] {father.speed, mother.speed};
        System.Array babes = new Babies[2] {father.babies, mother.babies};
        System.Array gnd = new Gender[2] {father.gender, mother.gender};
        
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
        if (++age / (int)lifespan > .5 && ! old)
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
        if (++age / (int)lifespan > .75 && fertile)
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
        return (new System.Random()).Next(0, (int)((int)lifespan * (int)animalSize * .5)) <= 
            (age * -.8 + (int)lifespan * (int)animalSize * .5);
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
            pregnancy--;
            if (pregnancy == 0)
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

    public int CheckDeath(bool food, Humidity humid, Temperature temp)
    {
        int ret = 0;

        if (! food) { ret -= 3; }

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

        return ret;
    }

    public Organism findFoodInRange()
    {
        //Will implement later
        return this;
    }

    public Animal findBreedingInRange()
    {
        //Will implement later
        return this;
    }
}
