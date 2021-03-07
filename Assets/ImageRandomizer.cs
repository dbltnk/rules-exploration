using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRandomizer : MonoBehaviour
{
    private ProceduralToolkit.Samples.CellularAutomatonConfigurator conf;

    // Start is called before the first frame update
    void Start () {
        conf = FindObjectOfType<ProceduralToolkit.Samples.CellularAutomatonConfigurator>();

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
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void Update () {
        float wiggleChance = 0.33f * Time.deltaTime;
        if (Random.Range(0f, 1f) <= wiggleChance && conf.IsPlaying) {

            float tZ = transform.localScale.z;
            if (Random.Range(0f, 1f) < 0.5f) {
                tZ *= 1.025f;
            } else {
                tZ *= 0.975f;
            }
            tZ = Mathf.Clamp(tZ, 0.9f, 1.1f);

            float tX = Mathf.Abs(transform.localScale.x);
            float s = Mathf.Sign(transform.localScale.x);
            if (Random.Range(0f, 1f) < 0.5f) {
                tX *= 1.025f;
            } else {
                tX *= 0.975f;
            }
            tX = Mathf.Clamp(tX, 0.9f, 1.1f);

            transform.localScale = new Vector3(tX * s, transform.localScale.y, tZ);
        }

        //transform.Rotate(Vector3.back, 1f);
    }
}
