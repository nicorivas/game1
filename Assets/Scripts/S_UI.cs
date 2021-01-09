using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_UI : MonoBehaviour
{
    static S_UI instance;
    GameObject canvasHearts;
    Text timeText;
    Text scoreText;
    GameObject world;
    static GameObject gameOverPanel;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        timeText = GameObject.Find("Time").GetComponent<Text>(); 
        timeText.text = "";
        scoreText = GameObject.Find("Score").GetComponent<Text>(); 
        scoreText.text = "";
        canvasHearts = GameObject.Find("/Canvas/Hearts");
        gameOverPanel = GameObject.Find("/Canvas/GameOver");
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float minutes = Mathf.FloorToInt(S_World.time / 60);
        float seconds = Mathf.FloorToInt(S_World.time % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        scoreText.text = string.Format("{0:0000000}", S_World.score);
    }

    static public void ShowDescriptionMessage(string description) {
        GameObject.Find("/Canvas/PowerDescription").GetComponent<CanvasGroup>().alpha = 1f;
        GameObject.Find("/Canvas/PowerDescription").GetComponent<CanvasGroup>().blocksRaycasts = true;
        Text descriptionText = GameObject.Find("/Canvas/PowerDescription/PowerDescriptionText").GetComponent<Text>();
        descriptionText.text = description;
        S_World.events.Add(new Event(instance.gameObject, Config.Ticks_Description_Message, HideDescriptionMessage));
    }

    static public void HideDescriptionMessage() {
        GameObject.Find("/Canvas/PowerDescription").GetComponent<CanvasGroup>().alpha = 0f;
        GameObject.Find("/Canvas/PowerDescription").GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    static public void GameOver() {
        gameOverPanel.SetActive(true);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
