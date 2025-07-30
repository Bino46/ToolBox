using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    Dictionary<string, TextMeshProUGUI> allUITexts = new Dictionary<string, TextMeshProUGUI>();
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI mainText;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        allUITexts.Add("MainAttack", mainText);
    }

    public void ChangeTextOnUi(string uiElement, string newText)
    {
        allUITexts[uiElement].text = newText;
    }
}
