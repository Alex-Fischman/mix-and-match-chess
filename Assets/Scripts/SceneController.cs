using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : Controller
{
    public string combatScene = "Combat";

    public Button fight;
    public Button newPart;
    public Button combineParts;
    public Button breakPiece;
    public Button promotePiece;
    public Button demotePiece;
    public Button partInfo;

    public RectTransform partInfoIconPanel;
    public Text partInfoDescription;
    public RectTransform teamPanel;
    public RectTransform piecesPanel;
    public RectTransform partsPanel;

    public void Start()
    {
        fight.onClick.AddListener(StartGame);
        newPart.onClick.AddListener(CreateNewPart);
        combineParts.onClick.AddListener(() => state = 0);
        breakPiece  .onClick.AddListener(() => state = 1);
        promotePiece.onClick.AddListener(() => state = 2);
        demotePiece .onClick.AddListener(() => state = 3);
        partInfo    .onClick.AddListener(() => state = 4);

        for (int i = 0; i < 3; ++i) {
            Garage.CreateNewTeam();
            for (int j = 0; j < 6; ++j) {
                Garage.CreateRandomPiece();
                Garage.AddToTeam(0, i);
            }
        }

        for (int i = 0; i < 3; ++i) Garage.CreateRandomPiece();
        for (int i = 0; i < 3; ++i) Garage.CreateRandomPart();

        Garage.Save();

        LoadTeamPanel();
        LoadPiecesPanel();
        LoadPartsPanel();
    }

    private int state = -1;
    private List<GameObject> objects = new List<GameObject>();
    public void Update() {
        if (state != -1 && Input.GetMouseButtonUp(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit)) {
                GameObject clicked = rayHit.collider.gameObject;
                if (clicked != null) objects.Add(clicked);
            }
            if (state == 0 && objects.Count == 3) {
                bool allParts = true;
                for (int i = 0; i < 3; ++i) {
                    if (objects[i].transform.parent.gameObject.name != "Parts Panel") {
                        allParts = false;
                    }
                }
                if (allParts) {
                    int found = 0;
                    int[] seeds = new int[3]{-1,-1,-1};
                    for (int i = 0; i < 3; ++i) {
                        int seed = objects[i].GetComponent<Part>().seed;
                        int type = seed % 3;
                        if (seeds[type] == -1) {
                            seeds[type] = seed;
                            found++;
                        }
                    }
                    if (found == 3) {
                        Garage.Load();
                        Garage.CombineParts(seeds);
                        Garage.Save();

                        LoadPiecesPanel();
                        LoadPartsPanel();
                    }
                }
                objects = new List<GameObject>();
                state = -1;
            }
            else if (state == 1 && objects.Count == 1) {
                if (objects[0].name == "Bot" || 
                    objects[0].name == "Mid" || 
                    objects[0].name == "Top") {
                    GameObject piece = objects[0].transform.parent.gameObject;
                    string location = piece.transform.parent.gameObject.name;
                    if (location == "Pieces Panel") {
                        List<int> seeds = new List<int>();
                        foreach (Transform part in piece.transform) {
                            seeds.Add(part.gameObject.GetComponent<Part>().seed);
                        }

                        Garage.Load();
                        for (int i = 0; i < Garage.GetPieceCount(); ++i) {
                            int[] test = Garage.GetPiece(i);
                            if (test[0] == seeds[0] && 
                                test[1] == seeds[1] && 
                                test[2] == seeds[2]) {
                                Garage.SplitPiece(i);
                                break;
                            }
                        }
                        Garage.Save();

                        LoadPiecesPanel();
                        LoadPartsPanel();
                    }
                }
                objects = new List<GameObject>();
                state = -1;
            }
            else if (state == 2 && objects.Count == 1) {
                if (objects[0].name == "Bot" || 
                    objects[0].name == "Mid" || 
                    objects[0].name == "Top") {
                    GameObject piece = objects[0].transform.parent.gameObject;
                    string location = piece.transform.parent.gameObject.name;
                    if (location == "Pieces Panel") {
                        List<int> seeds = new List<int>();
                        foreach (Transform part in piece.transform) {
                            seeds.Add(part.gameObject.GetComponent<Part>().seed);
                        }

                        Garage.Load();
                        if (Garage.GetTeam(0).Count < 6) {
                            for (int i = 0; i < Garage.GetPieceCount(); ++i) {
                                int[] test = Garage.GetPiece(i);
                                if (test[0] == seeds[0] && 
                                    test[1] == seeds[1] && 
                                    test[2] == seeds[2]) {
                                    Garage.AddToTeam(i, 0);
                                    break;
                                }
                            }
                            Garage.Save();

                            LoadTeamPanel();
                            LoadPiecesPanel();
                        }
                    }
                }
                objects = new List<GameObject>();
                state = -1;
            }
            else if (state == 3 && objects.Count == 1) {
                if (objects[0].name == "Bot" || 
                    objects[0].name == "Mid" || 
                    objects[0].name == "Top") {
                    GameObject piece = objects[0].transform.parent.gameObject;
                    string location = piece.transform.parent.gameObject.name;
                    if (location == "Team Panel") {
                        List<int> seeds = new List<int>();
                        foreach (Transform part in piece.transform) {
                            seeds.Add(part.gameObject.GetComponent<Part>().seed);
                        }

                        Garage.Load();
                        List<int[]> team = Garage.GetTeam(0);
                        for (int i = 0; i < team.Count; ++i) {
                            if (team[i][0] == seeds[0] && 
                                team[i][1] == seeds[1] && 
                                team[i][2] == seeds[2]) {
                                Garage.RemoveFromTeam(0, i);
                                break;
                            }
                        }
                        Garage.Save();

                        LoadTeamPanel();
                        LoadPiecesPanel();
                    }
                }
                objects = new List<GameObject>();
                state = -1;
            }
            else if (state == 4 && objects.Count == 1) {
                if (objects[0].name == "Bot" || 
                    objects[0].name == "Mid" || 
                    objects[0].name == "Top") {
                    Part part = objects[0].GetComponent<Part>();

                    partInfoDescription.text = part.description;

                    RectTransform partTransform = 
                        CreatePart(0, part.seed).AddComponent<RectTransform>();
                    foreach (Transform child in partInfoIconPanel.gameObject.transform) Destroy(child.gameObject);
                    partTransform.SetParent(partInfoIconPanel);

                    float scale = partInfoIconPanel.rect.width * 0.9f;
                    float height = partInfoIconPanel.rect.height * 0.9f;
                    partTransform.localScale = new Vector3(scale, scale, scale);
                    partTransform.localPosition = new Vector3(0, 0, 0);
                    partTransform.Rotate(90, 0, 0);

                    partTransform.anchorMax = new Vector2(0.9f, 0.9f);
                    partTransform.anchorMin = new Vector2(0.1f, 0.1f);
                    partTransform.offsetMax = Vector2.zero;
                    partTransform.offsetMin = Vector2.zero;
                }
                objects = new List<GameObject>();
                state = -1;
            }
        }
    }

    private void LoadTeamPanel() {
        foreach (Transform child in teamPanel) {
            GameObject.Destroy(child.gameObject);
        }

        Garage.Load();

        for (int i = 0; i < Garage.GetTeam(0).Count; ++i) {
            int[] seeds = Garage.GetTeam(0)[i];

            RectTransform pieceTransform = 
                CreatePiece(0, seeds).AddComponent<RectTransform>();
            pieceTransform.SetParent(teamPanel);

            float scale = teamPanel.rect.width / 5;
            pieceTransform.localScale = new Vector3(scale, scale, scale);
            pieceTransform.localPosition = new Vector3(0, 0, 0);
            pieceTransform.Rotate(90, 0, 0);

            pieceTransform.anchorMax = 
                new Vector2((float)(i%3+1)/3, 1-(Mathf.Floor(i/3)+1)/2);
            pieceTransform.anchorMin = 
                new Vector2((float)(i%3)  /3, 1-(Mathf.Floor(i/3))  /2);
            pieceTransform.offsetMax = Vector2.zero;
            pieceTransform.offsetMin = Vector2.zero;
        }
    }

    private void LoadPiecesPanel() {
        foreach (Transform child in piecesPanel) {
            GameObject.Destroy(child.gameObject);
        }

        float height = piecesPanel.parent.GetComponent<RectTransform>().rect.height;
        float width = piecesPanel.rect.width;
        float scale = width / 5;
        float height_ = Mathf.Max(height, (Mathf.Floor((Garage.GetPieceCount()-1)/3)+1)*height/2);
        piecesPanel.sizeDelta = new Vector2(0, height_ - height);

        Garage.Load();

        for (int i = 0; i < Garage.GetPieceCount(); ++i) {
            int[] seeds = Garage.GetPiece(i);

            RectTransform pieceTransform = 
                CreatePiece(0, seeds).AddComponent<RectTransform>();
            pieceTransform.SetParent(piecesPanel);

            
            pieceTransform.localScale = new Vector3(scale, scale, scale);
            pieceTransform.localPosition = new Vector3(0, 0, 0);
            pieceTransform.Rotate(90, 0, 0);

            pieceTransform.anchorMax = 
                new Vector2((float)(i%3+1)/3, 1-(Mathf.Floor(i/3)+1)/2*height/height_);
            pieceTransform.anchorMin = 
                new Vector2((float)(i%3)  /3, 1-(Mathf.Floor(i/3))  /2*height/height_);
            pieceTransform.offsetMax = Vector2.zero;
            pieceTransform.offsetMin = Vector2.zero;
        }
    }

    private void LoadPartsPanel() {
        foreach (Transform child in partsPanel) {
            GameObject.Destroy(child.gameObject);
        }

        float height = partsPanel.parent.GetComponent<RectTransform>().rect.height;
        float width = partsPanel.rect.width;
        float scale = width / 5;
        float height_ = Mathf.Max(height, (Mathf.Floor((Garage.GetPartCount()-1)/3)+1)*height/6);
        partsPanel.sizeDelta = new Vector2(0, height_ - height);

        Garage.Load();

        for (int i = 0; i < Garage.GetPartCount(); ++i) {
            int seed = Garage.GetPart(i);

            RectTransform partTransform = 
                CreatePart(0, seed).AddComponent<RectTransform>();
            partTransform.SetParent(partsPanel);

            
            partTransform.localScale = new Vector3(scale, scale, scale);
            partTransform.localPosition = new Vector3(0, 0, 0);
            partTransform.Rotate(90, 0, 0);

            partTransform.anchorMax = 
                new Vector2((float)(i%3+1)/3, 1-(Mathf.Floor(i/3)+1)/6*height/height_);
            partTransform.anchorMin = 
                new Vector2((float)(i%3)  /3, 1-(Mathf.Floor(i/3))  /6*height/height_);
            partTransform.offsetMax = Vector2.zero;
            partTransform.offsetMin = Vector2.zero;
        }
    }

    private void StartGame() {
        Garage.Load();
        if (Garage.GetTeam(0).Count == 6) {
            SceneManager.LoadScene(combatScene);
        }
    }

    private void CreateNewPart() {
        Garage.CreateRandomPart();
        Garage.Save();
        LoadPartsPanel();
    }
}
