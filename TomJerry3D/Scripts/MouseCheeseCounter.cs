using UnityEngine;
using UnityEngine.Events;

public class MouseCheeseCounter : MonoBehaviour
{
    [SerializeField] private int totalCheese;
    [SerializeField] private UnityEvent<int> onTotalChanged;

    public int TotalCheese => totalCheese;

    public void AddCheese(int amount)
    {
        totalCheese = Mathf.Max(0, totalCheese + amount);
        onTotalChanged?.Invoke(totalCheese);
    }
}
