using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlantMutationPopup : MonoBehaviour {
    public Text mutatedPlantNameText;
    public InputField newPlantNameField;
    private bool originallyPaused;
    private bool originallySpedUp;
    private Plant newPlant;
    private Tile tile;

    public void initialize(Plant p, Tile tile, bool originallyPaused, bool originallySpedUp) {
        this.newPlant = p;
        this.tile = tile;
        this.mutatedPlantNameText.text = p.name;
        this.newPlantNameField.text = p.name + "*";
        this.originallyPaused = originallyPaused;
        this.originallySpedUp = originallySpedUp;
    }

    public void resumeGame() {
        string newName = newPlantNameField.text;

        if (newName.Equals("")) {
            newName = mutatedPlantNameText.text + "*";
        }

        newPlant.name = newName;

        tile.addPlant(newPlant, 1);

        World world = GameObject.Find("World").GetComponent<World>();

        world.resolveMutation();

        if (world.numPlantMutationsToBeNamed == 0) {
            if (originallySpedUp) {
                world.speedUp();
            }
            else if (!originallyPaused) {
                world.play();
            }
        }

        Destroy(this.gameObject);
    }
}
