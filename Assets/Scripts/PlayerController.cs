using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 5f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;

    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;

    private float thrusterFuelAmount = 1f;

    public AudioSource thrusterSound;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        thrusterSound = GetComponent<AudioSource>();
        thrusterSound.Pause();
        SetJointSettings(jointSpring);
    }

    void Update()
    {
        if (PauseMenu.IsOn)
            return;

        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        //calc move speed as a 3d vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov; //(1,0,0)
        Vector3 _movVertical = transform.forward * _zMov; //(0,0,1)

        //final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

        //animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        //apply movement
        motor.Move(_velocity);

        //calc rotation as a 3d vector: just for turning around!
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        //apply rotation
        motor.Rotate(_rotation);

        //calc camera rotation as a 3d vector: just for turning around!
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSensitivity;

        //apply camera rotation
        motor.RotateCamera(_cameraRotationX);

        //calc the thruster force
        Vector3 _thrusterForce = Vector3.zero;

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
                thrusterSound.UnPause();
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }
        } else
        {
            thrusterSound.Pause();
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }
    
    //joint settings setter
    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive {positionSpring = jointSpring, maximumForce = jointMaxForce};
    }
}
