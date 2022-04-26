using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public string combatScene = "Combat";

    public void Start()
    {
        for (int i = 0; i < 3; ++i) {
            Garage.CreateNewTeam();
            for (int j = 0; j < 6; ++j) {
                Garage.CreateRandomPiece();
                Garage.AddToTeam(0, i);
            }
        }
        Garage.Save();
        SceneManager.LoadScene(combatScene);
    }
}
