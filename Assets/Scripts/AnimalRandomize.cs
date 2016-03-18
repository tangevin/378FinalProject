using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AnimalRandomize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Started");

        Button myButton = GetComponent<Button>(); // <-- you get access to the button component here

        myButton.onClick.AddListener(randomize);  // <-- you assign a method to the button OnClick event here
    }
	
	// Update is called once per frame
	void Update () {

        
    }


    void randomize()
    {
        Debug.Log("Stuff");
        List<Dropdown> dropdowns = new List<Dropdown>();

        dropdowns.Add(GameObject.Find("Aggression").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Appetite").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Sprite").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Size").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Gender").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Vision distance").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Speed").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Litter size").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Gestation time").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Humidity tolerance").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Temperature tolerance").GetComponent<Dropdown>());
        dropdowns.Add(GameObject.Find("Lifespan").GetComponent<Dropdown>());

        foreach (Dropdown d in dropdowns) {
            d.value = Random.Range(1, d.options.Count);
        }
    }
}
