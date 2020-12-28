using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_Hearts : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    void Start()
    {
        hearts = new List<GameObject>();
        for (int i=0; i<(int)Player.maxHearts; i++) {
            Debug.Log(GameObject.Find("H"+(i+1)));
            hearts.Add(GameObject.Find("H"+(i+1)));
        }    
    }

    // Update is called once per frame
    void Update()
    {
        int nhearts = (int)Player.GetHealth();
        int count = 0;
        foreach (GameObject heart in hearts)
        {
            if  (count < nhearts) {
                heart.GetComponent<Image>().sprite = fullHeart;
            } else {
                heart.GetComponent<Image>().sprite = emptyHeart;
            }
            count++;
        }
    }
}
