using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public Camera camera;

    GameObject[,] map;
    public int height, width;
    public GameObject tilePrefab, worldPrefab;
    public HashSet<int> speciesIDs = new HashSet<int>();
    private bool gameTick = true;

    private Vector3 dragOrigin;
    public float dragSpeed;
    public float scrollSpeed;

    private const int startingTickSpeed = 4;
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

    // Use this for initialization
    void Start()
    {
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

            if (!curTile.initialized) {
                curTile.InitializeBiome(this, this.GetTilesInRange(curTile.x, curTile.y, 1), random);
            }
        }
    }

    void Update()
    {
        if (gameTick && !paused)
        {
            gameTick = false;
            StartCoroutine(update());
        }

        if (Input.GetMouseButtonDown(0)) {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(0)) {
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
    }

    private IEnumerator update()
    {
        yield return new WaitForSeconds(tickSpeed);

        if (!paused) {
            foreach (GameObject tileObject in this.map)
            {
                tileObject.GetComponent<Tile>().onGameTick();
            }

            foreach (GameObject tileObject in this.map)
            {
                tileObject.GetComponent<Tile>().resetMovement();
            }
        }

        this.gameTick = true;
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

    public void play() {
        if (!playSelected) {
            playButton.image.sprite = playSelectedSprite;
            pauseButton.image.sprite = pauseUnselectedSprite;
            accelerateButton.image.sprite = accelerateUnselectedSprite;

            this.paused = false;
            this.tickSpeed = startingTickSpeed;

            this.playSelected = true;
            this.pauseSelected = false;
            this.accelerateSelected = false;
        }
    }

    public void pause() {
        if (!pauseSelected) {
            playButton.image.sprite = playUnselectedSprite;
            pauseButton.image.sprite = pauseSelectedSprite;
            accelerateButton.image.sprite = accelerateUnselectedSprite;

            this.paused = true;

            this.playSelected = false;
            this.pauseSelected = true;
            this.accelerateSelected = false;
        }
    }

    public void speedUp() {
        if (!accelerateSelected) {
            playButton.image.sprite = playUnselectedSprite;
            pauseButton.image.sprite = pauseUnselectedSprite;
            accelerateButton.image.sprite = accelerateSelectedSprite;

            this.paused = false;
            this.tickSpeed = startingTickSpeed / 2;

            this.playSelected = false;
            this.pauseSelected = false;
            this.accelerateSelected = true;
        }
    }
}