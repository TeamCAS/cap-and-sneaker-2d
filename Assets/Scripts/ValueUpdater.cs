using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueUpdater : MonoBehaviour {

    public string valueName = "";

    Text text;

    void Start() {
        text = transform.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        if (valueName == "orb") {
            text.text = GameManager.DataHandler.getOrbCount().ToString();
        }
        else if (valueName == "score") {
            text.text = GameManager.DataHandler.getScore().ToString();
        }
        else {
            text.text = "ERROR";
        }
	}
}
