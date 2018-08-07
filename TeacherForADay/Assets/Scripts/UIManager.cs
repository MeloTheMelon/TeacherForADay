using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject gameLoopUI;
    public GameObject endScreenBackground;
    public GameObject endScreen;
    public GameObject highscoreText;
    public Text scoreText;
    public GameObject highscoreListObject;
    public GameObject enterNameObject;
    public InputField nameInput;
    public Text highscoreTable;

    List<Highscore> highscores = new List<Highscore>();


	// Use this for initialization
	void Start () {
        nameInput.characterLimit = 3;
        string highscoresString = ReadString();
        string[] temp = highscoresString.Split('♪');
        foreach(string s in temp)
        {
            string[] temp2 = s.Split('◘');
            highscores.Add(new Highscore(temp2[0], int.Parse(temp2[1])));
        }
        highscores.Sort((p1, p2) => p1.score.CompareTo(p2.score));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Start Menu

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    //End Screen

    public void openEndScreen(int score)
    {
        gameLoopUI.SetActive(false);
        endScreen.SetActive(true);
        endScreenBackground.SetActive(true);
        endScreenBackground.SetActive(true);
        if (checkForNewHighscore("µ-µ", score))
        {
            Debug.Log("New Highscore");
            highscoreText.SetActive(true);
            StartCoroutine(waitForHighscoreList());
        }
        scoreText.text = "Score: " + score;
    }

    bool checkForNewHighscore(string name, int score)
    {
        bool newHighscore = false;
        Highscore newPlayer = new Highscore(name, score);
        highscores.Add(newPlayer);
        highscores.Sort((p2, p1) => p1.score.CompareTo(p2.score));
        if (highscores[highscores.Count-1] != newPlayer)
        {
            newHighscore = true;
        }
        highscores.RemoveAt(highscores.Count-1);
        return newHighscore;

    }

    IEnumerator waitForHighscoreList()
    {
        yield return new WaitForSeconds(5f);
        endScreen.SetActive(false);
        enterNameObject.SetActive(true);
    }

    public void submitNameButton()
    {
        string playerName = nameInput.text;
        string saveText = "";
        highscoreTable.text = "";
        foreach(Highscore h in highscores)
        {
            if (h.name.Equals("µ-µ"))
            {
                h.name = playerName;
            }
            highscoreTable.text += h.name + ": " + h.score+'\n';
            saveText += h.name + "◘" + h.score + "♪";
        }
        saveText = saveText.Substring(0, saveText.Length - 1);
        enterNameObject.SetActive(false);
        highscoreListObject.SetActive(true);
        Debug.Log(saveText);
        WriteString(saveText);
    }

    public void exitScoreScreenButton()
    {
        SceneManager.LoadScene(0);
    }

    static void WriteString(string text)
    {
        string path = "Assets/Resources/highscores.txt";

        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(text);
        writer.Flush();
        writer.Close();

    }

    static string ReadString()
    {
        string path = "Assets/Resources/highscores.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string text = reader.ReadToEnd();
        reader.Close();
        return text;
    }


}

public class Highscore
{
    public string name;
    public int score;

    public Highscore(string playerName, int playerScore)
    {
        name = playerName;
        score = playerScore;
    }

}

