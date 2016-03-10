using UnityEngine;
using System.Collections;

public class ParameterDropdownHandling : MonoBehaviour {
    private bool animalsSelected = true;

    public GameObject animalParameters;
    public GameObject plantParameters;

    public void changeDisplayedOptions() {
        if (this.animalsSelected) {
            animalParameters.SetActive(false);
            plantParameters.SetActive(true);
        }
        else {
            animalParameters.SetActive(true);
            plantParameters.SetActive(false);
        }

        this.animalsSelected = !this.animalsSelected;
    }
}
