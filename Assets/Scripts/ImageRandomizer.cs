using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRandomizer : MonoBehaviour
{
    public Sprite cellMoore;
    public Sprite cellVonNeumann;

    PlayerControlScript playerControlScript;
    CellManagerScript cellManagerScript;
    CellObjectScript cellObjectScript;
    SpriteRenderer rend;

    void Start () {
        playerControlScript = FindObjectOfType<PlayerControlScript>();
        cellManagerScript = FindObjectOfType<CellManagerScript>();
        cellObjectScript = GetComponentInParent<CellObjectScript>();
        rend = GetComponent<SpriteRenderer>();

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
        Species thisSpecies = cellManagerScript.GetSpecies(cellObjectScript.GetCoords());
        bool isVonNeumann4 = false;
        List<SPECIES_GROUP> groups = new List<SPECIES_GROUP>();
        if (thisSpecies != null) {
            groups = thisSpecies.speciesGroups;
        }
        if (groups.Contains(SPECIES_GROUP.VON_NEUMANN_4)) isVonNeumann4 = true;

        if (isVonNeumann4) {
            rend.sprite = cellVonNeumann;
        }
        else {
            rend.sprite = cellMoore;
        }

        bool isAlive = cellManagerScript.GetCellStateAtCoords(cellObjectScript.GetCoords()).alive;
        float wiggleChance = 1f / cellManagerScript.updateRate * Time.deltaTime;
        if (Random.Range(0f, 1f) <= wiggleChance && playerControlScript.simulationRunning && isAlive) {

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
