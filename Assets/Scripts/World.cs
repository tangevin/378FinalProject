using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    GameObject[,] map;
    public int height, width;
    public GameObject tilePrefab, worldPrefab;

    // Use this for initialization
    void Start()
    {
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
    }

    public void GetTilesInRange(List<GameObject> list, int x, int y, int range)
    {
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
}