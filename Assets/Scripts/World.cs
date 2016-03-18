using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    new public Camera camera;

    private bool isShowing;
    private GameObject[,] map;
    public int height, width;
    public GameObject tilePrefab, worldPrefab, worldStatsPrefab;
    public HashSet<int> speciesIDs = new HashSet<int>();

    private Vector3 dragOrigin;
    public float dragSpeed;
    public float scrollSpeed;

    private const int startingTickSpeed = 240;
    private int tickCounter = 0;
    public int tickSpeed { get; private set; }
    public bool paused { get; private set; }
    public Button pauseButton;
    private bool pauseSelected = false;
    public Sprite pauseSelectedSprite;
    public Sprite pauseUnselectedSprite;
    public Button playButton;
    private bool playSelected = true;
    public Sprite playSelectedSprite;
    public Sprite playUnselectedSprite;
    public Button accelerateButton;
    private bool accelerateSelected = false;
    public Sprite accelerateSelectedSprite;
    public Sprite accelerateUnselectedSprite;
    public WorldStats worldStats;
    private int worldStatsCounter;

    public GameObject worldStatsPanel;

    public GameObject mutatedPlantPopupPanel;
    public int numPlantMutationsToBeNamed { get; private set; }

    private Tile currentTile;

    // Use this for initialization
    void Start()
    {
        worldStatsCounter = 10;
        isShowing = false;
        tickSpeed = startingTickSpeed;
        paused = false;

        Vector3 pos;
        map = new GameObject[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y % 2 == 0)
                {
                    pos = new Vector3((x - 0.25f - width / 2) * .5f, (y - height / 2) * .375f);
                }
                else
                {
                    pos = new Vector3((x + 0.25f - width / 2) * .5f, (y - height / 2) * .375f);
                }

                GameObject go = (GameObject)Instantiate(tilePrefab, pos, Quaternion.identity);
                go.GetComponent<Tile>().SetData(x, y, worldPrefab);
                map[y, x] = go;
            }
        }

        System.Random random = new System.Random();

        foreach (GameObject go in map)
        {
            Tile curTile = go.GetComponent<Tile>();

            if (!curTile.initialized)
            {
                curTile.InitializeBiome(this, GetTilesInRange(curTile.x, curTile.y, 1), random);
            }
        }
        worldStats.initialize();
        worldStatsPanel.gameObject.SetActive(false);
    }

    public List<GameObject> getAllTiles()
    {
        List<GameObject> tiles = new List<GameObject>();
        foreach (GameObject obj in map)
            tiles.Add(obj);
        return tiles;
    }

    void Update()
    {
        if (tickCounter >= tickSpeed && !paused)
        {
            updateWorld();
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            camera.transform.position -= new Vector3((mousePosition.x - dragOrigin.x) * (dragSpeed / 100), 
                (mousePosition.y - dragOrigin.y) * (dragSpeed / 100), 
                (mousePosition.z - dragOrigin.z) * (dragSpeed / 100));
            dragOrigin = mousePosition;
        }

        // Mouse wheel moving forward
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && camera.orthographicSize > 1)
        {
            camera.orthographicSize -= (scrollSpeed / 2);
        }
   
        // Mouse wheel moving backward
        if(Input.GetAxis("Mouse ScrollWheel") < 0 && camera.orthographicSize < 5)
        {
            camera.orthographicSize += (scrollSpeed / 2);
        }

        if (!paused)
        {
            tickCounter++;
        }

        if (Input.GetKeyDown("w"))
        {
            isShowing = !isShowing;
            worldStats.UpdateLargest();
            if (pauseSelected && worldStatsPanel.gameObject.active)
                play();
            else if ((playSelected || accelerateSelected) && !worldStatsPanel.gameObject.active)
                pause();

            if (worldStats.animalPanelVis)
            {
                worldStats.animalPanelVis = false;
                worldStats.animalPanel.gameObject.SetActive(false);
                worldStats.popAnimal.gameObject.SetActive(true);
                worldStats.popAnimalN.gameObject.SetActive(true);
                worldStats.popPlant.gameObject.SetActive(true);
                worldStats.popPlantN.gameObject.SetActive(true);
            }
            if (worldStats.plantPanelVis)
            {
                worldStats.plantPanelVis = false;
                worldStats.plantPanel.gameObject.SetActive(false);
                worldStats.popAnimal.gameObject.SetActive(true);
                worldStats.popAnimalN.gameObject.SetActive(true);
                worldStats.popPlant.gameObject.SetActive(true);
                worldStats.popPlantN.gameObject.SetActive(true);
            }
            worldStatsPanel.gameObject.SetActive(isShowing);
        }
    }

    private void updateWorld()
    {
        Debug.Log("World tick");
        if (!paused)
        {
            if (worldStatsCounter++ == 10)
            {
                worldStats.UpdatePopulations();
                worldStatsCounter = 0;
            }

            foreach (GameObject tileObject in map)
            {
                tileObject.GetComponent<Tile>().onGameTick();
            }

            foreach (GameObject tileObject in map)
            {
                tileObject.GetComponent<Tile>().resetMovement();
            }
        }

        tickCounter = 0;
    }

    public List<GameObject> GetTilesInRange(int x, int y, int range)
    {
        List<GameObject> list = new List<GameObject>();
        // Add the current tile
        list.Add(map[y, x]);

        if (range > 0)
        {
            // Get the first ring of tiles adjacent to the current tile
            GetImmediateAdjacentTiles(list, x, y);

            for (int rng = 0; rng < range - 1; rng++)
            {
                // Only check the tiles that were added from the previous ring
                int size = list.Count;
                for (int index = 0; index < size; index++)
                {
                    // Get each consecutive ring of adjacent tiles
                    Tile temp = list[index].GetComponent<Tile>();
                    GetImmediateAdjacentTiles(list, temp.x, temp.y);
                }
            }
        }
        return list;
    }

    private void GetImmediateAdjacentTiles(List<GameObject> list, int x, int y)
    {
        // Same and left
        if (x > 0)
        {
            AddConditionally(list, map[y, x - 1]);
        }
        // Same and right
        if (x < width - 1)
        {
            AddConditionally(list, map[y, x + 1]);
        }

        // Row Above
        if (y > 0)
        {
            // Immediately above
            AddConditionally(list, map[y - 1, x]);

            if (y % 2 == 1 && x < width - 1)
            {
                // Above and right
                AddConditionally(list, map[y - 1, x + 1]);
            }
            else if (y % 2 == 0 && x > 0)
            {
                // Above and left
                AddConditionally(list, map[y - 1, x - 1]);
            }
        }

        // Row Below
        if (y < height - 1)
        {
            // Immediately below
            AddConditionally(list, map[y + 1, x]);

            if (y % 2 == 1 && x < width - 1)
            {
                // Below and right
                AddConditionally(list, map[y + 1, x + 1]);
            }
            else if (y % 2 == 0 && x > 0)
            {
                // Below and left
                AddConditionally(list, map[y + 1, x - 1]);
            }
        }
    }

    private void AddConditionally(List<GameObject> list, GameObject tile)
    {
        // Only add a tile to the list if it wasn't there before
        if (!list.Contains(tile))
        {
            list.Add(tile);
        }
    }

    public void play()
    {
        if (!playSelected)
        {
            playButton.image.sprite = playSelectedSprite;
            pauseButton.image.sprite = pauseUnselectedSprite;
            accelerateButton.image.sprite = accelerateUnselectedSprite;

            paused = false;
            tickSpeed = startingTickSpeed;

            playSelected = true;
            pauseSelected = false;
            accelerateSelected = false;
        }
    }

    public void pause()
    {
        if (!pauseSelected)
        {
            playButton.image.sprite = playUnselectedSprite;
            pauseButton.image.sprite = pauseSelectedSprite;
            accelerateButton.image.sprite = accelerateUnselectedSprite;

            paused = true;

            playSelected = false;
            pauseSelected = true;
            accelerateSelected = false;
        }
    }

    public void speedUp()
    {
        if (!accelerateSelected)
        {
            playButton.image.sprite = playUnselectedSprite;
            pauseButton.image.sprite = pauseUnselectedSprite;
            accelerateButton.image.sprite = accelerateSelectedSprite;

            paused = false;
            tickSpeed = startingTickSpeed / 2;

            playSelected = false;
            pauseSelected = false;
            accelerateSelected = true;
        }
    }

    public void showPlantMutationPopup(Plant p, Tile tile) {
        this.numPlantMutationsToBeNamed++;
        bool originallyPaused = paused;
        bool originallySpedUp = accelerateSelected;
        this.pause();

        GameObject popupObject = (GameObject)Instantiate(mutatedPlantPopupPanel);
        PlantMutationPopup popup = popupObject.GetComponent<PlantMutationPopup>();
        popup.initialize(p, tile, originallyPaused, originallySpedUp);
        popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    public void resolveMutation() {
        this.numPlantMutationsToBeNamed--;
    }

    public void setCurrentlySelectedTile(Tile currentTile) {
        this.currentTile = currentTile;
    }

    public void hideCurrentlySelectedTilePanel() {
        this.currentTile.hideTilePanel();
    }
}