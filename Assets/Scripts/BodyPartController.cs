using UnityEngine;

namespace GameAI {
    [System.Serializable]
    #nullable enable
    public class BodyPartController {
        // Attributes
        public Rigidbody rb;
        public ConfigurableJoint? joint; 
        public string name;
        public Vector3 startingPos;
        public Quaternion startingRot;
        public GroundContact? groundContact;
        // Constructor
        public BodyPartController(Transform bt, string fieldName) {
            name = fieldName; // ensure consistency in different agent rigs
            rb = bt.GetComponent<Rigidbody>();
            joint = bt.TryGetComponent<ConfigurableJoint>(out var hasJoint) ? hasJoint : null;
            startingPos = bt.localPosition;
            startingRot = bt.localRotation;
            groundContact = bt.GetComponent<GroundContact>();
        }
        // Methods
        // Set the target rotation for the body part
        public void SetTargetRotation(float x, float y, float z) {
            if (joint == null) {
                Debug.LogWarning($"Body part {name} has no ConfigurableJoint.");
                return;
            }
            // Normalize inputs from [-1, 1] to [0, 1]
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            z = (z + 1f) * 0.5f;
            // Calculate rotation within joint limits
            float xRot = Mathf.Lerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, x);
            float yRot = Mathf.Lerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, y);
            float zRot = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);
            // Set joint's target rotation based on calculated Euler angles
            joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
        }
        public void SetJointStrength(float strength) {
            if (joint == null) {
                Debug.LogWarning($"Body part {name} has no ConfigurableJoint.");
                return;
            } 
            // Normalize strength from [-1, 1] to [0, 1]
            strength = (strength + 1f) * 0.5f;
            // Configure the joint slerp drive
            JointDrive jd = new JointDrive {
                positionSpring = 10000f,
                positionDamper = 100f,  
                maximumForce = strength * 25000
            };
            // Apply the configured JointDrive to the joint's slerpDrive
            joint.slerpDrive = jd;
        }
        // Reset body part position after each episode end
        public void Reset() {
            startingPos += new Vector3(0f, 1f, 0f);
            rb.transform.SetPositionAndRotation(startingPos, startingRot);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            if(groundContact != null) groundContact.Reset();
        }
    }
}
