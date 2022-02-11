using Fusion;
using UnityEngine.Events;
using UnityEngine;

public class TreasureHolder : NetworkBehaviour
{
    [SerializeField] private GameObject stolenCoinPrefab;

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

    public void MoveMoney(TreasureHolder moneyGiver, TreasureHolder moneyReceiver)
    {
        int stolenAmount = MoveMoneyAmount(moneyGiver, moneyReceiver);

        if (stolenAmount > 0 && Object.HasStateAuthority)
        {
            RPC_MoveMoneyAnimation(moneyGiver, moneyReceiver);
        }

    }

    private int MoveMoneyAmount(TreasureHolder moneyGiver, TreasureHolder moneyReceiver)
    {
        int stolenAmount = moneyGiver.GiveTreasure();
        moneyReceiver.TakeTreasure(stolenAmount);

        return stolenAmount;
    }

    [Rpc(sources: RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_MoveMoneyAnimation(TreasureHolder moneyGiver, TreasureHolder moneyReceiver)
    {
        GameObject coin = Instantiate(stolenCoinPrefab, moneyGiver.transform.position, Quaternion.identity);
        coin.GetComponent<StolenCoinBehaviour>().target = moneyReceiver.transform;
    }
}
