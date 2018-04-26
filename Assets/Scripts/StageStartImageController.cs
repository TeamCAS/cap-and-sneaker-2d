using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageStartImageController : MonoBehaviour {

    public bool growing = false;
    public float growDuration = 2;
    public float shrinkDuration = 2;
    public bool transformWidth = false;
    public bool transformHeight = false;

    float origHeight;
    float origWidth;

    RectTransform rectTrans;

    void Start () {
        rectTrans = transform.GetComponent<RectTransform>();
        origHeight = rectTrans.rect.height;	
        origWidth = rectTrans.rect.width;
        transformStartTime = Time.time;
    }

    float transformStartTime = -1;

	// Update is called once per frame
	void Update () {

        if (growing) {
            float elapsed = Time.time - transformStartTime;
            float height = Mathf.Lerp(0, origHeight, elapsed / growDuration);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        } else {
            float elapsed = Time.time - transformStartTime;
            float height = Mathf.Lerp(origHeight, 0, elapsed / shrinkDuration);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

	}
}
