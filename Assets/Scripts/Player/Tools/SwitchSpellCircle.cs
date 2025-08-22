using UnityEngine;

public class SwitchSpellCircle : MonoBehaviour
{

    [SerializeField] GameObject[] circleList = new GameObject[6];

    public void Switch(int id)
    {
        ResetAll();

        circleList[id].SetActive(true);
        GameObject obj;

        obj = SpellCraftUI._instance.projectileButton.selectedSlot;
        obj.SetActive(true);
        obj.transform.position = circleList[0].transform.GetChild(0).transform.position;

        if (id > 0)
        {
            for (int i = 0; i < id; i++)
            {
                obj = SpellCraftUI._instance.behaviorsButtons[i].selectedSlot;
                obj.SetActive(true);
                obj.transform.position = circleList[id].transform.GetChild(i+1).transform.position;
            }
        }
    }

    void ResetAll()
    {
        for (int i = 0; i < SpellCraftUI._instance.behaviorsButtons.Length; i++)
        {
            SpellCraftUI._instance.behaviorsButtons[i].selectedSlot.SetActive(false);
        }
        SpellCraftUI._instance.projectileButton.selectedSlot.SetActive(false);

        for (int i = 0; i < circleList.Length; i++)
        {
            circleList[i].SetActive(false);
        }
    }

}
