using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    
    void FixedUpdate() {
        InputHandler.horizontal = Input.GetAxis("Horizontal");
        InputHandler.jump = Input.GetAxis("Jump");
    }

    // Holds onto the input variable results that other objects can use
    // each game loop
    public static class InputHandler {
        public static float horizontal;
        public static float jump;
    }

}
