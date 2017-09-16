using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    void FixedUpdate() {
        InputHandler.horizontal = Input.GetAxis("Horizontal");
        InputHandler.jump = Input.GetAxis("Jump");

        if (DataHandler.isPlayerAlive()) {
            StateHandler.setState(StateHandler.GameState.LOAD_START);
        }
    }

    // Holds onto the input variable results that other objects can use
    // each game loop
    public static class InputHandler {
        public static float horizontal;
        public static float jump;
    }

    // Contains all data the game will rely on other than controller input 
    // data and provides the functionality to manipulate these values in 
    // various ways
    public static class DataHandler {
        static float orbCount;
        static bool playerAlive;

        // Increments orb count by 1
        public static void incrementOrbCount() {
            orbCount++;
            print("orbcount = " + orbCount);
        }

        // Resets the orb count to zero
        public static void zeroOrbCount() {
            orbCount = 0;
        }

        // Sets the playerAlive to false
        public static void playerDied() {
            playerAlive = false;
        }

        public static bool isPlayerAlive() {
            return playerAlive;
        }

    }

    // Keeps track of state the game is currently in and which state 
    // it’s switching from/to. State will change based on the 
    // state of current values.
    public static class StateHandler {
        public enum GameState {
            GAMEPLAY,
            LOAD_START,
            LOADING,
            LOAD_END
        }

        // The current state the whole game is in
        private static GameState current;

        // Sets the state of the game
        public static void setState(GameState state) {
            current = state;
        }

        // Runs every update to check whether the state of the game 
        // should change based on requested state and values. 
        public static void updateState() {

            switch (current) {
                case GameState.LOAD_START: {
                        //TODO Disable player controls

                        //TODO start loading animation if not running

                        //TODO If animation is done, switch to next 
                        //     level and switch to that state
                        break;
                    }
                default: {

                        break;
                    }   
            }

        }
    }
}
