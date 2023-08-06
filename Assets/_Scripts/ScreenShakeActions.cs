using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        ShootAction.OnAnyShoot += OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += OnAnyGrenadeExploded;

        SwordAction.OnAnySwordHit += OnAnySwordHit;
    }

    private void OnAnySwordHit()
    {
        ScreenShake.Instance.Shake();
    }

    private void OnAnyGrenadeExploded()
    {
        ScreenShake.Instance.Shake(2f);
    }

    private void OnAnyShoot(Unit target, Unit origin)
    {
        ScreenShake.Instance.Shake(0.5f);
    }
}