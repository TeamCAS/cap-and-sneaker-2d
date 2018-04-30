using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageStartImageController : MonoBehaviour {

    [Header("Delay before starting the modifications below")]
    public float startDelay = 0;

    public enum OpacityAction { None, Fade, Appear }
    [Header("Settings to modify opacity over time")]
    public OpacityAction opacityAction = OpacityAction.None;
    public float opacityDuration = 1;

    public enum SizeActions { None, Grow, Shrink }
    [Header("Settings to transform size over time")]
    public SizeActions sizeActions = SizeActions.None;
    public float sizeDuration= 1;
    public bool transformWidth = false;
    public bool transformHeight = false;

    [Header("Self destruct after x seconds, -1 to disable")]
    public float timeToSelfDestruct = -1;

    float origHeight;
    float origWidth;

    RectTransform rectTrans;
    Image img;

    void Awake() {
        triggeredTime = Time.time;
        rectTrans = transform.GetComponent<RectTransform>();
        origHeight = rectTrans.rect.height;
        origWidth = rectTrans.rect.width;
        img = GetComponent<Image>();
    }

    void Start () {
        if (sizeActions == SizeActions.Grow) {
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        }
    }

    // When the animation was triggered
    float triggeredTime;

    public bool complete = false;

    public bool isComplete() { return complete; }

    bool destroyingSelf = false;

	// Update is called once per frame
	void Update () {
        if (startDelay < Time.time - triggeredTime && !complete) {
            UpdateOpacity();
            UpdateSize();
        }
        complete = true;
        if (sizeActions != SizeActions.None) complete = complete && sizeComplete;
        if (opacityAction != OpacityAction.None) complete = complete && opacityComplete;

        if (!destroyingSelf && complete && timeToSelfDestruct >= 0) {
            destroyingSelf = true;
            Destroy(transform.parent.gameObject, timeToSelfDestruct);
        }
    }

    bool sizeComplete = false;
    float sizeStartTime = -1;
    public void UpdateSize() {
        if (sizeStartTime == -1) {
            sizeStartTime = Time.time;
        }
        else {
            float progress = Time.time - sizeStartTime;
            progress /= sizeDuration;
            float height = 0, width = 0;
            if (sizeActions == SizeActions.Grow) {
                height = Mathf.Lerp(0, origHeight, progress);
                width = Mathf.Lerp(0, origWidth, progress);
                // Reset dimensions so that if only one axis is growing, the other will be back
                // at its original size
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, origHeight);
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, origWidth);
            }
            else if (sizeActions == SizeActions.Shrink) {
                height = Mathf.Lerp(origHeight, 0, progress);
                width = Mathf.Lerp(origWidth, 0, progress);
            }
            if (transformHeight) {
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
            if (transformWidth) {
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }

            if (progress >= 1) sizeComplete = true;

        }
    }

    bool opacityComplete = false;
    float opacityStartTime = -1;
    public void UpdateOpacity() {
        if (opacityAction != OpacityAction.None) { 
            if (opacityStartTime == -1) {
                opacityStartTime = Time.time;
            }
            else {
                float progress = Time.time - opacityStartTime;
                progress /= opacityDuration;
                if (opacityAction == OpacityAction.Appear) {
                    img.color = Color.Lerp(Color.clear, Color.black, progress);
                }
                else if (opacityAction == OpacityAction.Fade) {
                    img.color = Color.Lerp(Color.black, Color.clear, progress);
                }

                if (progress >= 1) opacityComplete = true;
            }
        }
    }
    
}
