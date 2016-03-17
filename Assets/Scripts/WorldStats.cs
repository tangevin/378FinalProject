using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldStats : MonoBehaviour {

    private bool isShowing;
    public int height, width; 

    private List<GameObject> gameTiles;
    private Dictionary<int, List<int>> animalPopHist;
    private Dictionary<Plant, List<int>> plantPopHist;
    private enum AnimalEnums {Ag}
    //private Dictionary<int, Animal> curAnimalSpec;
    //private List<Plant> curPlantSpec;

    void Start()
    {
        //world.GetComponent<World>().getAllTiles();
        animalPopHist = new Dictionary<int, List<int>>();
        plantPopHist = new Dictionary<Plant, List<int>>();
        isShowing = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            isShowing = !isShowing;
            //menu.SetActive(isShowing);
        }
    }

    void updatePopulations()
    {
        Dictionary<Animal, int> animalList;
        Dictionary<Plant, int> plantList;
        Dictionary<int, int> toAdd = new Dictionary<int, int>();
        Dictionary<Plant, int> toAddP = new Dictionary<Plant, int>();
        foreach(GameObject tile in gameTiles)
        {
            animalList = tile.GetComponent<Tile>().animals;
            plantList = tile.GetComponent<Tile>().plants;
            foreach(Animal a in animalList.Keys)
            {
                if (toAdd.Keys.ToList<int>().Contains(a.speciesID))
                    toAdd[a.speciesID] += animalList[a];
                else
                    toAdd.Add(a.speciesID, animalList[a]);
            }
            foreach(Plant p in plantList.Keys)
            {
                if (toAddP.Keys.ToList<Plant>().Contains(p))
                    toAddP[p] += plantList[p];
                else
                    toAddP.Add(p, plantList[p]);
            }
        }
        foreach(int id in toAdd.Keys)
            if (animalPopHist.Keys.ToList<int>().Contains(id))
                animalPopHist[id].Add(toAdd[id]);
            else
                animalPopHist.Add(id, new List<int>(toAdd[id]));
        foreach (Plant plant in toAddP.Keys)
            if (plantPopHist.Keys.ToList<Plant>().Contains(plant))
                plantPopHist[plant].Add(toAddP[plant]);
            else
                plantPopHist.Add(plant, new List<int>(toAddP[plant]));
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

    //float GetAnimalParamProp(System.Func<Animal> f, int id)
    //{
    //    List<Animal> animals = getAnimalofSpec(id);
    //    Dictionary<int, int> nums = new Dictionary<int, int>();
    //    foreach (Animal animal in animals)
    //    {
    //        int val = f(animal);
    //        if (nums.Keys.ToList<int>().Contains(val))
    //            nums[val] += 1;
    //        else
    //            nums.Add(val, 1);
    //    }
    //    return 0;
    //}
}
