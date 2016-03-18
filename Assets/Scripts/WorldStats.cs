using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class WorldStats : MonoBehaviour
{

    public Text anAggression;
    public Text anSpeed;
    public Text anName;
    public Text anPerception;
    public Text anGest;
    public Text anBabies;
    public Text anBodyType;
    public Text anGender;
    public Text anSize;
    public Text anFoodType;
    public Text anFoodNeed;

    public Text plName;
    public Text plSpread;
    public Text plType;
    public Text plPoison;
    public Text plWater;
    public Text plSpace;
    public Text plTemp;
    public Text plHumid;

    public Text growthPlant;
    public Text growthAnimal;

    public Text popPlant;
    public Text popAnimal;
    public Text popPlantN;
    public Text popAnimalN;

    public Dropdown AnimalDropdown;
    public Dropdown PlantDropdown;

    public Button Pan;

    public GameObject animalPanel;
    public GameObject plantPanel;

    public bool animalPanelVis;
    public bool plantPanelVis;

    private World world;
    private List<GameObject> gameTiles;
    private Dictionary<int, Animal> animalsInGame;
    private List<Plant> plantsInGame;
    public Dictionary<int, List<int>> animalPopHist;
    private Dictionary<Plant, List<int>> plantPopHist;
    private enum AnimalParams { AGGRESSION, APPETITE, DIET, SIZE, GENDER, BODYTYPE, LITTER, GESTATION, PERCEPTION, SPEED };

    public void initialize()
    {
        animalPanelVis = false;
        plantPanelVis = false;

        animalPanel.gameObject.SetActive(false);
        plantPanel.gameObject.SetActive(false);

        world = GameObject.Find("World").GetComponent<World>();
        gameTiles = world.getAllTiles();
        animalsInGame = new Dictionary<int, Animal>();
        plantsInGame = new List<Plant>();
        animalPopHist = new Dictionary<int, List<int>>();
        plantPopHist = new Dictionary<Plant, List<int>>();
        AnimalDropdown.options.Clear();
        AnimalDropdown.options.Add(new Dropdown.OptionData() { text = "None" });
        PlantDropdown.options.Clear();
        PlantDropdown.options.Add(new Dropdown.OptionData() { text = "None" });

        Pan.onClick.AddListener(delegate { UpdatePanel(); } );
    }

    void Start()
    {


    }

    void UpdatePanel()
    {
        //Debug.Log("An: " + AnimalDropdown.options[AnimalDropdown.value].text);
        //Debug.Log("Pl: " + PlantDropdown.options[PlantDropdown.value].text);
        animalPanel.gameObject.SetActive(false);
        plantPanel.gameObject.SetActive(false);
        plantPanelVis = false;
        animalPanelVis = false;
        if ((PlantDropdown.options[PlantDropdown.value].text.Equals("None") &&
            AnimalDropdown.options[AnimalDropdown.value].text.Equals("None")) ||
            (!PlantDropdown.options[PlantDropdown.value].text.Equals("None") &&
            !AnimalDropdown.options[AnimalDropdown.value].text.Equals("None")))
        {
            animalPanel.gameObject.SetActive(false);
            plantPanel.gameObject.SetActive(false);
            plantPanelVis = false;
            animalPanelVis = false;
            popAnimal.gameObject.SetActive(true);
            popAnimalN.gameObject.SetActive(true);
            popPlant.gameObject.SetActive(true);
            popPlantN.gameObject.SetActive(true);
        }
        else if (PlantDropdown.options[PlantDropdown.value].text.Equals("None"))
        {
            //Debug.Log("DOIN IT");
            AnimalPan(AnimalDropdown.options[AnimalDropdown.value].text);
        }
        else if (AnimalDropdown.options[AnimalDropdown.value].text.Equals("None"))
            PlantPan(PlantDropdown.options[PlantDropdown.value].text);
    }

    void Update()
    {
        
    }

    private int myAnimalDropdownValueChangedHandler()
    {
        if (!AnimalDropdown.options[AnimalDropdown.value].text.Equals("None"))
            AnimalPan(AnimalDropdown.options[AnimalDropdown.value].text);
        else
        {
            animalPanel.gameObject.SetActive(false);
            plantPanel.gameObject.SetActive(false);
            plantPanelVis = false;
            animalPanelVis = false;
        }
        return 1;
    }

    void AnimalPan(string name)
    {
        int anm = -100000000;
        Animal toDisp;

        if (plantPanelVis)
        {
            plantPanel.gameObject.SetActive(false);
            plantPanelVis = !plantPanelVis;
        }
        animalPanelVis = !animalPanelVis;
        animalPanel.gameObject.SetActive(animalPanelVis);
        popAnimal.gameObject.SetActive(false);
        popAnimalN.gameObject.SetActive(false);
        popPlant.gameObject.SetActive(false);
        popPlantN.gameObject.SetActive(false);
        foreach (int key in animalsInGame.Keys)
        {
            if (animalsInGame[key].name.Equals(name))
                anm = key;
        }
        if (anm != -100000000)
        {
            toDisp = GetAnimalStats(anm);
            DisplayAnimalStats(toDisp);
        }
    }

    void PlantPan(string name)
    {
        Plant plant = null;
        if (animalPanelVis)
        {
            animalPanel.gameObject.SetActive(false);
            animalPanelVis = !animalPanelVis;
        }
        plantPanelVis = !plantPanelVis;
        plantPanel.gameObject.SetActive(plantPanelVis);
        popAnimal.gameObject.SetActive(false);
        popAnimalN.gameObject.SetActive(false);
        popPlant.gameObject.SetActive(false);
        popPlantN.gameObject.SetActive(false);
        foreach (Plant p in plantsInGame)
        {
            if (p.name.Equals(name))
            {
                plant = p;
            }
        }
        if (plant != null)
            DisplayPlantStats(plant);
    }

    public void UpdatePopulations()
    {
        Dictionary<Animal, int> animalList;
        Dictionary<Plant, int> plantList;
        Dictionary<int, int> toAdd = new Dictionary<int, int>();
        Dictionary<Plant, int> toAddP = new Dictionary<Plant, int>();

        foreach (GameObject tile in gameTiles)
        {
            animalList = tile.GetComponent<Tile>().animals;
            plantList = tile.GetComponent<Tile>().plants;
            foreach (Animal a in animalList.Keys)
            {
                if (!animalsInGame.Keys.ToList<int>().Contains(a.speciesID))
                {
                    AnimalDropdown.options.Add(new Dropdown.OptionData() { text = a.name });
                    animalsInGame.Add(a.speciesID, a);
                }
                if (toAdd.Keys.ToList<int>().Contains(a.speciesID))
                    toAdd[a.speciesID] += animalList[a];
                else
                    toAdd.Add(a.speciesID, animalList[a]);
            }
            foreach (Plant p in plantList.Keys)
            {
                if (!plantsInGame.Contains(p))
                {
                    PlantDropdown.options.Add(new Dropdown.OptionData() { text = p.name });
                    plantsInGame.Add(p);
                }
                if (toAddP.Keys.ToList<Plant>().Contains(p))
                    toAddP[p] += plantList[p];
                else
                    toAddP.Add(p, plantList[p]);
            }
        }
        foreach (int id in toAdd.Keys)
            if (animalPopHist.Keys.ToList<int>().Contains(id))
                animalPopHist[id].Add(toAdd[id]);
            else {
                animalPopHist.Add(id, new List<int>());
                animalPopHist[id].Add(toAdd[id]);
            }
        foreach (Plant plant in toAddP.Keys)
            if (plantPopHist.Keys.ToList<Plant>().Contains(plant))
                plantPopHist[plant].Add(toAddP[plant]);
            else {
                plantPopHist.Add(plant, new List<int>());
                plantPopHist[plant].Add(toAddP[plant]);
            }
    }

    List<Animal> getAnimalofSpec(int id)
    {
        List<Animal> toRet = new List<Animal>();
        Dictionary<Animal, int> animalList;
        foreach (GameObject tile in gameTiles)
        {
            animalList = tile.GetComponent<Tile>().animals;
            foreach (Animal a in animalList.Keys)
            {
                if (!toRet.Contains(a))
                    toRet.Add(a);
            }
        }
        return toRet;
    }

    float AnimalGrowthRate(int id)
    {
        if (animalPopHist.Keys.ToList<int>().Contains(id))
            if (animalPopHist[id].Count > 1)
                return (float)animalPopHist[id][animalPopHist[id].Count - 1] / animalPopHist[id][animalPopHist[id].Count - 2];
        return 0;
    }

    float PlantGrowthRate(Plant p)
    {
        if (plantPopHist.Keys.ToList<Plant>().Contains(p))
            if (plantPopHist[p].Count > 1)
                return (float)plantPopHist[p][plantPopHist[p].Count - 1] / plantPopHist[p][plantPopHist[p].Count - 2];
        return 0;
    }

    void DisplayPlantStats(Plant p)
    {
        plName.text = p.name;
        plHumid.text = p.humidityTol.ToString();
        plPoison.text = p.poisonous.ToString();
        plSpace.text = p.spaceNeeded.ToString();
        plSpread.text = p.spread.ToString();
        plTemp.text = p.tempTol.ToString();
        plType.text = p.plantType.ToString();
        plWater.text = p.waterNeeded.ToString();
        growthPlant.text = PlantGrowthRate(p).ToString();
    }

    void DisplayAnimalStats(Animal a)
    {
        anName.text = a.name;
        anPerception.text = a.perception.ToString();
        anSize.text = a.animalSize.ToString();
        anSpeed.text = a.speed.ToString();
        growthAnimal.text = AnimalGrowthRate(a.speciesID).ToString();
        anBabies.text = a.babies.ToString();
        anBodyType.text = a.bodyType.ToString();
        anAggression.text = a.aggression.ToString();
        anFoodNeed.text = a.foodNeeded.ToString();
        anFoodType.text = a.foodType.ToString();
        anGender.text = a.gender.ToString();
        anGest.text = a.gestation.ToString();
    }

    Dictionary<int, List<int>> InitDict(List<Animal> animals)
    {
        Dictionary<int, List<int>> paramTotals = new Dictionary<int, List<int>>();

        //Initialize the dictionary 
        foreach (AnimalParams par in System.Enum.GetValues(typeof(AnimalParams)))
            paramTotals.Add((int)par, new List<int>());

        //Aggregate all of the animal parameter values
        foreach (Animal a in animals)
        {
            paramTotals[(int)AnimalParams.APPETITE].Add((int)a.foodNeeded);
            paramTotals[(int)AnimalParams.BODYTYPE].Add((int)a.bodyType);
            paramTotals[(int)AnimalParams.DIET].Add((int)a.foodType);
            paramTotals[(int)AnimalParams.GENDER].Add((int)a.gender);
            paramTotals[(int)AnimalParams.GESTATION].Add((int)a.gestation);
            paramTotals[(int)AnimalParams.LITTER].Add((int)a.babies);
            paramTotals[(int)AnimalParams.PERCEPTION].Add((int)a.perception);
            paramTotals[(int)AnimalParams.SIZE].Add((int)a.animalSize);
            paramTotals[(int)AnimalParams.SPEED].Add((int)a.speed);
            paramTotals[(int)AnimalParams.AGGRESSION].Add((int)a.aggression);
        }

        return paramTotals;
    }

    Animal GetAnimalStats(int id)
    {
        List<Animal> animals = getAnimalofSpec(id);
        Dictionary<int, List<int>> paramTotals = InitDict(animals);
        Dictionary<int, int> mostCommonParams = new Dictionary<int, int>();

        //Find the most common parameter and put it's integer value into an array of parameters
        foreach (int par in paramTotals.Keys.ToList<int>())
            mostCommonParams.Add(par, paramTotals[par].GroupBy(item => item).OrderByDescending(g => g.Count()).Select(g => g.Key).First());

        //Create a new animal and return it for display
        GameObject newAnimal = (GameObject)Instantiate(animals[0].gameObject, animals[0].transform.position, Quaternion.identity);
        Animal an = newAnimal.GetComponent<Animal>();
        an.initialize(animalsInGame[id].name,
                      (Aggression)mostCommonParams[(int)AnimalParams.AGGRESSION],
                      (FoodNeeded)mostCommonParams[(int)AnimalParams.APPETITE],
                      (FoodType)mostCommonParams[(int)AnimalParams.DIET],
                      (BodyType)mostCommonParams[(int)AnimalParams.BODYTYPE],
                      (AnimalSize)mostCommonParams[(int)AnimalParams.SIZE],
                      (Gender)mostCommonParams[(int)AnimalParams.GENDER],
                      (Perception)mostCommonParams[(int)AnimalParams.PERCEPTION],
                      (int)mostCommonParams[(int)AnimalParams.GESTATION],
                      (Speed)mostCommonParams[(int)AnimalParams.SPEED],
                      (Babies)mostCommonParams[(int)AnimalParams.LITTER],
                      animalsInGame[id].humidityTol,
                      animalsInGame[id].tempTol,
                      animalsInGame[id].lifespan,
                      animalsInGame[id].speciesID,
                      animalsInGame[id].typeSprite);
        return an;
    }

    Plant MostCommonPlant()
    {
        Plant ret = null;
        Dictionary<Plant, int> curPops = new Dictionary<Plant, int>();
        foreach (Plant p in plantPopHist.Keys.ToList<Plant>())
            curPops.Add(p, plantPopHist[p][plantPopHist[p].Count - 1]);
        int val = curPops.Values.Max();
        foreach (Plant p in curPops.Keys.ToList<Plant>())
            if (curPops[p] == val)
                ret = p;
        return ret;
    }

    Animal MostCommonAnimal()
    {
        int ret = 0;
        Dictionary<int, int> curPops = new Dictionary<int, int>();
        foreach (int i in animalPopHist.Keys.ToList<int>())
            curPops.Add(i, animalPopHist[i][animalPopHist[i].Count - 1]);
        int val = curPops.Values.Max();
        foreach (int pop in curPops.Keys.ToList<int>())
            if (curPops[pop] == val)
                ret = pop;
        return animalsInGame[ret];
    }

    public void UpdateLargest()
    {
        if (animalPopHist != null)
            if (animalPopHist.Count > 0)
                popAnimal.text = MostCommonAnimal().name;
        if (plantPopHist != null)
            if (plantPopHist.Count > 0)
                popPlant.text = MostCommonPlant().name;
    }

}
