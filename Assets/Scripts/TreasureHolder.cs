using Fusion;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TreasureHolder : NetworkBehaviour
{
    [Networked] public int treasure { get; set; }
    public int startingTreasure;

    public UnityEvent OnTreasureTaken;
    public UnityEvent OnTreasureGiven;

    void Start()
    {
        treasure = startingTreasure;    
    }

    public int GiveTreasure()
    {
        int stolenTreasure = StolenTreasureAmount();
        OnTreasureGiven.Invoke();
        return stolenTreasure;
    }

    public void TakeTreasure(int amount)
    {
        treasure += amount;
        OnTreasureTaken.Invoke();
    }

    public bool TreasureIsEmpty()
    {
        return treasure <= 0;
    }

    public bool TreasureIsAtStartAmount()
    {
        return treasure >= startingTreasure;
    }

    private int StolenTreasureAmount()
    {
        if (treasure > 0)
        {
            treasure--;
            return 1;
        }
        return 0;
    }
}
