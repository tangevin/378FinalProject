using UnityEngine;
using System.Collections;

public class ParameterDropdownHandling : MonoBehaviour
{
    private bool animalsSelected = true;

    public GameObject animalParameters;
    public GameObject plantParameters;

    public void changeDisplayedOptions()
    {
        animalParameters.SetActive(!animalsSelected);
        plantParameters.SetActive(animalsSelected);

        animalsSelected = !animalsSelected;
    }
}
