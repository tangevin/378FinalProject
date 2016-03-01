using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    private bool active;
    private SpriteRenderer sprite;
    private GameObject world;
    public int x { get; private set; }
    public int y { get; private set; }

    public GameObject plantPrefab;
    public GameObject animalPrefab;

    public int range;
    public AmountOfWater amountOfWater;
    public BaseHumidity baseHumidity;
    public BaseTemperature baseTemperature;
    public Humidity humidity;
    public Temperature temperature;
    public BiomeType biomeType;
    public HumidityVariation humidityVariation;
    public TemperatureVariation temperatureVariation;

    public Dictionary<Plant, int> plants = new Dictionary<Plant,int>();
    public Dictionary<Animal, int> animals = new Dictionary<Animal,int>();

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        active = false;
    }

    void Update() {
        if (this.plants.Keys.Count > 0 && this.animals.Keys.Count == 0) {
            sprite.color = Color.green;
        }
        else if (this.animals.Keys.Count > 0 && this.plants.Keys.Count == 0) {
            sprite.color = Color.red;
        }
        else if (this.animals.Keys.Count == 0 && this.plants.Keys.Count == 0)
        {
            sprite.color = Color.white;
        }
        else {
            sprite.color = DispProportions(getDispProportions());
        }
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Plants:");
            foreach (Plant p in this.plants.Keys) {
                Debug.Log(p.name + " " + this.plants[p]);
            }

            Debug.Log("Animals:");
            foreach (Animal a in this.animals.Keys) {
                Debug.Log(a.name + " " + this.animals[a]);
            }
        }
        else if (Input.GetMouseButtonUp(1)) {
            GameObject newPlant = (GameObject)Instantiate(plantPrefab, new Vector3(x, y, 0), Quaternion.identity);
            Plant p = newPlant.GetComponent<Plant>();
                
            p.initialize("Test Plant", Spread.MEDIUM, PlantType.BUSH, Poisonous.MINOR, WaterNeeded.MEDIUM, 
                SpaceNeeded.MEDIUM, false, false, HumidityTolerance.MEDIUM, TemperatureTolerance.MEDIUM, Lifespan.LONG);

            this.addPlant(p, 1);
        }
        else if (Input.GetKeyUp(KeyCode.A)) {
            GameObject newAnimal = (GameObject)Instantiate(animalPrefab, new Vector3(x, y, 0), Quaternion.identity);
            Animal a = newAnimal.GetComponent<Animal>();

            a.initialize("Test animal", Aggression.LOW, FoodNeeded.LOW, FoodType.HERBIVORE, BodyType.QUADPED, AnimalSize.SMALL, Gender.MALE, 
                Perception.FAR, 20, Speed.MEDIUM, Babies.DOUB, HumidityTolerance.MEDIUM, TemperatureTolerance.MEDIUM, Lifespan.LONG, 0);

            this.addAnimal(a);
            /*
            newAnimal = (GameObject)Instantiate(animalPrefab, new Vector3(x, y, 0), Quaternion.identity);
            a = newAnimal.GetComponent<Animal>();

            a.initialize("Test animal", Aggression.LOW, FoodNeeded.LOW, FoodType.HERBIVORE, BodyType.QUADPED, AnimalSize.SMALL, Gender.FEMALE,
                Perception.FAR, 2, Speed.MEDIUM, Babies.DOUB, HumidityTolerance.MEDIUM, TemperatureTolerance.MEDIUM, Lifespan.LONG, 0);

            this.addAnimal(a);*/
        }
    }

    void OnMouseDown()
    {
        /*List<GameObject> tiles = new List<GameObject>();
        world.GetComponent<World>().GetTilesInRange(tiles, x, y, range);

        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().ChangeColor();
        }*/
    }

    public void SetData(int x, int y, GameObject world)
    {
        this.x = x;
        this.y = y;
        this.world = world;
    }

    private void ChangeColor()
    {
        sprite.color = (active = !active) ? Color.green : Color.white;
    }

    public void onGameTick()
    {
        handlePlants();
        handleAnimals();
    }

    private void handleAnimals() {
        List<Animal> animalKeys = new List<Animal>(this.animals.Keys);

        foreach (Animal a in animalKeys)
        {
            if (!a.moved)
            {
                bool eaten = false;
                //If the animal is pregnant and ready to give birth
                if (a.ready == true)
                {
                    HashSet<Animal> babies = a.giveBirth();
                    foreach (Animal baby in babies)
                        this.addAnimal(baby);
                    a.removeChildren();
                }
                List<GameObject> closeTiles = new List<GameObject>();
                world.GetComponent<World>().GetTilesInRange(closeTiles, this.x, this.y, (int)a.perception + 1);
                closeTiles.Remove(this.gameObject);
                List<Animal> ans = new List<Animal>(this.animals.Keys);
                List<Plant> plant = new List<Plant>(this.plants.Keys);
                GameObject decision = makeDecison(a, ans, plant, closeTiles);

                if (decision != null)
                {
                    System.Type decisionType = decision.GetType();
                    //switch (decisionType.ToString())
                    switch ("GameObject")
                    {
                        case "Animal":
                            Animal other = decision.GetComponent<Animal>();
                            //No canibalism allowed
                            if (other.speciesID == a.speciesID)
                            {
                                //Only males seek out breeding
                                other.breed(a);
                            }
                            else
                            {
                                //Predator Looking for food
                                if (other.speed <= a.speed)
                                {
                                    a.eat(other);
                                    animals.Remove(other);
                                    eaten = true;
                                }
                            }
                            break;
                        case "Plant":
                            //I still need to figure out how they interact with poison
                            Plant plnt = decision.GetComponent<Plant>();
                            a.eat(plnt);
                            plants.Remove(plnt);
                            break;
                        case "GameObject":
                            Tile t = decision.GetComponent<Tile>();
                            a.moved = true;
                            t.addAnimal(a);
                            this.animals.Remove(a);
                            break;
                    }
                }
                int numSurvive = a.CheckSurvive(eaten, humidity, temperature);
                int numDie = a.CheckDeath(eaten, humidity, temperature);

                if ((numSurvive + numDie) < 0)
                    animals.Remove(a);
                if ((a.hunger < 0))
                    animals.Remove(a);
            }
        }
    }

    public void resetMovement()
    {
        foreach (Animal a in animals.Keys)
        {
            a.moved = false;
        }
    }

    private GameObject makeDecison(Animal anm, List<Animal> ans, List<Plant> plnts, List<GameObject> tiles) {
        //Still need to implement this.  It currently selects an option at random
        //Will do later this week but for an alpha it works fine
        System.Random random = new System.Random();
        System.Array objs;

        List<GameObject> breeding = new List<GameObject>();
        List<GameObject> eating = new List<GameObject>();

        foreach (Animal a in ans)
        {
            if (a.speciesID == anm.speciesID)
            {
                if (a.gender != anm.gender)
                    breeding.Add(a.gameObject);
            }
            else {
                eating.Add(a.gameObject);
            }
        }

        System.Array animalArr = eating.ToArray();
        if (anm.gender == Gender.MALE)
        {
            int ind = random.Next(0, 2);
            if (ind == 0)
            {
                animalArr = breeding.ToArray();
            }
        }

        System.Array tileArr = tiles.ToArray();

        GameObject animal = null;

        if (animalArr.Length != 0) {
            animal = (GameObject)animalArr.GetValue(random.Next(animalArr.Length));
        }

        GameObject tile = tiles[random.Next(tiles.Count)];

        if (anm.foodType == FoodType.HERBIVORE || anm.foodType == FoodType.OMNIVORE)
        {
            GameObject plant = null;

            if (plnts.Count != 0) {
                plant = plnts[random.Next(plnts.Count)].gameObject;
            }

            objs = new GameObject[3] { animal, plant, tile };
        }
        else
        {
            objs = new GameObject[2] { animal, tile };
        }

        //return (GameObject)objs.GetValue(random.Next(objs.Length));
        return (GameObject)objs.GetValue(2);
    }

    private void handlePlants() {
        List<Plant> plantKeys = new List<Plant>(this.plants.Keys);

        foreach (Plant p in plantKeys) {
            int numInTile = plants[p];

            int numGrowth = p.checkInTileGrowth(numInTile, amountOfWater, humidity, temperature);
            int numDeath = p.checkInTileDeath(numInTile, amountOfWater, humidity, temperature);

            int newTotal = numInTile + numGrowth - numDeath;

            if (newTotal <= 0) {
                plants.Remove(p);
            }
            else {
                if (p.checkCanSpread(newTotal)) {
                    List<GameObject> surroundingTiles = new List<GameObject>();
                    world.GetComponent<World>().GetTilesInRange(surroundingTiles, this.x, this.y, 1);
                    surroundingTiles.Remove(this.gameObject);

                    System.Random random = new System.Random();

                    List<Tile> tilesToSpreadTo = new List<Tile>();

                    if (p.spread == Spread.LOW) {
                        for (int i = 0; i < 1 && surroundingTiles.Count >= 1; i++) {
                            int spreadNumber = random.Next(0, surroundingTiles.Count);
                            GameObject tileObject = surroundingTiles[spreadNumber];

                            surroundingTiles.RemoveAt(spreadNumber);
                            tilesToSpreadTo.Add(tileObject.GetComponent<Tile>());

                            newTotal--;
                        }
                    }
                    else if (p.spread == Spread.MEDIUM) {
                        for (int i = 0; i < 2 && surroundingTiles.Count >= 1; i++) {
                            int spreadNumber = random.Next(0, surroundingTiles.Count);
                            GameObject tileObject = surroundingTiles[spreadNumber];

                            surroundingTiles.RemoveAt(spreadNumber);
                            tilesToSpreadTo.Add(tileObject.GetComponent<Tile>());

                            newTotal--;
                        }
                    }
                    else if (p.spread == Spread.HIGH) {
                        for (int i = 0; i < 3 && surroundingTiles.Count >= 1; i++) {
                            int spreadNumber = random.Next(0, surroundingTiles.Count);
                            GameObject tileObject = surroundingTiles[spreadNumber];

                            surroundingTiles.RemoveAt(spreadNumber);
                            tilesToSpreadTo.Add(tileObject.GetComponent<Tile>());

                            newTotal--;
                        }
                    }

                    foreach (Tile t in tilesToSpreadTo) {
                        if (p.spaceNeeded == SpaceNeeded.SMALL) {
                            t.addPlant(p, 3);
                        }
                        else if (p.spaceNeeded == SpaceNeeded.MEDIUM) {
                            t.addPlant(p, 2);
                        }
                        else if (p.spaceNeeded == SpaceNeeded.LARGE) {
                            t.addPlant(p, 1);
                        }
                        else if (p.spaceNeeded == SpaceNeeded.EXTRALARGE) {
                            t.addPlant(p, 1);
                        }
                    }
                }

                this.plants[p] = newTotal;
            }
        }
    }

    public Dictionary<int, int> getDispProportions()
    {
        int plant = -1;
        int plantNum = plants.Count;
        int count = 1;
        List<Animal> animalKeys = new List<Animal>(this.animals.Keys);
        Dictionary<int, int> counts = new Dictionary<int, int>();
        counts.Add(plant, plantNum);

        foreach (Animal a in animalKeys)
        {
            if (counts.ContainsKey(a.speciesID))
            {
                counts[a.speciesID] += 1;
            }
            else
                counts.Add(a.speciesID, 1);
        }
        return counts;
    }

    public Color DispProportions(Dictionary<int, int> proportions)
    {
        //Will implement this in a bit, and will optimize it later in week so you don't check everything every game tick, but that will take some work
        int total = 0;
        foreach (int count in proportions.Values)
        {
            total += count;
        }
        float plantProp = proportions[-1] / total;
        return (Color.green + Color.red) * plantProp;
    }

    public void addPlant(Plant p, int numToAdd) {
        if (this.plants.ContainsKey(p)) {
            this.plants[p] += numToAdd;
        }
        else {
            this.plants.Add(p, numToAdd);
        }
    }

    public void addAnimal(Animal a)
    {
        this.animals.Add(a, 1);
    }
}
