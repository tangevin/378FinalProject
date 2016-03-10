using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    private bool active, initialized;
    private SpriteRenderer sprite;
    private GameObject world;
    private enum dictKeys {ANIMAL, PLANT, TILE};
    public int x { get; private set; }
    public int y { get; private set; }

    public GameObject plantPrefab;
    public GameObject animalPrefab;

    public Biome biome;

    public Dictionary<Plant, int> plants = new Dictionary<Plant,int>();
    public Dictionary<Animal, int> animals = new Dictionary<Animal,int>();

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        active = false;
        initialized = false;
    }

    public void InitializeBiome(List<GameObject> adjacentTiles)
    {
        if (!initialized)
        {
            initialized = true;
            int random = (int)(Random.value * 100);

            if (random < 35)
                biome = new Biome(BiomeType.OCEAN);
            else if (random < 45)
                biome = new Biome(BiomeType.RIVER);
            else if (random < 55)
                biome = new Biome(BiomeType.PLAIN);
            else if (random < 65)
                biome = new Biome(BiomeType.FOREST);
            else if (random < 75)
                biome = new Biome(BiomeType.MOUNTAIN);
            else
                biome = new Biome(BiomeType.DESERT);
        }
    }

    void Update() 
    {
        if (this.plants.Keys.Count > 0 && this.animals.Keys.Count == 0) 
        {
            sprite.color = Color.green;
        }
        else if (this.animals.Keys.Count > 0 && this.plants.Keys.Count == 0) 
        {
            sprite.color = Color.red;
        }
        else if (this.animals.Keys.Count == 0 && this.plants.Keys.Count == 0)
        {
            sprite.color = biome.color;
        }
        else
        {
            sprite.color = DispProportions(getDispProportions());
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            string toPrint = "Plants: ";

            foreach (Plant p in this.plants.Keys)
            {
                toPrint += p.name + " " + this.plants[p] + ", ";
            }

            toPrint += "Animals: ";
            foreach (Animal a in this.animals.Keys)
            {
                toPrint += a.name + " " + this.animals[a] + ", ";
            }

            Debug.Log(toPrint);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            GameObject newPlant = (GameObject)Instantiate(plantPrefab, new Vector3(x, y, 0), Quaternion.identity);
            Plant p = newPlant.GetComponent<Plant>();
                
            p.initialize("Test Plant", Spread.MEDIUM, PlantType.BUSH, Poisonous.MINOR, WaterNeeded.MEDIUM, 
                SpaceNeeded.MEDIUM, false, false, HumidityTolerance.MEDIUM, TemperatureTolerance.MEDIUM, Lifespan.LONG);

            this.addPlant(p, 1);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            GameObject newAnimal = (GameObject)Instantiate(animalPrefab, new Vector3(x, y, 0), Quaternion.identity);
            Animal a = newAnimal.GetComponent<Animal>();

            a.initialize("Test animal", Aggression.LOW, FoodNeeded.MEDIUM, FoodType.HERBIVORE, BodyType.QUADPED, AnimalSize.SMALL, Gender.MALE, 
                Perception.FAR, 3, Speed.MEDIUM, Babies.SING, HumidityTolerance.MEDIUM, TemperatureTolerance.MEDIUM, Lifespan.LONG, 0);

            this.addAnimal(a);
            
            newAnimal = (GameObject)Instantiate(animalPrefab, new Vector3(x, y, 0), Quaternion.identity);
            a = newAnimal.GetComponent<Animal>();

            a.initialize("Test animal", Aggression.LOW, FoodNeeded.MEDIUM, FoodType.HERBIVORE, BodyType.QUADPED, AnimalSize.SMALL, Gender.FEMALE,
                Perception.FAR, 3, Speed.MEDIUM, Babies.SING, HumidityTolerance.MEDIUM, TemperatureTolerance.MEDIUM, Lifespan.LONG, 0);

            this.addAnimal(a);
        }
    }

    void OnMouseDown()
    {
        /**List<GameObject> tiles = new List<GameObject>();
        world.GetComponent<World>().GetTilesInRange(tiles, x, y, 3);

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
        //Debug.Log("Boop...");
    }



    private void handleAnimals()
    {
        List<Animal> animalKeys = new List<Animal>(this.animals.Keys);
        List<Animal> toRem = new List<Animal>();
        List<Animal> toAdd = new List<Animal>();
        Dictionary<Animal, Tile> toMove = new Dictionary<Animal, Tile>();
        Dictionary<Animal, Animal> toPreg = new Dictionary<Animal, Animal>();
        foreach (Animal a in animalKeys)
        {
            bool eaten = false;
            int poison = 0;
            a.AgeAnimal();
            if (!a.moved)
            {
                //If the animal is pregnant and ready to give birth
                if (a.readyForBirth())
                {
                    HashSet<Animal> babies = a.giveBirth();
                    foreach (Animal baby in babies)
                        toAdd.Add(baby);
                    a.removeChildren();
                }
                List<GameObject> closeTiles = world.GetComponent<World>().GetTilesInRange(this.x, this.y, (int)a.perception + 1);
                closeTiles.Remove(this.gameObject);
                Dictionary<int, GameObject> dec = makeDec(a, closeTiles);
    
                if (dec.ContainsKey((int) dictKeys.ANIMAL))
                {
                    Animal other = dec[(int)dictKeys.ANIMAL].GetComponent<Animal>();
                    if (other.speciesID == a.speciesID && other.pregnant == false) {
                        if (! toPreg.ContainsKey(other))
                            toPreg.Add(other, a);
                    }
                    else if (other.speciesID != a.speciesID)
                    {
                        eaten = true;
                        a.eat(other.GetComponent<Organism>());
                        animals[other] -= 1;
                    }
                }
                else if (dec.ContainsKey((int) dictKeys.PLANT))
                {
                    Plant p = dec[(int)dictKeys.PLANT].GetComponent<Plant>();
                    a.eat(p);
                    switch (a.animalSize)
                    {
                        case AnimalSize.HUGE:
                            plants[p] -= 10;
                            break;
                        case AnimalSize.LARGE:
                            plants[p] -= 6;
                            break;
                        case AnimalSize.MEDIUM:
                            plants[p] -= 4;
                            break;
                        case AnimalSize.SMALL:
                            plants[p] -= 2;
                            break;
                        case AnimalSize.TINY:
                            plants[p] -= 1;
                            break;
                    }
                    if (plants[p] <= 0)
                        plants.Remove(p);
                    poison = (int)p.poisonous;
                    eaten = true;
                }
                else if (dec.ContainsKey((int) dictKeys.TILE))
                {
                    Tile t = dec[(int)dictKeys.TILE].GetComponent<Tile>();
                    a.moved = true;
                    if (! toMove.ContainsKey(a))
                        toMove.Add(a, t);
                    toRem.Add(a);
                }

                a.UpdateAnimal();
                int numSurvive = a.CheckSurvive(eaten, biome.humidity, biome.temperature);
                int numDie = a.CheckDeath(eaten, biome.humidity, biome.temperature, poison);
                if ((numSurvive + numDie) < 0)
                {
                    toRem.Add(a);
                }
            }
            if (a.getHunger() < 0 || ! a.ModelMortality())
            {
                toRem.Add(a);
            }
        }
        foreach (Animal an in animalKeys) {
            if (animals[an] <= 0)
            {
                toRem.Add(an);
            }
        }
        foreach (Animal anmil in toPreg.Keys)
        {
            anmil.breed(toPreg[anmil]);
        }
        foreach (Animal anmil in toMove.Keys)
            toMove[anmil].addAnimal(anmil);
        foreach (Animal anmil in toRem)
            animals.Remove(anmil);
        foreach (Animal baby in toAdd) {
            if (animals.ContainsKey(baby))
                animals[baby] += 1;
            else
                animals.Add(baby, 1);
        }
    }

    public void resetMovement()
    {
        foreach (Animal a in animals.Keys)
        {
            a.moved = false;
        }
    }

    private Dictionary<int, GameObject> makeDec(Animal anm, List<GameObject> tiles)
    {
        System.Random rand = new System.Random();
        Dictionary<int, GameObject> retDict = new Dictionary<int, GameObject>();
        Organism org = anm.findFoodInRange(this);
        Animal brd;
        if (anm.gender == Gender.MALE)
            brd = anm.FindBreedingInRange(this);
        else
            brd = null;
        Tile tile = anm.FindTileInRange(tiles, this);
        if (org == null && brd == null)
        {
            retDict.Add((int)dictKeys.TILE, tile.gameObject);
        }
        else if (org == null && tile == null)
            retDict.Add((int)dictKeys.ANIMAL, brd.gameObject);
        else if (tile == null && brd == null)
        {
            if (org.GetComponent<Animal>())
                retDict.Add((int)dictKeys.ANIMAL, org.gameObject);
            else
                retDict.Add((int)dictKeys.PLANT, org.gameObject);
        }
        else if (tile == null)
        {
            if (anm.hunger < 50)
            {
                if (org.GetComponent<Animal>())
                    retDict.Add((int)dictKeys.ANIMAL, org.gameObject);
                else
                    retDict.Add((int)dictKeys.PLANT, org.gameObject);
            }
            else {
                int rnd = rand.Next(0, 2);
                if (rnd == 1)
                {
                    if (org.GetComponent<Animal>())
                        retDict.Add((int)dictKeys.ANIMAL, org.gameObject);
                    else
                        retDict.Add((int)dictKeys.PLANT, org.gameObject);
                }
                else
                    retDict.Add((int)dictKeys.ANIMAL, brd.gameObject);
            }
        }
        else if (org == null)
        {
            int rnd = rand.Next(0, 2);
            if (rnd == 1)
            {
                retDict.Add((int)dictKeys.ANIMAL, brd.gameObject);
            }
            else
            {
                retDict.Add((int)dictKeys.TILE, tile.gameObject);
            }
        }
        else if (brd == null)
        {
            if (anm.hunger < 50)
            {
                if (org.GetComponent<Animal>())
                    retDict.Add((int)dictKeys.ANIMAL, org.gameObject);
                else
                    retDict.Add((int)dictKeys.PLANT, org.gameObject);
            }
            else {
                int rnd = rand.Next(0, 2);
                if (rnd == 1)
                {
                    if (org.GetComponent<Animal>())
                        retDict.Add((int)dictKeys.ANIMAL, org.gameObject);
                    else
                        retDict.Add((int)dictKeys.PLANT, org.gameObject);
                }
                else
                    retDict.Add((int)dictKeys.TILE, tile.gameObject);
            }
        }
        else {
            if (anm.hunger < 50)
            {
                if (org.GetComponent<Animal>())
                    retDict.Add((int)dictKeys.ANIMAL, org.gameObject);
                else
                    retDict.Add((int)dictKeys.PLANT, org.gameObject);
            }
            else {
                int rnd = rand.Next(0, 3);
                if (rnd == 1)
                {
                    if (org.GetComponent<Animal>())
                        retDict.Add((int)dictKeys.ANIMAL, org.gameObject);
                    else
                        retDict.Add((int)dictKeys.PLANT, org.gameObject);
                }
                else if (rnd == 2)
                    retDict.Add((int)dictKeys.ANIMAL, brd.gameObject);
                else
                    retDict.Add((int)dictKeys.TILE, tile.gameObject);
            }
        }
        return retDict;
    }

    

    private void handlePlants()
    {
        List<Plant> plantKeys = new List<Plant>(this.plants.Keys);

        foreach (Plant p in plantKeys)
        {
            int numInTile = plants[p];

            int numGrowth = p.checkInTileGrowth(numInTile, biome.amountOfWater, biome.humidity, biome.temperature);
            int numDeath = p.checkInTileDeath(numInTile, biome.amountOfWater, biome.humidity, biome.temperature);

            int newTotal = numInTile + numGrowth - numDeath;

            if (newTotal <= 0)
            {
                plants.Remove(p);
            }
            else
            {
                if (p.checkCanSpread(newTotal))
                {
                    List<GameObject> surroundingTiles = world.GetComponent<World>().GetTilesInRange(this.x, this.y, 1);
                    surroundingTiles.Remove(this.gameObject);

                    List<GameObject> invalidTiles = new List<GameObject>();
                    foreach (GameObject tileObject in surroundingTiles) {
                        Tile tile = tileObject.GetComponent<Tile>();

                        if (tile.biome.biomeType == BiomeType.OCEAN) {
                            invalidTiles.Add(tileObject);
                        }
                        else if (tile.biome.biomeType == BiomeType.MOUNTAIN && !p.canSurviveInMountains) {
                            invalidTiles.Add(tileObject);
                        }
                        else if (tile.biome.biomeType == BiomeType.DESERT && !p.canSurviveInDesert) {
                            invalidTiles.Add(tileObject);
                        }
                    }

                    foreach (GameObject invalidTile in invalidTiles) {
                        surroundingTiles.Remove(invalidTile);
                    }

                    System.Random random = new System.Random();

                    List<Tile> tilesToSpreadTo = new List<Tile>();

                    if (p.spread == Spread.LOW) {
                        for (int i = 0; i < 1 && surroundingTiles.Count >= 1; i++)
                        {
                            int spreadNumber = random.Next(0, surroundingTiles.Count);
                            GameObject tileObject = surroundingTiles[spreadNumber];

                            surroundingTiles.RemoveAt(spreadNumber);
                            tilesToSpreadTo.Add(tileObject.GetComponent<Tile>());

                            newTotal--;
                        }
                    }
                    else if (p.spread == Spread.MEDIUM)
                    {
                        for (int i = 0; i < 2 && surroundingTiles.Count >= 1; i++)
                        {
                            int spreadNumber = random.Next(0, surroundingTiles.Count);
                            GameObject tileObject = surroundingTiles[spreadNumber];

                            surroundingTiles.RemoveAt(spreadNumber);
                            tilesToSpreadTo.Add(tileObject.GetComponent<Tile>());

                            newTotal--;
                        }
                    }
                    else if (p.spread == Spread.HIGH)
                    {
                        for (int i = 0; i < 3 && surroundingTiles.Count >= 1; i++)
                        {
                            int spreadNumber = random.Next(0, surroundingTiles.Count);
                            GameObject tileObject = surroundingTiles[spreadNumber];

                            surroundingTiles.RemoveAt(spreadNumber);
                            tilesToSpreadTo.Add(tileObject.GetComponent<Tile>());

                            newTotal--;
                        }
                    }

                    foreach (Tile t in tilesToSpreadTo)
                    {
                        if (p.spaceNeeded == SpaceNeeded.SMALL)
                        {
                            t.addPlant(p, 3);
                            newTotal -= 3;
                        }
                        else if (p.spaceNeeded == SpaceNeeded.MEDIUM)
                        {
                            t.addPlant(p, 2);
                            newTotal -= 2;
                        }
                        else if (p.spaceNeeded == SpaceNeeded.LARGE)
                        {
                            t.addPlant(p, 1);
                            newTotal -= 1;
                        }
                        else if (p.spaceNeeded == SpaceNeeded.EXTRALARGE)
                        {
                            t.addPlant(p, 1);
                            newTotal -= 1;
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
            {
                counts.Add(a.speciesID, 1);
            }
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
        return Color.yellow;
    }

    public void addPlant(Plant p, int numToAdd)
    {
        if (this.plants.ContainsKey(p))
        {
            this.plants[p] += numToAdd;
        }
        else
        {
            this.plants.Add(p, numToAdd);
        }
    }

    public void addAnimal(Animal a)
    {
        this.animals.Add(a, 1);
    }
}