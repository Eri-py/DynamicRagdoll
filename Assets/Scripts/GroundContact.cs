using UnityEngine;
using Unity.MLAgents;
using System;

namespace GameAI {
    public class GroundContact : MonoBehaviour {
        // Attributes
        public bool isTouchingGround;
        public bool penalizeAgent;
        public bool endEpisode;
        public float penalty = -1;
        const String groundTag = "Ground";
        // Methods
        void OnCollisionEnter(Collision collision){
            if(collision.transform.CompareTag(groundTag)){
                isTouchingGround = true;
            }
        }
        void OnCollisionExit(Collision other) {
            if(other.transform.CompareTag(groundTag)) {
                isTouchingGround = false;
            }
        }
        public void Reset(){
            isTouchingGround = false;
        }
    }
}
