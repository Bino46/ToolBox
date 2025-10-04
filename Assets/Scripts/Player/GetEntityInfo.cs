using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetEntityInfo : MonoBehaviour
{
    [SerializeField] GameObject ui;
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI entityName;
    [SerializeField] TextMeshProUGUI currMaxHp;
    private LayerMask checkMask;

    void Start()
    {
        checkMask = LayerMask.GetMask("Entity");
    }

    void Update()
    {
        CheckEntity();
    }

    void CheckEntity()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity) && hit.transform.tag == "Entity")
        {
            RPG_Entity checkedEntity = hit.transform.GetComponent<RPG_Entity>();

            if (checkedEntity == null)
                checkedEntity = hit.transform.GetComponentInParent<RPG_Entity>();

            ui.SetActive(true);

            hpSlider.value = checkedEntity.currHp;
            hpSlider.maxValue = checkedEntity.hp;

            currMaxHp.text = checkedEntity.currHp.ToString() + " / " + checkedEntity.hp.ToString();

            entityName.text = checkedEntity.ent_name;
        }
        else
            ui.SetActive(false);
    }
}
