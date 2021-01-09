using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Power : MonoBehaviour
{
    protected string description;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            Action();
            ShowDescription();
            EliminateOthers();
            Eliminate();
        }
    }

    public void Eliminate() {
        Destroy(gameObject);
    }

    private void EliminateOthers() {
        GameObject[] powers = GameObject.FindGameObjectsWithTag("Power");
        foreach (GameObject power in powers) {
            power.GetComponent<S_Power>().Eliminate();
        }
    }

    private void ShowDescription() {
        S_UI.ShowDescriptionMessage(description);
    }

    public virtual void Action() { }
}
