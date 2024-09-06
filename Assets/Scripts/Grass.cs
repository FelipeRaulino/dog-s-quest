using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    public ParticleSystem fxGrassBurst;
    private bool isCut;

    void GetHit(int amount){
        if (isCut == false) {
            isCut = true;
            transform.localScale = new Vector3(1f, 1f, 1f);
            fxGrassBurst.Emit(30);
        }
    }
}
