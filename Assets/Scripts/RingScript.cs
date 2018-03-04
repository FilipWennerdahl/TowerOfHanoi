using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingScript : MonoBehaviour {

    //Based on a script found in this Unity thread:
    //https://answers.unity.com/questions/1156868/setting-velocity-of-rigidbody-to-mouse-speed.html

    [Range(1, 4)]
    public int ringSize;
    [Range(0, 2000)]
    public float maxVelocity = 1000f;
    [Range(1, 2000)]
    public float nudgeStrength = 300;

    private RuleManager ruleManager;
    private InputManager inputManager;
    private Rigidbody2D rigidbody;
    private float originalGravityScale;
    private bool currentlyDragging = false;
    private bool movable = false;
    private float defaultMaxVelocity;

    void Start() {
        ruleManager = GameObject.FindGameObjectWithTag("RuleManager").GetComponent<RuleManager>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        originalGravityScale = rigidbody.gravityScale;
        defaultMaxVelocity = maxVelocity;
    }

    void Update() {
        FreezeYPosition();
        LimitVelocity();
        DragRing();
    }

    //Mark the ring unmovable and re-apply gravity to the ring
    public void RingUnmovable() {
        movable = false;
        rigidbody.gravityScale = originalGravityScale;
        currentlyDragging = false;
    }

    public void SetMaxVelocity(float velocity) {
        maxVelocity = velocity;
    }

    public void ResetMaxVelocity() {
        maxVelocity = defaultMaxVelocity;
    }

    //Freeze the Y position of the ring when it is placed on a pin, this is to avoid the player being able to nudge other rings off the pin with the held ring, do not freeze if the game is won
    private void FreezeYPosition() {
        if(rigidbody.constraints != RigidbodyConstraints2D.FreezePositionY && !ruleManager.AllRingsPlaced(ringSize) && rigidbody.velocity == Vector2.zero && !ruleManager.GameWon()) {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
        } 
    }

    //Limits the velocity of the ring to avoid physics bugs such as dragging the ring fast enough to pull it through other objects
    //NOTE: It is not adviced by the unity team to effect the velocity directly 
    private void LimitVelocity() {
        float xVelocity = Mathf.Clamp(rigidbody.velocity.x, -maxVelocity, maxVelocity);
        float yVelocity = Mathf.Clamp(rigidbody.velocity.y, -maxVelocity, maxVelocity);
        rigidbody.velocity = new Vector2(xVelocity, yVelocity);
    }

    //If the ring is movable, release the constraints set if the ring was placed on a pin,
    //stop the rotation in case the ring is rotation, turn of gravity to avoid the ring from constantly trying to go downwards.
    //Also add a little nudge to the ring making the interaction feel more physical and responsive
    public void PrepareForDrag() {
        movable = ruleManager.IsRingMovable(ringSize);

        if (!currentlyDragging && movable) {
            rigidbody.constraints = RigidbodyConstraints2D.None;
            rigidbody.freezeRotation = true;
            rigidbody.freezeRotation = false;
            rigidbody.gravityScale = 0f;
            rigidbody.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * nudgeStrength);
            currentlyDragging = true;
        }

    }

    //Applies the mouse delta as velocity to the ring, also adds force towards the mouse cursor to compensate for offsets when the ring gets stuck behind other objects
    private void DragRing() {

        if (currentlyDragging && movable) {        
            rigidbody.velocity = inputManager.GetInputDelta();
            rigidbody.AddForce(inputManager.DirectionTowardsInput(transform.position) * 110);
        }

    }

}
