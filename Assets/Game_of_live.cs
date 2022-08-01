using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//[ExecuteAlways]
public class Game_of_live : MonoBehaviour
{
    public int GRID_DIM_X = 10;
    public int GRID_DIM_Y = 10;
    public int GRID_DIM_Z = 10;

    bool[] game;
    bool[] newGame;
    GameObject[] gameObjects;

    public GameObject prefab;
    public Transform parent;

    public enum GridPreset
    {
        Random,
        Cube,
        Filled
    };

    [SerializeField]
    private GridPreset gridPreset;

    public static GridPreset Preset
    {
        get; set;
    }

    void Awake()
    {
        fillGame();
        fillGameObjects();
        render();
    }

    void Update()
    {
        //InvokeRepeating("gameOfLive", 1, 1);
    }

    void gameOfLive()
    {
        Debug.Log("step");
        updateGame();
        render();
    }

    private void fillGame()
    {
        game = new bool[GRID_DIM_X * GRID_DIM_Y * GRID_DIM_Z];
        newGame = new bool[GRID_DIM_X * GRID_DIM_Y * GRID_DIM_Z];

        switch (gridPreset)
        {
            case GridPreset.Random:
                for (int i = 0; i < GRID_DIM_X * GRID_DIM_Y * GRID_DIM_Z; i++)
                {
                    game[i] = UnityEngine.Random.value > 0.7 ? true : false;
                    newGame[i] = game[i];
                }
                break;
            case GridPreset.Cube:
                for (int z = (int)(GRID_DIM_Z * 0.4); z < (int)(GRID_DIM_Z * 0.6); z++)
                {
                    for (int y = (int)(GRID_DIM_Y * 0.4); y < (int)(GRID_DIM_Y * 0.6); y++)
                    {
                        for (int x = (int)(GRID_DIM_X * 0.4); x < (int)(GRID_DIM_X * 0.6); x++)
                        {
                            int idx = z * GRID_DIM_X * GRID_DIM_Y + y * GRID_DIM_X + x;
                            game[idx] = true;
                            newGame[idx] = true;
                        }
                    }
                }
                break;
            case GridPreset.Filled:
                for (int i = 0; i < GRID_DIM_X * GRID_DIM_Y * GRID_DIM_Z; i++)
                {
                    game[i] = true;
                    newGame[i] = true;
                }
                break;
        }

    }

    private void fillGameObjects()
    {
        gameObjects = new GameObject[GRID_DIM_X * GRID_DIM_Y * GRID_DIM_Z];

        for (int idx = 0; idx < parent.childCount; idx++)
        {
            GameObject child = parent.GetChild(idx).gameObject;
            DestroyImmediate(child);
        }

        Mesh tohMesh = toh_mesh.getMesh();

        for (int z = 0; z < GRID_DIM_Z; z++)
        {
            for (int y = 0; y < GRID_DIM_Y; y++)
            {
                for (int x = 0; x < GRID_DIM_X; x++)
                {
                    Vector3 position = new Vector3(x * 2 + (y % 2) * 1, y * 1, z * 2 + (y % 2) * 1);
                    position.x *= parent.transform.localScale.x;
                    position.y *= parent.transform.localScale.y;
                    position.z *= parent.transform.localScale.z;
                    GameObject obj = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
                    gameObjects[z * GRID_DIM_X * GRID_DIM_Y + y * GRID_DIM_X + x] = obj;
                    obj.GetComponent<MeshFilter>().mesh = tohMesh;
                }
            }
        }
    }

    private void updateGame()
    {
        for (int z = 0; z < GRID_DIM_Z; z++)
        {
            for (int y = 0; y < GRID_DIM_Y; y++)
            {
                for (int x = 0; x < GRID_DIM_X; x++)
                {
                    int idx = z * GRID_DIM_X * GRID_DIM_Y + y * GRID_DIM_X + x;
                    int countNeighbours = 0;
                    int countValidNeighbours = 0;
                    int up = idx + (GRID_DIM_X * GRID_DIM_Y);
                    int down = idx - (GRID_DIM_X * GRID_DIM_Y);
                    int[] neighbours = new int[6 + 8] { idx+1, idx-1, idx+GRID_DIM_X, idx-GRID_DIM_X,
                                                        up, up+1, up+GRID_DIM_X, up+GRID_DIM_X+1,
                                                        down, down + 1, down + GRID_DIM_X, down + GRID_DIM_X + 1,
                                                        idx + 2*(GRID_DIM_X * GRID_DIM_Y), idx - 2*(GRID_DIM_X * GRID_DIM_Y)};
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        try
                        {
                            countNeighbours += game[neighbours[i]] ? 1 : 0;
                            countValidNeighbours++;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // do nothing
                            continue;
                        }
                    }
                    //Debug.Log(countValidNeighbours + ", " + countNeighbours + ", " + game[idx]);
                    float percentage = countNeighbours / (float)countValidNeighbours;
                    if (game[idx])
                    {
                        if (percentage < 0.25 || percentage > 0.75)
                        {
                            newGame[idx] = false;
                            continue;
                        }
                        newGame[idx] = true;
                        continue;
                    }
                    if (percentage > 0.5 && percentage < 0.75)
                    {
                        newGame[idx] = true;
                    }
                }
            }
        }

        for (int i = 0; i < GRID_DIM_X * GRID_DIM_Y * GRID_DIM_Z; i++)
        {
            game[i] = newGame[i];
        }
    }

    private void render()
    {
        for (int z = 0; z < GRID_DIM_Z; z++)
        {
            for (int y = 0; y < GRID_DIM_Y; y++)
            {
                for (int x = 0; x < GRID_DIM_X; x++)
                {
                    int idx = z * GRID_DIM_X * GRID_DIM_Y + y * GRID_DIM_X + x;
                    gameObjects[idx].SetActive(game[idx]);
                }
            }
        }
    }

    public void redraw()
    {
        fillGame();
        fillGameObjects();
        render();
    }
}