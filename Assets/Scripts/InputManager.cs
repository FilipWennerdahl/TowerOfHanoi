using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    //Based on a script found here:
    //https://answers.unity.com/questions/1126621/best-way-to-detect-touch-on-a-gameobject.html

    private bool mousePresent;
    private Vector3 inputDelta;
    private Vector3 inputPositionWorld;
    private Vector3 lastInputPosition;
    private Vector3 lastTouchPosition;
    private GameObject pressedObject;


    void Start() {
        mousePresent = Input.mousePresent;
        lastTouchPosition = Vector3.zero;
        Debug.Log(Input.simulateMouseWithTouches);
    }

    void Update () {
        CalculateInputDelta();

        if(mousePresent) {
            MouseInput();
        } else {
            TouchInput();
        }

    }

    public Vector3 GetInputDelta() {
        return inputDelta;
    }

    //Returns the input position depeding if you have a mouse available 
    public Vector2 GetInputPosition() {
        if (mousePresent) {
            return Input.mousePosition;
        }
        if (Input.touchCount > 0) {
            lastTouchPosition = Input.GetTouch(0).position;
        }
        return lastTouchPosition;
    }

    public Vector3 DirectionTowardsInput(Vector3 position) {
        return Camera.main.ScreenToWorldPoint(GetInputPosition()) - position;
    }

    //Calculate the mouse delta based on the movement of the mouse cursor or the touch
    private void CalculateInputDelta() {

        if (mousePresent) {
            float distanceToScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            Vector3 currentInputPosition = Camera.main.ScreenToWorldPoint(new Vector3(GetInputPosition().x, GetInputPosition().y, distanceToScreen));
            inputDelta = (currentInputPosition - lastInputPosition) / Time.deltaTime;
            lastInputPosition = currentInputPosition;
        } else if (Input.touchCount > 0) {
            inputDelta = Input.GetTouch(0).deltaPosition / 2;
        }

    }

    //Do a raycast if pressing left mouse button, otherwise let go of the ring
    private void MouseInput() {

        if (Input.GetMouseButtonDown(0)) {

            PerformRaycast();

        } else if (pressedObject != null && Input.GetMouseButtonUp(0)) {
            pressedObject.GetComponent<RingScript>().RingUnmovable();
            pressedObject = null;
        }
    }

    //Do a raycast if touching the screen, otherwise letting go drop the ring
    private void TouchInput() {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {

            PerformRaycast();

        } else if(Input.touchCount < 1 && pressedObject != null) {
            pressedObject.GetComponent<RingScript>().RingUnmovable();
            pressedObject = null;
        }

    }

    //Perform a raycasst att the current pressed input position, if the raycast hits a ring prepare it to be dragged
    private void PerformRaycast() {
        inputPositionWorld = Camera.main.ScreenToWorldPoint(GetInputPosition());
        RaycastHit2D hitInformation = Physics2D.Raycast(inputPositionWorld, Camera.main.transform.forward);

        if (hitInformation.collider != null && hitInformation.transform.gameObject.GetComponent<RingScript>() != null) {
            pressedObject = hitInformation.transform.gameObject;
            pressedObject.GetComponent<RingScript>().PrepareForDrag();
        }
    }

}
