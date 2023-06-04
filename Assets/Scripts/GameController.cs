using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{

    public GameObject scorePanel;
    public TextMeshProUGUI highScore;
    // Start is called before the first frame update
    public void endGame()
	{
        displayScorePanel();
        PaintingController.ableToPaint = false;
	}

    public void displayScorePanel()
	{
        highScore.text = Scoring.highScore.ToString();
        scorePanel.SetActive(true);
    }

    public void reloadScene()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
