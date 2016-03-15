using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlantRandomizer : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Debug.Log("Started");

        Button myButton = GetComponent<Button>(); // <-- you get access to the button component here

        myButton.onClick.AddListener(randomize);  // <-- you assign a method to the button OnClick event here
    }

    // Update is called once per frame
    void Update()
    {


    }


    void randomize()
    {
        Debug.Log("Stuff");
        List<Dropdown> dropdowns = new List<Dropdown>();

        dropdowns.Add(GameObject.Find("Spread").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("PlantType").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Poisonous").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("WaterNeeded").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("SpaceNeeded").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("PlantHumidityTolerance").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("PlantTemperatureTolerance").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("PlantLifespan").GetComponent<Dropdown>());

        foreach (Dropdown d in dropdowns)
        {
            d.value = Random.Range(1, d.options.Count);
        }
    }
}
