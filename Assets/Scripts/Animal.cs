﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animal : Organism
{
    private SpriteRenderer sprite;
    private int lowGes = 2;
    private int highGes = 20;
    private int hungerMax = 100;

    public int speciesID;
    public int pregnancy;
    public int hunger;
    public bool ready;

    public Aggression aggression;
    public FoodNeeded foodNeeded;
    public FoodType foodType;
    public AnimalSize animalSize;
    public BodyType bodyType;
    public Gender gender;
    public int gestation;
    public Perception perception;
    public Speed speed;
    public Babies babies;
    public bool pregnant;
    public HashSet<Animal> spawn = new HashSet<Animal>();

    // Use this for initialization--
    void Start ( )
    {
        
    }

    public void initialize(Aggression agr, FoodNeeded fdNd, FoodType fType, BodyType bType, AnimalSize anSize, Gender gndr, Perception perc, int gest,
                Speed spd, Babies bbies, HumidityTolerance humid, TemperatureTolerance tempTol, Lifespan lfs, int specID)
    {
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
        base.initialize(humid, tempTol, lfs);
    }

	// Update is called once per frame
	public void breed(Animal parent)
    { 
        for (int i = 0; i < (int)babies; i++)
            spawn.Add(createNew(this, parent));
        pregnant = true;
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
        a.initialize((Aggression)agg.GetValue(random.Next(agg.Length)),
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

    public HashSet<Animal> giveBirth()
    {
        return spawn;
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

    void Update()
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
        switch(fneed)
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

        if (food) { ret += 5; }

        if (humidityTol == HumidityTolerance.LOW && humid == Humidity.LOW) { ret += 1; };

        if (humidityTol == HumidityTolerance.MEDIUM && humid == Humidity.MEDIUM) { ret += 1; };

        if (humidityTol == HumidityTolerance.HIGH && humid == Humidity.HIGH) { ret += 1; };

        if (tempTol == TemperatureTolerance.LOW && temp == Temperature.LOW) { ret += 1; };

        if (tempTol == TemperatureTolerance.MEDIUM && temp == Temperature.MEDIUM) { ret += 1; };

        if (tempTol == TemperatureTolerance.HIGH && temp == Temperature.HIGH) { ret += 1; };

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
