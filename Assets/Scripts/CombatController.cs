using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatController : Controller
{
    public Dictionary<Hex, GameObject> tiles;
    public Dictionary<Hex, GameObject> pieces;

    private int state = 0;
    private Part part;
    private Hex current;
    private Hex[] targets;

    public void Start()
    {
        CreateBoard();
    }

    public void Update()
    {
        if (state == 0 && Input.GetMouseButtonDown(0))
        {
            if (part != null) SetBoardColors(true);

            foreach (KeyValuePair<Hex, GameObject> pair in pieces)
            {
                foreach (Transform child in pair.Value.transform)
                {
                    if (child.gameObject.GetComponent<Clickable>().click)
                    {
                        part = child.gameObject.GetComponent<Part>();
                        current = pair.Key;
                        targets = part.GetTargets(current, this);
                        SetBoardColors(false);
                        if (targets.Length > 0) state = 1;
                        return;
                    }
                }
            }
        }
        else if (state == 1 && Input.GetMouseButtonDown(0))
        {
            SetBoardColors(true);

            state = 0;
            foreach (Hex target in targets)
            {
                if (tiles[target].GetComponent<Clickable>().click)
                {
                    state = 2;
                    targets = new Hex[] { target };
                    return;
                }
            }
        }
        else if (state == 2 && part.Act(current, targets[0], this)) state = -1;

        else if (state == -1) state = -2;
        else if (state == -2) state = 0;
    }

    private void SetBoardColors(bool reset)
    {
        SetColor(part.gameObject, reset ? part.normal : part.selected);
        foreach (Hex target in targets) SetColor(tiles[target], reset ? Color.white : part.selected);
    }

    private void CreateBoard()
    {
        tiles = new Dictionary<Hex, GameObject>();
        pieces = new Dictionary<Hex, GameObject>();

        Garage.Load();
        for (int i = 0; i < teams; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                Hex position = new Hex(-j, j - 5, 5);
                for (int k = 0; k < i * 6 / teams; ++k) position = position.RotateLeft();
                OrientObject(pieces, CreatePiece(i, Garage.GetTeam(i)[j]), position);
            }
        }

        OrientObject(tiles, CreateTile(), new Hex(0, 0, 0));
        for (int i = 1; i < size; ++i)
        {
            Hex hex = Hex.Direction(4).Scale(i);
            for (int j = 0; j < 6; ++j)
            {
                for (int k = 0; k < i; ++k)
                {
                    GameObject tile = OrientObject(tiles, CreateTile(), hex);
                    SetColor(tile, Color.white);
                    hex = hex.Neighbor(j);
                }
            }
        }
    }

    private GameObject OrientObject(Dictionary<Hex, GameObject> set, GameObject gameObject, Hex position)
    {
        Point a = Layout.basic.HexToPixel(position);
        gameObject.transform.parent = transform;
        gameObject.transform.Translate((float)a.x, 0, (float)a.y);
        set.Add(position, gameObject);
        return gameObject;
    }

    private GameObject CreateTile()
    {
        GameObject gameObject = new GameObject("Tile");

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[12];
        Vector3[] normals = new Vector3[12];
        List<Color> colors = new List<Color>();
        for (int i = 0; i < 12; ++i)
        {
            Point a = Layout.basic.HexCornerOffset(i);
            vertices[i] = new Vector3((float)a.x, i < 6 ? 0 : -1, (float)a.y);
            normals[i] = new Vector3(0, i < 6 ? 1 : -1, 0);
            colors.Add(Color.white);
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.SetColors(colors);
        mesh.triangles = new int[60]{
            0, 1, 2,    0, 2, 3,    0, 3, 4,    0, 4, 5,
            6, 8, 7,    6, 9, 8,    6, 10, 9,   6, 11, 10,
            0, 6, 1,    6, 7, 1,    1, 7, 2,    7, 8, 2,
            2, 8, 3,    8, 9, 3,    3, 9, 4,    9, 10, 4,
            4, 10, 5,   10, 11, 5,  5, 11, 0,   11, 6, 0
        };

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = tileMaterial;
        gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.AddComponent<Clickable>();
        return gameObject;
    }
}
