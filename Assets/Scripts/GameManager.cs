using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Read Only")]
    public bool controlsEnabled;
    public float horizontal, jump;

    [Header("Starting Data Values")]
    public int startOrbCount = 0;
    public int startLifeCount = 0;

    GameObject orbObject;

    void Start() {
        // Find child objects to create clones from
        foreach (Transform child in transform) {
            if (child.CompareTag("Orb")) {
                orbObject = child.gameObject;
            }
        }

        if (orbObject == null) Debug.LogWarning("Origunal Orb GameObject not found.");
        else ObjectCreator.SetOrbObject(orbObject);

        DataHandler.SetLifeCount(startLifeCount);
        DataHandler.SetOrbCount(startOrbCount);

        // Ignore collisions between collectibles and enemies
        Physics2D.IgnoreLayerCollision(11, 12, true);
        // Ignore collisions between collectibles
        Physics2D.IgnoreLayerCollision(11, 11, true);

        AudioSource sfx = GameObject.Find("PlayerHitSFX").GetComponent<AudioSource>();
        SoundHandler.SetAudioSource(sfx);
    }

    void FixedUpdate() {
        InputHandler.setHorizontal(Input.GetAxis("Horizontal"));
        InputHandler.setJump(Input.GetAxis("Jump"));
        InputHandler.setThrow(Input.GetAxis("Fire1"));

        //StateHandler.updateState();

        // Update values so they can be seen in editor
        controlsEnabled = InputHandler.isControlsEnabled();
        horizontal = InputHandler.getHorizontal();
        jump = InputHandler.getJump();
    }



    public static class ObjectCreator {
        static GameObject orb;

        // Creates a non kinematic orb subject to external forces
        public static GameObject createOrb(Vector3 position, Quaternion rotation) {
            if (orb == null) {
                Debug.LogWarning("Original orb not found, cannot create requested orb");
                return null;
            }

            GameObject orbClone = GameObject.Instantiate(orb, position, rotation);
            orbClone.SetActive(true);
            return orbClone;
        }

        public static void SetOrbObject(GameObject obj) { orb = obj; }
    }


    public static class SoundHandler {
        static AudioSource playerHit;

        public static void SetAudioSource(AudioSource playerHitSFX) {
            playerHit = playerHitSFX;
        }

        public static void StartPlayerHitSFX() {
            playerHit.PlayOneShot(playerHit.clip);
        }
    }



    // Holds onto the input variable results that other objects can use
    // each game loop
    public static class InputHandler {
        static bool capThrow;
        static float horizontal;
        static float jump;
        static bool controlsEnabled = true;
        static bool[] jumpLog = new bool[2];

        public static float getHorizontal() {
            if (controlsEnabled) return horizontal;
            return 0;
        }

        public static float getJump() {
            if (controlsEnabled) return jump;
            return 0;
        }

        public static bool getThrow() {
            if (controlsEnabled) return capThrow;
            return false;
        }

        public static bool jumpPressed() { return controlsEnabled && jump == 1; }

        public static void setHorizontal(float h) { horizontal = h; }
        public static void setJump(float j) {
            jump = j;
            jumpLog[0] = jumpLog[1];
            jumpLog[1] = jumpPressed();
        }

        public static void enableControls() { controlsEnabled = true; }
        public static void disableControls() { controlsEnabled = false; }
        public static bool isControlsEnabled() { return controlsEnabled; }

        public static void setThrow(float value) {
            if (value > 0.5f) capThrow = true;
            else capThrow = false;
        }
    }





    // Contains all data the game will rely on other than controller input 
    // data and provides the functionality to manipulate these values in 
    // various ways
    public static class DataHandler {
        static float score;
        static float orbCount;
        static float lifeCount;
        static bool playerAlive;
        static bool playerHit;

        // Increments orb count by 1
        public static void incrementOrbCount() {
            orbCount++;
            score += 15;
        }

        // Resets the orb count to zero
        public static void zeroOrbCount() {
            orbCount = 0;
        }

        public static void incrementPlayerLives() {
            lifeCount++;
            score += 100;
        }

        // When player is hit, disable ability for them to 
        // gather collectibles until they regain control. Also prevent
        // enemies from targeting or hitting them.
        public static void SetPlayerHit() {
            playerHit = true;
            Physics2D.IgnoreLayerCollision(10, 11, true);
            Physics2D.IgnoreLayerCollision(10, 12, true);
        }

        public static void SetPlayerRecovered() {
            playerHit = false;
            Physics2D.IgnoreLayerCollision(10, 11, false);
            Physics2D.IgnoreLayerCollision(10, 12, false);
        }

        // Sets the playerAlive to false
        public static void SetPlayerDead() { playerAlive = false; }
        // Sets the playerAlive to false
        public static void SetPlayerAlive() { playerAlive = true; }

        public static bool isPlayerAlive() {
            return playerAlive;
        }

        public static void SetOrbCount(float amt) { orbCount = amt; }
        public static void SetLifeCount(float amt) { lifeCount = amt; }

        public static float getOrbCount() { return orbCount; }
        public static float getScore() { return score; }
        public static float getLifeCount() { return lifeCount; }

        public static bool GetPlayerHitStatus() { return playerHit; }

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
        static GameState current;

        public static GameState getCurrent() { return current; }

        // Sets the state of the game
        public static void setState(GameState state) { current = state; }

        // Runs every update to check whether the state of the game 
        // should change based on requested state and values. 
        public static void updateState() {

            if (!DataHandler.isPlayerAlive() && current == GameState.GAMEPLAY) {
                current = GameState.LOAD_START;
            }
            
            switch (current) {
                case GameState.LOAD_START: {
                        //TODO Disable player controls
                        InputHandler.disableControls();
                        //TODO start loading animation if not running

                        //TODO If animation is done, switch to LOADING state
                        current = GameState.LOADING;
                        break;
                    }
                case GameState.LOADING: {
                        //TODO Show loading screen if not running

                        //TODO Perform loading tasks
                        LevelHandler.loadLevel();

                        //TODO When loading is complete, switch to LOAD_END state
                        current = GameState.LOAD_END;
                        break;
                    }
                case GameState.LOAD_END: {

                        //TODO Start load end animation if not running

                        //TODO If animation is done, switch to proper state and
                        //     enable player controls
                        current = GameState.GAMEPLAY;
                        InputHandler.enableControls();
                        break;
                    }
                default: {
                        print("ERROR: StateHandler reached default case " + 
                              "which should not be possible!");
                        break;
                    }   
            }

        }
    }


    public static class LevelHandler {

        public enum Level {
            TEST,
            STAGE_ONE
        }

        private static Level current = Level.TEST;

        private static void loadTestStage() {
            // Return if the level is already loaded
            if (current == Level.TEST) return;

        }

        private static void loadStageOne() {
            // Return if the level is already loaded
            if (current == Level.STAGE_ONE) return;

        }

        public static void loadLevel() {
            switch (current) {
                case Level.TEST: loadTestStage();
                    break;
                case Level.STAGE_ONE: loadStageOne();
                    break;
                default:
                    break;
            }
        }

    }
    
}
