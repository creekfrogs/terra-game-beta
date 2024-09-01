using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    [HideInInspector] public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;

    [Header("Camera Settings")]
    [SerializeField] float cameraSmoothSpeed = 1;
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30;
    [SerializeField] float maximumPivot = 60;
    [SerializeField] float unlockedCameraHeight = 1.65f;
    [SerializeField] float lockedCameraHeight = 2.0f;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    [SerializeField] float cameraCollisionRadius = 0.2f;
    private float defaultCameraZPosition;
    private float targetCameraZPosition;
    private Vector3 cameraObjectPosition;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Lock On Variables")]
    [SerializeField] float lockOnRadius = 20;
    [SerializeField] float minimumViewableAngle = -50;
    [SerializeField] float maximumViewableAngle = 50;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    private Coroutine cameraLockOnHeightCoroutine;
    [SerializeField] float setCameraHeightSpeed = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        defaultCameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        if(player.playerNetworkManager.isLockedOn.Value)
        {
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;
        }
        else
        {
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = defaultCameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

    public void HandleFindLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity; // WILL BE USED TO DETERMINE THE CLOSEST TARGET
        float shortestDistanceOfRightTarget = Mathf.Infinity; // WILL BE USED TO DETERMINED THE SHORTEST DISTANCE ON ONE AXIS TO THE RIGHT OF CURRENT TARGET
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; // WILL BE USED TO DETERMINED THE SHORTEST DISTANCE ON ONE AXIS TO THE LEFT OF CURRENT TARGET
        
        if(player != null)
        {
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

            // FIND POTENTIAL TARGETS
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                if (lockOnTarget != null)
                {
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                    float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                    if (lockOnTarget.isDead.Value) // IF TARGET IS DEAD, IGNORE
                        continue;

                    if (lockOnTarget.transform.root == player.transform.root) // IF TARGET IS SELF, IGNORE
                        continue;

                    if (viewableAngle >= minimumViewableAngle && viewableAngle <= maximumViewableAngle) // IF TARGET IS OUTSITE OF FOV OR BLOCKED BY ENVIRO, IGNORE
                    {
                        RaycastHit hit;

                        //ADD ENVIRO MASK
                        if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, lockOnTarget.characterCombatManager.lockOnTransform.position, out hit, WorldUtilityManager.instance.GetEnviroLayers()))
                        {
                            //WE CANNOT SEE THE TARGET
                            continue;
                        }
                        else
                        {
                            availableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }

            // SORT THROUGH POTENTIAL TARGETS BY PROXIMITY
            for (int k = 0; k < availableTargets.Count; k++)
            {
                if (availableTargets[k] != null)
                {
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);
                    Vector3 lockTargetsDirection = availableTargets[k].transform.position - player.transform.position;

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[k];
                    }

                    if (player.playerNetworkManager.isLockedOn.Value)
                    {
                        Vector3 relativeEnemyPos = player.transform.InverseTransformPoint(availableTargets[k].transform.position);
                        var distanceFromLeftTarget = relativeEnemyPos.x;
                        var distanceFromRightTarget = relativeEnemyPos.x;

                        if (availableTargets[k] == player.playerCombatManager.currentTarget)
                            continue;

                        if(relativeEnemyPos.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockOnTarget = availableTargets[k];
                        }
                        else if (relativeEnemyPos.x >= 0.00 & distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockOnTarget = availableTargets[k];
                        }
                    }
                }
                else
                {
                    player.playerNetworkManager.isLockedOn.Value = false;
                    ClearLockOnTargets();
                }
            }
        }
    }

    public void SetLockCameraHeight()
    {
        if(cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }
        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
        SetLockCameraHeight();
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
        {
            yield return null;
        }

        ClearLockOnTargets();
        HandleFindLockOnTargets();

        if(nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.playerNetworkManager.isLockedOn.Value = true;
        }

        yield return null;
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                if(player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    cameraPivotTransform.transform.localRotation = Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                }
            }

            yield return null;
        }
    }
}