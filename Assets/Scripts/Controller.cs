using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public int teams = 3;
    public int size = 6;

    protected static Color[] teamColors = new Color[] {
        Color.blue,
        Color.red,
        Color.green,
        Color.yellow
    };

    public Material tileMaterial;
    public Material partMaterial;

    public Mesh[] topMeshes;
    public Mesh[] midMeshes;
    public Mesh[] botMeshes;

    protected GameObject CreatePiece(int team, int[] seeds)
    {
        GameObject piece = new GameObject("Piece");
        piece.transform.localPosition = Vector3.up * 3 / 2;
        piece.AddComponent<Piece>().team = team;
        for (int i = 0; i < 3; ++i)
        {
            GameObject part = CreatePart(team, seeds[i]);
            part.transform.parent = piece.transform;
            part.transform.localPosition = Vector3.up * (i - 1);
        }
        return piece;
    }

    protected GameObject CreatePart(int team, int seed) {
        Mesh[][] meshes = new Mesh[][] { botMeshes, midMeshes, topMeshes };
        return InstantiatePart(
            seed,
            seed % 3,
            meshes[seed % 3][seed % meshes[seed % 3].Length],
            Color.HSVToRGB((float)seed / int.MaxValue, 1, 1),
            teamColors[team]
        );
    }

    private GameObject InstantiatePart(int seed, int type, Mesh mesh, Color color, Color teamColor)
    {
        GameObject gameObject = new GameObject(type == 0 ? "Bot" : type == 1 ? "Mid" : "Top");
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = partMaterial;
        gameObject.AddComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        gameObject.AddComponent<Clickable>();
        gameObject.AddComponent<Outline>().OutlineColor = teamColor;
        SetColor(gameObject, color);

        Part part;
        if (type == 0) part = gameObject.AddComponent<BasicBot>();
        else if (type == 1) part = gameObject.AddComponent<BasicMid>();
        else part = gameObject.AddComponent<BasicTop>();

        part.seed = seed;
        part.normal = color;
        part.selected = type == 0 ? Color.blue : type == 1 ? Color.green : Color.red;
        return gameObject;
    }

    protected void SetColor(GameObject gameObject, Color color)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        List<Color> colors = new List<Color>();
        foreach (Vector3 _ in meshFilter.mesh.vertices) colors.Add(color);
        meshFilter.mesh.SetColors(colors);
    }
}
