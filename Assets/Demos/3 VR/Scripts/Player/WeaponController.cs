using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace LlockhamIndustries.VR
{
    public class WeaponController : VRController
    {
        //Weapon
        public Misc.Weapon weapon;
        private WeaponMode mode = WeaponMode.Held;

        //Update
        public void Update()
        {
            if (mode != WeaponMode.Prereleased && Trigger)
            {
                mode = WeaponMode.Prereleased;
            }
            else if (mode == WeaponMode.Prereleased && !Trigger)
            {
                if (weapon.Velocity > 2f) mode = WeaponMode.Released;
                else mode = WeaponMode.Held;
            }

            //Set state based on weapon mode
            if (weapon != null)
            {
                if (mode == WeaponMode.Prereleased) weapon.WeaponState = weapon.PreReleased;
                else if (mode == WeaponMode.Released) weapon.WeaponState = weapon.Released;
                else if (mode == WeaponMode.Held && Grip) weapon.WeaponState = weapon.Extended;
                else weapon.WeaponState = weapon.Standard;
            }

            //Scene restore
            if (TrackPad) Misc.DestructableManager.Restore();
        }
    }

    public enum Hand { Left, Right };
    public enum WeaponMode { Held, Prereleased, Released };
}