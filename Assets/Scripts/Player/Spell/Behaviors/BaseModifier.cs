using UnityEngine;

[CreateAssetMenu(fileName = "BaseModifier", menuName = "Base Modifier", order = 1)]
public class BaseModifier : AddedBehavior
{
    public enum Operation { Add, Multiply, Substract, Divide };
    public Operation operation;
}
