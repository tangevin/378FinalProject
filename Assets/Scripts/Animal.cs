using UnityEngine;
using System.Collections;

public class Animal : Organism {
    private SpriteRenderer sprite;

    public Aggression aggression;
    public FoodNeeded foodNeeded;
    public FoodType foodType;
    public AnimalSize animalSize;
    public BodyType bodyType;
    public Gender gender;
    public Gestation gestation;
    public Perception perception;
    public Speed speed;
    public Babies babies;

    // Use this for initialization
    void Start () {
        //Randomly select an enum value and assign it to each parameter
        System.Random random = new System.Random();

        System.Array agValues = Aggression.GetValues(typeof(Animal));
        aggression = (Aggression)agValues.GetValue(random.Next(agValues.Length));

        System.Array ndValues = FoodNeeded.GetValues(typeof(Animal));
        foodNeeded = (FoodNeeded)ndValues.GetValue(random.Next(ndValues.Length));

        System.Array typeValues = FoodType.GetValues(typeof(Animal));
        foodType = (FoodType)typeValues.GetValue(random.Next(typeValues.Length));

        System.Array genValues = BodyType.GetValues(typeof(Animal));
        gender = (Gender)genValues.GetValue(random.Next(genValues.Length));

        System.Array gestValues = Gender.GetValues(typeof(Animal));
        gestation = (Gestation)gestValues.GetValue(random.Next(gestValues.Length));

        System.Array perValues = Gestation.GetValues(typeof(Animal));
        perception = (Perception)perValues.GetValue(random.Next(perValues.Length));

        System.Array spdValues = Perception.GetValues(typeof(Animal));
        speed = (Speed)spdValues.GetValue(random.Next(spdValues.Length));

        System.Array babValues = Speed.GetValues(typeof(Animal));
        babies= (Babies)babValues.GetValue(random.Next(babValues.Length));

        System.Array sizeValues = AnimalSize.GetValues(typeof(Animal));
        animalSize = (AnimalSize)sizeValues.GetValue(random.Next(sizeValues.Length));
    }

	// Update is called once per frame
	void Update () {
	
	}
}
