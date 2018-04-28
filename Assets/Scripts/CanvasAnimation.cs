using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAnimation : MonoBehaviour {

    public bool complete = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        bool status = true;
		foreach(Transform child in transform) {
            StageStartImageController[] canvasAnim;
            canvasAnim = child.GetComponents<StageStartImageController>();
            if (canvasAnim == null || canvasAnim.Length == 0) {
                foreach(Transform grandChild in child) {
                    canvasAnim = grandChild.GetComponents<StageStartImageController>();
                    foreach (var item in canvasAnim) {
                        status = status && item.isComplete();
                    }
                }
            } else {
                foreach (var item in canvasAnim) {
                    status = status && item.isComplete();
                }
            }
        }
        complete = status;
        //if (complete) Debug.Break();
        if (complete) {
            GameManager.CanvasAnimationHandler.End(); ;
        }
	}
}
