using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRandomizer : MonoBehaviour
{
    PlayerControlScript playerControlScript;

    void Start () {
        playerControlScript = FindObjectOfType<PlayerControlScript>();

        float r = Random.Range(0f, 5f);

        if (Random.Range(0f, 1f) < 0.25f) {
            transform.rotation = Quaternion.AngleAxis(0f + r, Vector3.forward);
        } else if (Random.Range(0f, 1f) < 0.5f) {
            transform.rotation = Quaternion.AngleAxis(90f + r, Vector3.forward);
        } else if (Random.Range(0f, 1f) < 0.75f) {
            transform.rotation = Quaternion.AngleAxis(180f + r, Vector3.forward);
        } else if (Random.Range(0f, 1f) < 1f) {
            transform.rotation = Quaternion.AngleAxis(270f + r, Vector3.forward);
        }

        if (Random.Range(0f, 1f) < 0.5f) {
            transform.localScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void Update () {
        float wiggleChance = 1f * Time.deltaTime;
        if (Random.Range(0f, 1f) <= wiggleChance && playerControlScript.simulationRunning) {

            float tZ = transform.localScale.z;
            if (Random.Range(0f, 1f) < 0.5f) {
                tZ *= 1.025f;
            } else {
                tZ *= 0.975f;
            }
            tZ = Mathf.Clamp(tZ, 2.7f, 3.3f);

            float tX = Mathf.Abs(transform.localScale.x);
            float s = Mathf.Sign(transform.localScale.x);
            if (Random.Range(0f, 1f) < 0.5f) {
                tX *= 1.025f;
            } else {
                tX *= 0.975f;
            }
            tX = Mathf.Clamp(tX, 2.7f, 3.3f);

            transform.localScale = new Vector3(tX * s, transform.localScale.y, tZ);
        }
    }
}
