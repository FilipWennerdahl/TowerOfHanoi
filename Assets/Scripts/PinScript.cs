using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PinScript : MonoBehaviour {

    public RuleManager.Pin pin;
    [Range(1, 5000)]
    public int ringEjectForce = 1;

    private RuleManager ruleManager;
    private GameObject unplaceableRing = null;

    void Start() {
        ruleManager = GameObject.FindGameObjectWithTag("RuleManager").GetComponent<RuleManager>();  
    }

    void Update() {
        RemoveUnplacableRing();
    }

    //Remove a potential ring that cannot be placed on the pin by shooting it in a random upwards direction 
    private void RemoveUnplacableRing() {

        if (unplaceableRing != null) {
            int ringSize = unplaceableRing.GetComponent<RingScript>().ringSize;
            unplaceableRing.GetComponentInParent<Rigidbody2D>().AddForce(RandomUpwardsDirection() * ringEjectForce);
        }

    }

    //Give the rings one second to initalize their placement on the starting pin
    private bool RingPlacementInitialized() {
        return Time.time > 1f;
    }

    private Vector2 RandomUpwardsDirection() {
        return new Vector2(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(0f, 1f));
    }

    //If the ring is placable on the pin att it to pin in the rule manager otherwhise 
    void OnTriggerEnter2D(Collider2D collision) {
        int ringSize = collision.GetComponentInParent<RingScript>().ringSize;

        if (ruleManager.IsRingPlacable(ringSize, pin)) {
            ruleManager.AddRingToPin(ringSize, pin);
        } else if(RingPlacementInitialized()){
            collision.GetComponentInParent<RingScript>().RingUnmovable();
            unplaceableRing = collision.gameObject.transform.parent.gameObject;
        }
             
    }

    //Add a nice slow slide-down effect, if game is won shoot the rings up 
    void OnTriggerStay2D(Collider2D collision) {
        collision.GetComponentInParent<RingScript>().SetMaxVelocity(15);

        if (ruleManager.GameWon()) {
            collision.gameObject.GetComponentInParent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            collision.gameObject.GetComponentInParent<Rigidbody2D>().AddForce(RandomUpwardsDirection() * ringEjectForce);
        } 

    }

    //Reset the ring to it's original max velocity, removed it from the pin in the rule manager and reset the marker for a ring that is not placable.
    void OnTriggerExit2D(Collider2D collision) {
        RingScript ring = collision.GetComponentInParent<RingScript>();
        ring.ResetMaxVelocity();
        ruleManager.RemoveRingFromPin(ring.ringSize, pin);
        unplaceableRing = null;
    }

}
