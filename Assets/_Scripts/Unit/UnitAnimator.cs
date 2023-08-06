using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private static readonly int IsWalkingAnimatorBool = Animator.StringToHash("IsWalking");
    private static readonly int ShootAnimatorTrigger = Animator.StringToHash("Shoot");
    private static readonly int SwordSlashAnimatorTrigger = Animator.StringToHash("SwordSlash");
    private static readonly int JumpUpAnimatorTrigger = Animator.StringToHash("JumpUp");
    private static readonly int JumpDownAnimatorTrigger = Animator.StringToHash("JumpDown");

    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += OnStartMoving;
            moveAction.OnStopMoving += OnStopMoving;
            moveAction.OnChangeFloorStarted += OnChangeFloorStarted;
        }

        if (TryGetComponent(out ShootAction shootAction)) 
            shootAction.OnShoot += OnShoot;

        if (TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += OnSwordActionCompleted;
        }
    }

    private void OnChangeFloorStarted(GridPosition originGridPos, GridPosition targetGridPos)
    {
        animator.SetTrigger(targetGridPos.floor > originGridPos.floor
            ? JumpUpAnimatorTrigger
            : JumpDownAnimatorTrigger);
    }

    private void Start()
    {
        EquipRifle();
    }

    private void OnSwordActionCompleted()
    {
        EquipRifle();
    }

    private void OnSwordActionStarted()
    {
        EquipSword();
        animator.SetTrigger(SwordSlashAnimatorTrigger);
    }

    private void OnShoot(Unit targetUnit, Unit shootingUnit)
    {
        animator.SetTrigger(ShootAnimatorTrigger);

        Transform bulletProjectileTransform =
            Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

        Vector3 targetUnitShootPosition = targetUnit.GetWorldPosition();

        const float unitShoulderHeight = 1.7f;
        targetUnitShootPosition.y += unitShoulderHeight;

        bulletProjectileTransform.GetComponent<BulletProjectile>().Setup(targetUnitShootPosition);
    }

    private void OnStopMoving()
    {
        animator.SetBool(IsWalkingAnimatorBool, false);
    }

    private void OnStartMoving()
    {
        animator.SetBool(IsWalkingAnimatorBool, true);
    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
}