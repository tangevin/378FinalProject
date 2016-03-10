using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour
{
    private bool active;
    public bool initialized { get; private set; }
    private SpriteRenderer sprite;
    private GameObject world;
    private enum dictKeys {ANIMAL, PLANT, TILE};
    public int x { get; private set; }
    public int y { get; private set; }

    public GameObject plantPrefab;
    public GameObject animalPrefab;

    public Biome biome = null;

    public Dictionary<Plant, int> plants = new Dictionary<Plant,int>();
    public Dictionary<Animal, int> animals = new Dictionary<Animal,int>();

    private EventSystem eventSystem;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        active = false;
        initialized = false;
        eventSystem = EventSystem.current;
    }

    public void InitializeBiome(World world, List<GameObject> adjacentTiles, System.Random random)
    {
        if (!initialized)
        {
            int oceanChance = 1;
            int riverChance = 1;
            int plainChance = 1;
            int forestChance = 1;
            int mountainChance = 1;
            int desertChance = 1;

            int highChange = 55;
            int mediumChange = 15;
            int lowChange = 1;

            foreach (GameObject tileObject in adjacentTiles) {
                Tile tile = tileObject.GetComponent<Tile>();

                if (tile.initialized) {
                    switch (tile.biome.biomeType) {
                        case BiomeType.OCEAN:
                            oceanChance += highChange;
                            riverChance += mediumChange;
                            break;
                        case BiomeType.RIVER:
                            riverChance += highChange;
                            oceanChance += mediumChange;
                            plainChance += lowChange;
                            forestChance += lowChange;
                            break;
                        case BiomeType.PLAIN:
                            plainChance += highChange;
                            forestChance += mediumChange;
                            riverChance += lowChange;
                            mountainChance += lowChange;
                            desertChance += lowChange;
                            break;
                        case BiomeType.FOREST:
                            forestChance += highChange;
                            plainChance += mediumChange;
                            riverChance += lowChange;
                            mountainChance += lowChange;
                            break;
                        case BiomeType.MOUNTAIN:
                            mountainChance += highChange;
                            desertChance += mediumChange;
                            plainChance += lowChange;
                            forestChance += lowChange;
                            break;
                        case BiomeType.DESERT:
                            desertChance += highChange;
                            mountainChance += mediumChange;
                            plainChance += lowChange;
                            break;
                    }
                }
            }

            int chanceSum = oceanChance + riverChance + plainChance + forestChance + mountainChance + desertChance;

            int r = random.Next(chanceSum);

            if (r < oceanChance) {
                biome = new Biome(BiomeType.OCEAN);
            }
            else if (r < oceanChance + riverChance) {
                biome = new Biome(BiomeType.RIVER);
            }
            else if (r < oceanChance + riverChance + plainChance) {
                biome = new Biome(BiomeType.PLAIN);
            }
            else if (r < oceanChance + riverChance + plainChance + forestChance) {
                biome = new Biome(BiomeType.FOREST);
            }
            else if (r < oceanChance + riverChance + plainChance + forestChance + mountainChance) {
                biome = new Biome(BiomeType.MOUNTAIN);
            }
            else {
                biome = new Biome(BiomeType.DESERT);
            }

            initialized = true;
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

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && !eventSystem.IsPointerOverGameObject())
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
        else if (Input.GetMouseButtonUp(1) && !eventSystem.IsPointerOverGameObject())
        {
            if (this.biome.biomeType != BiomeType.OCEAN) {
                Dropdown entityType = GameObject.Find("Type selector").GetComponent<Dropdown>();

                InputField speciesNameField = GameObject.Find("Species Name").GetComponent<InputField>();
                string speciesName = speciesNameField.text;

                if (entityType.value == 0) {
                    GameObject newAnimal = (GameObject)Instantiate(animalPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    Animal a = newAnimal.GetComponent<Animal>();

                    try {
                        Dropdown aggression = GameObject.Find("Aggression").GetComponent<Dropdown>();
                        Dropdown appetite = GameObject.Find("Appetite").GetComponent<Dropdown>();
                        Dropdown diet = GameObject.Find("Diet").GetComponent<Dropdown>();
                        Dropdown legs = GameObject.Find("Legs").GetComponent<Dropdown>();
                        Dropdown size = GameObject.Find("Size").GetComponent<Dropdown>();
                        Dropdown gender = GameObject.Find("Gender").GetComponent<Dropdown>();
                        Dropdown vision = GameObject.Find("Vision distance").GetComponent<Dropdown>();
                        Dropdown speed = GameObject.Find("Speed").GetComponent<Dropdown>();
                        Dropdown litter = GameObject.Find("Litter size").GetComponent<Dropdown>();
                        Dropdown gestation = GameObject.Find("Gestation time").GetComponent<Dropdown>();
                        Dropdown humid = GameObject.Find("Humidity tolerance").GetComponent<Dropdown>();
                        Dropdown temp = GameObject.Find("Temperature tolerance").GetComponent<Dropdown>();
                        Dropdown lifespan = GameObject.Find("Lifespan").GetComponent<Dropdown>();

                        Aggression aggr = ParseEnum<Aggression>(aggression.options.ToArray()[aggression.value].text);
                        FoodNeeded fatness = ParseEnum<FoodNeeded>(appetite.options.ToArray()[appetite.value].text);
                        FoodType vegan = ParseEnum<FoodType>(diet.options.ToArray()[diet.value].text);
                        BodyType triped = ParseEnum<BodyType>(legs.options.ToArray()[legs.value].text);
                        AnimalSize giants = ParseEnum<AnimalSize>(size.options.ToArray()[size.value].text);
                        Gender genitalia = ParseEnum<Gender>(gender.options.ToArray()[gender.value].text);
                        Perception vis = ParseEnum<Perception>(vision.options.ToArray()[vision.value].text);
                        Speed sonic = ParseEnum<Speed>(speed.options.ToArray()[speed.value].text);
                        Babies babycount = ParseEnum<Babies>(litter.options.ToArray()[litter.value].text);
                        int gesttime = gestation.value * 2;
                        HumidityTolerance humids = ParseEnum<HumidityTolerance>(humid.options.ToArray()[humid.value].text);
                        TemperatureTolerance temps = ParseEnum<TemperatureTolerance>(temp.options.ToArray()[temp.value].text);
                        Lifespan lifetime = ParseEnum<Lifespan>(lifespan.options.ToArray()[lifespan.value].text);

                        if (speciesName.Equals("")) {
                            speciesName = "Nameless Animal";
                        }

                        int id;
                        if (Animal.namesToIDs.ContainsKey(speciesName)) {
                            id = Animal.namesToIDs[speciesName];
                        }
                        else {
                            id = Animal.nextID;
                            Animal.namesToIDs[speciesName] = id;
                            Animal.nextID++;
                        }

                        a.initialize(speciesName, aggr, fatness, vegan, triped, giants, genitalia,
                            vis, gesttime, sonic, babycount, humids, temps, lifetime, id);

                        addAnimal(a);
                    }
                    catch {
                        Debug.Log("Some attributes weren't set.");
                    }
                }
                else {
                    Toggle mountainSurvivable = GameObject.Find("SurviveInMountains").GetComponent<Toggle>();
                    bool canSurviveInMountain = mountainSurvivable.isOn;

                    Toggle desertSurvivable = GameObject.Find("SurviveInDeserts").GetComponent<Toggle>();
                    bool canSurviveInDesert = desertSurvivable.isOn;

                    if ((this.biome.biomeType != BiomeType.MOUNTAIN && this.biome.biomeType != BiomeType.DESERT) ||
                        (this.biome.biomeType == BiomeType.MOUNTAIN && canSurviveInMountain) ||
                        (this.biome.biomeType == BiomeType.DESERT && canSurviveInDesert)) {
                        GameObject newPlant = (GameObject)Instantiate(plantPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        Plant p = newPlant.GetComponent<Plant>();

                        try {
                            Dropdown spreadDropdown = GameObject.Find("Spread").GetComponent<Dropdown>();
                            Dropdown plantTypeDropdown = GameObject.Find("PlantType").GetComponent<Dropdown>();
                            Dropdown poisonousDropdown = GameObject.Find("Poisonous").GetComponent<Dropdown>();
                            Dropdown waterNeededDropdown = GameObject.Find("WaterNeeded").GetComponent<Dropdown>();
                            Dropdown spaceNeededDropdown = GameObject.Find("SpaceNeeded").GetComponent<Dropdown>();
                            Dropdown humidityToleranceDropdown = GameObject.Find("PlantHumidityTolerance").GetComponent<Dropdown>();
                            Dropdown temperatureToleranceDropdown = GameObject.Find("PlantTemperatureTolerance").GetComponent<Dropdown>();
                            Dropdown lifespanDropdown = GameObject.Find("PlantLifespan").GetComponent<Dropdown>();

                            Spread spread = ParseEnum<Spread>(spreadDropdown.options.ToArray()[spreadDropdown.value].text);
                            PlantType plantType = ParseEnum<PlantType>(plantTypeDropdown.options.ToArray()[plantTypeDropdown.value].text);
                            Poisonous poisonous = ParseEnum<Poisonous>(poisonousDropdown.options.ToArray()[poisonousDropdown.value].text);
                            WaterNeeded waterNeeded = ParseEnum<WaterNeeded>(waterNeededDropdown.options.ToArray()[waterNeededDropdown.value].text);
                            SpaceNeeded spaceNeeded = ParseEnum<SpaceNeeded>(spaceNeededDropdown.options.ToArray()[spaceNeededDropdown.value].text);
                            HumidityTolerance humidityTolerance = ParseEnum<HumidityTolerance>(humidityToleranceDropdown.options.ToArray()[humidityToleranceDropdown.value].text);
                            TemperatureTolerance temperatureTolerance = ParseEnum<TemperatureTolerance>(temperatureToleranceDropdown.options.ToArray()[temperatureToleranceDropdown.value].text);
                            Lifespan lifespan = ParseEnum<Lifespan>(lifespanDropdown.options.ToArray()[lifespanDropdown.value].text);

                            if (speciesName.Equals("")) {
                                speciesName = "Nameless Plant";
                            }

                            p.initialize(speciesName, spread, plantType, poisonous, waterNeeded, spaceNeeded, canSurviveInMountain,
                                canSurviveInDesert, humidityTolerance, temperatureTolerance, lifespan);

                            this.addPlant(p, 1);
                        }
                        catch {
                            Debug.Log("Some attributes weren't set.");
                        }
                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.A) && !eventSystem.IsPointerOverGameObject())
        {
            
        }
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
                        //Debug.Log("Preg");
                        if (! toPreg.ContainsKey(other))
                            toPreg.Add(other, a);
                    }
                    else if (other.speciesID != a.speciesID)
                    {
                        //Debug.Log("eat");
                        eaten = true;
                        a.eat(other.GetComponent<Organism>());
                        animals[other] -= 1;
                    }
                }
                else if (dec.ContainsKey((int) dictKeys.PLANT))
                {
                    //Debug.Log("eat plant");
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
                    //Debug.Log("move");
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

            if (numGrowth > 0) {
                System.Random random = new System.Random();

                if (random.Next(2) == 0) {
                    Plant evolvePlant = p.evolve(numGrowth, this.biome);

                    if (evolvePlant != null) {
                        numGrowth--;
                        this.addPlant(evolvePlant, 1);
                    }

                    if (numGrowth > 0) {
                        Plant mutatePlant = p.mutate(numGrowth);

                        if (mutatePlant != null) {
                            numGrowth--;
                            this.addPlant(mutatePlant, 1);
                        }
                    }
                }
                else {
                    Plant mutatePlant = p.mutate(numGrowth);

                    if (mutatePlant != null) {
                        numGrowth--;
                        this.addPlant(mutatePlant, 1);
                    }

                    if (numGrowth > 0) {
                        Plant evolvePlant = p.evolve(numGrowth, this.biome);

                        if (evolvePlant != null) {
                            numGrowth--;
                            this.addPlant(evolvePlant, 1);
                        }
                    }
                }
            }

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
