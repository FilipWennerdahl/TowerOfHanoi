using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBoundaries : MonoBehaviour {

    public float boundryBoxWidth;

    //Dynamically sets boundry collision boxes at the edges of the screen depending on the screen size
    void Start () {
        SetLeftBoundry();
        SetRightBoundry();
        SetTopBoundry();
    }

    private void SetLeftBoundry() {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(boundryBoxWidth, PixelToUnit(Screen.height) * 1.2f);
        collider.offset = new Vector2(-((PixelToUnit(Screen.width) / 2) + (boundryBoxWidth / 2)), 0);
    }

    private void SetRightBoundry() {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(boundryBoxWidth, PixelToUnit(Screen.height) * 1.2f);
        collider.offset = new Vector2(((PixelToUnit(Screen.width) / 2) + (boundryBoxWidth / 2)), 0);
    }

    private void SetTopBoundry() {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(PixelToUnit(Screen.width) * 1.2f, boundryBoxWidth);
        collider.offset = new Vector2(0, ((PixelToUnit(Screen.height) / 2) + (boundryBoxWidth / 2)));
    }

    //Converts pixels to unity units when using an orthographic camera
    private float PixelToUnit(float pixels) {
        return ((2 * Camera.main.orthographicSize) / Camera.main.pixelHeight) * pixels;
    }

}
