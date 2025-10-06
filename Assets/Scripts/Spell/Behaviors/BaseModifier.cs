using UnityEngine;

//[CreateAssetMenu(fileName = "BaseModifier", menuName = "Base Modifier", order = 1)]
public class BaseModifier : AddedBehavior
{
    public enum Operation { Add, Multiply };
    public Operation operation;
}
