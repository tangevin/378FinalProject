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
        if (this.plants.Keys.Count > 0) {
            sprite.color = Color.green;
        }
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonUp(0)) {
            foreach (Plant p in this.plants.Keys) {
                Debug.Log(p.name + " " + this.plants[p]);
            }
        }
        else if (Input.GetMouseButtonUp(1)) {
            GameObject newPlant = (GameObject)Instantiate(plantPrefab, new Vector3(x, y, 0), Quaternion.identity);
            Plant p = newPlant.GetComponent<Plant>();
                
            p.initialize("Test Plant", Spread.MEDIUM, PlantType.BUSH, Poisonous.MINOR, WaterNeeded.MEDIUM, 
                SpaceNeeded.MEDIUM, false, false, HumidityTolerance.MEDIUM, TemperatureTolerance.MEDIUM, Lifespan.LONG);

            this.addPlant(p, 1);
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

        foreach (Animal a in animalKeys) {

        }
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

    public void addPlant(Plant p, int numToAdd) {
        if (this.plants.ContainsKey(p)) {
            this.plants[p] += numToAdd;
        }
        else {
            this.plants.Add(p, numToAdd);
        }
    }
}
