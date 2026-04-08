using UnityEngine;

public class WeaponInitializer : MonoBehaviour
{
    public void InitializeWeapon()
    {
        PlayerController.Instance?.AttackController.SetWeapon(WeaponType.Spear);
    }
}
