using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animal : Organism
{
    private SpriteRenderer sprite;
    private int lowGes = 2;
    private int highGes = 20;
    private int speciesID;
    public int pregnancy;
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
        throw new System.Exception("Don't use this constructor.  An animal should always be given a species ID.");
    }

    private Animal(Aggression agr, FoodNeeded fdNd, FoodType fType, BodyType bType, AnimalSize anSize, Gender gndr, Perception perc, int gest,
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

    void Start (int specID, HumidityTolerance humid, TemperatureTolerance temp, Lifespan lfs)
    {
        pregnant = false;
        speciesID = specID;
        //Randomly select an enum value and assign it to each parameter
        System.Random random = new System.Random();

        System.Array agValues = Aggression.GetValues(typeof(Animal));
        aggression = (Aggression)agValues.GetValue(random.Next(agValues.Length));

        System.Array ndValues = FoodNeeded.GetValues(typeof(Animal));
        foodNeeded = (FoodNeeded)ndValues.GetValue(random.Next(ndValues.Length));

        System.Array typeValues = FoodType.GetValues(typeof(Animal));
        foodType = (FoodType)typeValues.GetValue(random.Next(typeValues.Length));

        System.Array sizeValues = AnimalSize.GetValues(typeof(Animal));
        animalSize = (AnimalSize)sizeValues.GetValue(random.Next(sizeValues.Length));

        System.Array tpValues = BodyType.GetValues(typeof(Animal));
        gender = (Gender)tpValues.GetValue(random.Next(tpValues.Length));

        System.Array genValues = Gender.GetValues(typeof(Animal));
        gender = (Gender)genValues.GetValue(random.Next(genValues.Length));

        gestation = random.Next(lowGes, highGes);
        pregnancy = gestation;

        System.Array perValues = Perception.GetValues(typeof(Animal));
        perception = (Perception)perValues.GetValue(random.Next(perValues.Length));

        System.Array spdValues = Speed.GetValues(typeof(Animal));
        speed = (Speed)spdValues.GetValue(random.Next(spdValues.Length));

        System.Array babValues = Speed.GetValues(typeof(Animal));
        babies= (Babies)babValues.GetValue(random.Next(babValues.Length));


        //Initialization of base enums
        base.initialize(humid, temp, lfs);
    }

    void Start (Aggression agr, FoodNeeded fdNd, FoodType fType, BodyType bType, AnimalSize anSize, Gender gndr, Perception perc, int gest,
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
	void breed(Animal parent)
    { 
        for (int i = 0; i < (int)babies; i++)
            spawn.Add(createNew(this, parent));
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

        return new Animal((Aggression)agg.GetValue(random.Next(agg.Length)),
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
    }

    HashSet<Animal> giveBirth()
    {
        return spawn;
    }

    void removeChildren()
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
    }

}
