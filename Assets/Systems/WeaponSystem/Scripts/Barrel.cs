#region

using System.Collections;
using UnityEngine;

#endregion

public abstract class Barrel : MonoBehaviour
{
    [Header("Debug")] [SerializeField] private bool debugShot;

    [Header("Debug")] [SerializeField] private bool debugContinuousShot;

    protected Weapon weapon;

    // void OnValidate()
    // {
    //     if (debugShot)
    //     {
    //         debugShot = false;
    //         Shot();
    //     }
    //     
    //     if (debugContinuousShot)
    //     {
    //         StartShooting();
    //     } else 
    //     {
    //         StopShooting();
    //     }
    // }

    public void AssignWeapon(Weapon weaponParent)
    {
        weapon = weaponParent;
    }

    public virtual void Shot()
    {
        weapon.PlayShot();
        if (Weapon.ShotMode.Continuous == weapon.shotMode) weapon.playWeaponShoot = false;

        if (weapon.BulletPrefab != null)
        {
            GameObject bullet = Instantiate(weapon.BulletPrefab, transform.position, transform.rotation);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            Vector3 offset = transform.right * 0.1f + transform.forward * 0.1f;
            bullet.transform.position += offset;

            Vector3 forceDirection = transform.up + transform.right * 0.3f;
            bulletRigidbody.AddForce(forceDirection * 100);
            StartCoroutine(BulletLife(bullet));
        }
        else
        {
            Debug.LogWarning("No bullet prefab found for " + gameObject.name);
        }

        if ((weapon.Animator != null && weapon.shotMode == Weapon.ShotMode.Continuous)
            || (weapon.Animator != null && weapon.shotMode == Weapon.ShotMode.ShotByShot &&
                !weapon.playWeaponShotByShot))
        {
            Debug.Log("Shot animation");
            weapon.playWeaponShotByShot = true;
            weapon.Animator.SetTrigger("Shoot");
        }

        IEnumerator BulletLife(GameObject bullet)
        {
            yield return new WaitForSeconds(5);
            Destroy(bullet);
        }
    }

    protected void CantShoot()
    {
        Debug.Log("Cant shoot");
        if (weapon.IsReloading())
            Debug.Log("Cant shoot, reloading");
        else if (weapon.IsCurrentAmmoEmpty())
            //TODO: Make this a setting
            //weapon.Reload();
            weapon.PlayEmptyClipSound();
    }

    public virtual void StartShooting()
    {
    }

    public virtual void StopShooting()
    {
    }
}