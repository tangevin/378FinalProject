using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    private bool active;
    private SpriteRenderer sprite;
    private GameObject world;
    public int x { get; private set; }
    public int y { get; private set; }

    public int range;
    public BaseHumidity baseHumidity;
    public BaseTemperature baseTemperature;
    public BiomeType biomeType;
    public HumidityVariation humidityVariation;
    public TemperatureVariation temperatureVariation;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        active = false;
    }

    void OnMouseDown()
    {
        List<GameObject> tiles = new List<GameObject>();
        world.GetComponent<World>().GetTilesInRange(tiles, x, y, range);

        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Tile>().ChangeColor();
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
}
