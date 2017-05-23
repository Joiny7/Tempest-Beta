using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class VibrationController : MonoBehaviour {

    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    public float BulletVibStrength;
    public float BulletVibTime;
    public float MissileVibStrength;
    public float MissileVibTime;
    public float CollisionVibTime;
    public float CollisionVibStrength;
    public float BoostVibStrength;
    public float DamageVibStrengthLeft;
    public float DamageVibStrengthRight;
    public float DamageVibTime;
    public float ScatterVibStrength;
    public float ScatterVibTime;


    public IEnumerator ShotMissileVib()
    {
        GamePad.SetVibration(playerIndex, MissileVibStrength, MissileVibStrength);
        yield return new WaitForSeconds(MissileVibTime);
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public IEnumerator ShotScatterVib()
    {
        GamePad.SetVibration(playerIndex, ScatterVibStrength, ScatterVibStrength);
        yield return new WaitForSeconds(ScatterVibTime);
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public IEnumerator ShotBulletVib()
    {
        GamePad.SetVibration(playerIndex, BulletVibStrength, BulletVibStrength);
        yield return new WaitForSeconds(BulletVibTime);
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public void CollisionVibStart()
    {
        GamePad.SetVibration(playerIndex, CollisionVibStrength, CollisionVibStrength);
    }

    public void CollisionVibStop()
    {
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public IEnumerator BoostVibStart()
    {
        GamePad.SetVibration(playerIndex, BoostVibStrength, BoostVibStrength);
        yield return new WaitForSeconds(0.5f);
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public IEnumerator MineDropVib()
    {
        GamePad.SetVibration(playerIndex, 1f, 1f);
        yield return new WaitForSeconds(0.25f);
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public IEnumerator DeathVib()
    {
        GamePad.SetVibration(playerIndex, 4f, 4f);
        yield return new WaitForSeconds(0.2f);
        GamePad.SetVibration(playerIndex, 2f, 1f);
        yield return new WaitForSeconds(0.2f);
        GamePad.SetVibration(playerIndex, 1f, 2f);
        yield return new WaitForSeconds(0.2f);
        GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public IEnumerator DamageVib()
    {
        GamePad.SetVibration(playerIndex, DamageVibStrengthLeft, DamageVibStrengthRight);
        yield return new WaitForSeconds(DamageVibTime);
        GamePad.SetVibration(playerIndex, 0f, 0f);
    } 
}
