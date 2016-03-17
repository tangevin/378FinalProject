using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

        Image background = GameObject.Find("Object Select Background").GetComponent<Image>();
        RectTransform backSpot = background.GetComponent<RectTransform>();
        InputField speciesNameField = GameObject.Find("Species Name").GetComponent<InputField>();
        RectTransform fieldSpot = speciesNameField.GetComponent<RectTransform>();

        if (animalsSelected)
        {
            fieldSpot.anchoredPosition = new Vector2(fieldSpot.anchoredPosition.x, -475);
            backSpot.anchoredPosition = new Vector2(backSpot.anchoredPosition.x, 0);
        } else
        {
            fieldSpot.anchoredPosition = new Vector2(fieldSpot.anchoredPosition.x, -365);
            backSpot.anchoredPosition = new Vector2(backSpot.anchoredPosition.x, 110);
        }

    }
}
