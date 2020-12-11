using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_UI : MonoBehaviour
{
    Text txt;
    Text selectedObject;
    GameObject world;
    static GameObject gameOverPanel;
    // Start is called before the first frame update
    void Start()
    {
        txt = GameObject.Find("Status Text").GetComponent<Text>(); 
        selectedObject = GameObject.Find("SelectedObject").GetComponent<Text>();
        txt.text = "";
        gameOverPanel = GameObject.Find("/Canvas/GameOver");
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = "Life: " + Player.instance.GetHealth();
        txt.text = "Month: "+S_World.month+" Day: "+S_World.day + " Hour: " + S_World.hour;
        txt.text += "  ";
        txt.text += "Fire: " + S_World.GetFireEnergy();
        txt.text += "  ";
        txt.text += "Life: " + S_World.GetLifeEnergy();
        GameObject cObject = null;
        if (Player.IsHoldingObject()) {
            cObject = Player.GetHeldObject();
        } else if (S_World.selectedObject != null) {
            cObject = S_World.selectedObject;
        }
        if (cObject != null) {
            S_TerrainObject heldObjectScript = cObject.GetComponent<S_TerrainObject>();
            string name = heldObjectScript.GetName();
            selectedObject.text = name;
            if (name == "Rock" || name == "Magma" || name == "Volcano") {
                selectedObject.text += "\n";
                selectedObject.text += "Type: "+heldObjectScript.GetObjectType();
                selectedObject.text += "\n";
                selectedObject.text += "Life: "+heldObjectScript.GetLifeEnergy();
                selectedObject.text += "\n";
                selectedObject.text += "Fire: "+heldObjectScript.GetFireEnergy();
            }
        } else {
            selectedObject.text = "None";
        }
    }

    static public void GameOver() {
        gameOverPanel.SetActive(true);
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
