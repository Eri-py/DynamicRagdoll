using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace GameAI {
    public class Stand : Agent {
        // Attributes
        [Header("Lower Body Parts")]
        public Transform hip;
        public Transform leftThigh;
        public Transform leftShin;
        public Transform leftFoot;
        public Transform rightThigh;
        public Transform rightShin;
        public Transform rightFoot; 
        [Header("Upper Body Parts")]
        public Transform spine;
        public Transform chest;
        public Transform upperLeftArm;
        public Transform lowerLeftArm;
        public Transform leftHand;
        public Transform upperRightArm;
        public Transform lowerRightArm;
        public Transform rightHand;
        public Transform head;
        private List<BodyPartController> bpControllers = new List<BodyPartController>();
        // Methods
        public override void Initialize() {
        bpControllers.Add(new BodyPartController(hip, "hip"));
        bpControllers.Add(new BodyPartController(leftThigh, "leftThigh"));
        bpControllers.Add(new BodyPartController(leftShin, "leftShin"));
        bpControllers.Add(new BodyPartController(leftFoot, "leftFoot"));
        bpControllers.Add(new BodyPartController(rightThigh, "rightThigh"));
        bpControllers.Add(new BodyPartController(rightShin, "rightShin"));
        bpControllers.Add(new BodyPartController(rightFoot, "rightFoot"));
        bpControllers.Add(new BodyPartController(spine, "spine"));
        bpControllers.Add(new BodyPartController(chest, "chest"));
        bpControllers.Add(new BodyPartController(upperLeftArm, "upperLeftArm"));
        bpControllers.Add(new BodyPartController(lowerLeftArm, "lowerLeftArm"));
        bpControllers.Add(new BodyPartController(leftHand, "leftHand"));
        bpControllers.Add(new BodyPartController(upperRightArm, "upperRightArm"));
        bpControllers.Add(new BodyPartController(lowerRightArm, "lowerRightArm"));
        bpControllers.Add(new BodyPartController(rightHand, "rightHand")); 
        bpControllers.Add(new BodyPartController(head, "head"));
        }
        // Reset each body part at start of each episode
        public override void OnEpisodeBegin() {
            foreach (var bpController in bpControllers){
                bpController.Reset();
            }
        }
        public override void CollectObservations(VectorSensor sensor) {
            foreach (var bpController in bpControllers) {
                // If body part is touching the ground
                sensor.AddObservation(bpController.groundContact.isTouchingGround);
                // Position of body part
                sensor.AddObservation(bpController.rb.transform.localPosition);
                // Rotation of body part
                sensor.AddObservation(bpController.rb.transform.localRotation);
                // linear velocity of body part
                sensor.AddObservation(bpController.rb.velocity);
                // Angular velocity of body part
                sensor.AddObservation(bpController.rb.angularVelocity);
                // target rotation of joint
                if (bpController.joint) {
                    sensor.AddObservation(bpController.joint.targetRotation);
                }
                if (bpController.joint != null) {
                    sensor.AddObservation(Mathf.InverseLerp(bpController.joint.lowAngularXLimit.limit, 
                                                            bpController.joint.highAngularXLimit.limit, 
                                                            bpController.joint.targetRotation.eulerAngles.x));
                }
            }
        }
        public override void OnActionReceived(ActionBuffers actions) {
            int actionIndex = 0;
            foreach (var bpController in bpControllers){
                if (bpController.joint != null) {
                    float x = actions.ContinuousActions[actionIndex++];
                    float y = actions.ContinuousActions[actionIndex++];
                    float z = actions.ContinuousActions[actionIndex++];
                    float strength = actions.ContinuousActions[actionIndex++];
                    
                    bpController.SetTargetRotation(x, y, z);
                    bpController.SetJointStrength(strength);
                }
            }

        }
        public override void Heuristic(in ActionBuffers actionsOut) {
            var continuousActions = actionsOut.ContinuousActions;
            int actionIndex = 0;
            foreach (var bpController in bpControllers){
                if (bpController.joint != null) {
                    float time = Time.time;
                    continuousActions[actionIndex++] = Mathf.Sin(time);
                    continuousActions[actionIndex++] = Mathf.Cos(time);
                    continuousActions[actionIndex++] = 0.0f;
                    continuousActions[actionIndex++] = 1f;
                }
            }
        }
    }
}
