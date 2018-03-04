using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : MonoBehaviour {

    public enum Pin { LEFT, MIDDLE, RIGHT };

    public int numberOfRings = 4;
    public GameObject winScreen;

    //HashSets to represent the pins, this ensures there cannot be duplicate entries of a ring on the same pin
    private HashSet<int> leftPin = new HashSet<int>();
    private HashSet<int> middlePin = new HashSet<int>();
    private HashSet<int> rightPin = new HashSet<int>();
    private float timeOfRightPinPlacement;
    private bool gameWon = false;

    //Fill the left pin with rings when the game i started
    void Start() {
        for (int i = 1; i < numberOfRings + 1; i++) {
            leftPin.Add(i);
        }
    }

    void Update() {
        WinCondition();
    }

    //Check if a ring can be moved by first checking if the pin it's placed on contains a smaller ring,
    //if it does not or if it's not placed a pin, check that all other rings are placed on a pin.
    public bool IsRingMovable(int ring) {

        if (leftPin.Contains(ring) && DoesPinContainSmallerRing(ring, leftPin)) {
            return false;
        } else if (middlePin.Contains(ring) && DoesPinContainSmallerRing(ring, middlePin)) {
            return false;
        } else if (rightPin.Contains(ring) && DoesPinContainSmallerRing(ring, rightPin)) {
            return false;
        }

        return AllRingsPlaced(ring);
    }

    //Add the given ring to the given pin
    public void AddRingToPin(int ring, Pin pin) {
        //Debug.Log("Add rubg: " + ring + " to pin: " + pin);
        switch (pin) {
            case Pin.LEFT:
                leftPin.Add(ring); break;
            case Pin.MIDDLE:
                middlePin.Add(ring); break;
            case Pin.RIGHT:
                rightPin.Add(ring);
                timeOfRightPinPlacement = Time.time;
                break;
        }

    }

    //Remove the given ring from the given pin
    public void RemoveRingFromPin(int ring, Pin pin) {
        //Debug.Log("Remove rubg: " + ring + " from pin: " + pin);
        switch (pin) {
            case Pin.LEFT:
                leftPin.Remove(ring); break;
            case Pin.MIDDLE:
                middlePin.Remove(ring); break;
            case Pin.RIGHT:
                rightPin.Remove(ring); break;
        }

    }

    //Check if the given ring can be placed on the given pin by checking if there is a smaller ring present on the pin
    public bool IsRingPlacable(int ring, Pin pin) {

        switch (pin) {
            case Pin.LEFT:
                if(DoesPinContainSmallerRing(ring, leftPin)) {
                    return false;
                }
                break;
            case Pin.MIDDLE:
                if (DoesPinContainSmallerRing(ring, middlePin)) {
                    return false;
                }
                break;
            case Pin.RIGHT:
                if (DoesPinContainSmallerRing(ring, rightPin)) {
                    return false;
                }
                break;
        }
        return true;

    }

    //Checks if all rings except the given one is placed on a pin, end search early if a ring is not placed
    public bool AllRingsPlaced(int currentRing) {
        bool allRingsPlacad = true;

        for(int i = 1; i < numberOfRings+1; i++) {

            if(i != currentRing) {
                allRingsPlacad = leftPin.Contains(i) || middlePin.Contains(i) || rightPin.Contains(i);
            }

            if (!allRingsPlacad) {
                return false;
            }

        }

        return true;
    }

    //Help method to compare two rings
    private bool DoesPinContainSmallerRing(int ringToCompare, HashSet<int> pin) {
        foreach (int placedRing in pin) {

            if (ringToCompare > placedRing) {
                return true;
            }

        }
        return false;
    }

    //Win the game if 4 rings have been placed on the right pin for more than one second
    private void WinCondition() {

        if (rightPin.Count > 3 && Time.time - 1  > timeOfRightPinPlacement && !gameWon) {
            winScreen.SetActive(true);
            gameWon = true;
        }
    }

    public bool GameWon() {
        return gameWon;
    }

}
