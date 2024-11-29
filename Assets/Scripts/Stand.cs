using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

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
        private List<BodyPartController> bpControllers = new();
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
        public override void OnEpisodeBegin() {
            foreach (var bpController in bpControllers){
                bpController.Reset();
            }
        }
        public override void CollectObservations(VectorSensor sensor) {
            foreach (var bpController in bpControllers) {
                sensor.AddObservation(bpController.groundContact.isTouchingGround);
                sensor.AddObservation(bpController.rb.transform.localPosition);
                sensor.AddObservation(bpController.rb.transform.localRotation);
                sensor.AddObservation(bpController.rb.velocity);
                sensor.AddObservation(bpController.rb.angularVelocity);
            }
        }
        public override void OnActionReceived(ActionBuffers actions) {
            int actionIndex = 0;

            foreach (var bpController in bpControllers) {
                if (bpController.joint != null) {
                    ConfigurableJoint joint = bpController.joint;
                    float x = 0, y = 0, z = 0;

                    if (joint.angularXMotion != ConfigurableJointMotion.Locked) {
                        x = actions.ContinuousActions[actionIndex++];
                    }
                    if (joint.angularYMotion != ConfigurableJointMotion.Locked) {
                        y = actions.ContinuousActions[actionIndex++];
                    }
                    if (joint.angularZMotion != ConfigurableJointMotion.Locked) {
                        z = actions.ContinuousActions[actionIndex++];
                    }
                    float strength = actions.ContinuousActions[actionIndex++];
                    bpController.SetTargetRotation(x, y, z);
                    bpController.SetJointStrength(strength);
                }
            }
            Rewards();
        }
        private void Rewards() {
            int groundContacts = 0;

            foreach (var bpController in bpControllers) {
                if (bpController.groundContact.isTouchingGround && bpController.groundContact.penalizeAgent) {
                    groundContacts++;
                    AddReward(-1.0f);
                }

                if (bpController.name == "hip" || bpController.name == "spine" || bpController.name == "chest" || bpController.name.Contains("Thigh")) {
                    Vector3 bodyUp = bpController.rb.transform.up;
                    float alignment = Vector3.Dot(bodyUp, Vector3.up);
                    AddReward(alignment / 5);
                    if (alignment > 0.95f) {
                        AddReward(0.5f);
                    }
                }

                if (bpController.name.Contains("Hand")) {
                    string hand = bpController.name[..bpController.name.IndexOf("H")];
                    Transform ThighTransform = bpControllers.Find(bp => bp.name.Contains(hand + "Thigh")).rb.transform;
                    float distance = Vector3.Distance(ThighTransform.position, bpController.rb.transform.position);
                    if (distance > 0.6f || distance < 0.4f) {
                        AddReward(-distance / 2);
                    }
                }

                if (bpController.name.Contains("Thigh") || bpController.name.Contains("Shin") || bpController.name.Contains("Foot")) {
                    Vector3 veloctity = bpController.rb.velocity;
                    float speed = veloctity.magnitude;
                    if (speed > 0.15f) {
                        AddReward(-speed / 6);
                    }
                    if (speed > 5) {
                        AddReward(-2.0f);
                        EndEpisode();
                    }
                }
            }

            if (groundContacts >= 5) {
                AddReward(-2.0f);
                EndEpisode();
            }
        }
        public override void Heuristic(in ActionBuffers actionsOut) {
            var continuousActions = actionsOut.ContinuousActions;
            int actionIndex = 0;

            foreach (var bpController in bpControllers){
                if (bpController.joint != null) {

                    float time = Time.time;
                    var joint = bpController.joint;

                    if (joint.angularXMotion != ConfigurableJointMotion.Locked) {
                        continuousActions[actionIndex++] = Mathf.Sin(time);
                    }
                    if (joint.angularYMotion != ConfigurableJointMotion.Locked) {
                        continuousActions[actionIndex++] = Mathf.Cos(time);
                    }
                    if (joint.angularZMotion != ConfigurableJointMotion.Locked) {
                        continuousActions[actionIndex++] = 0;
                    }
                    continuousActions[actionIndex++] = 1f;
                }
            }
 
        }
    }
}
