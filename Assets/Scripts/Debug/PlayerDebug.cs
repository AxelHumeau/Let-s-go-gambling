using UnityEngine;
using System.Collections;

public class PlayerDebug : MonoBehaviour
{
	[SerializeField] private Player player;
	public bool hasDouble = false;
	public bool hasHalf = false;
    public bool hasLuckIncrease = false;
    public bool hasLuckDecrease = false;
    public bool hasNoBluff = false;
    public bool hasMineResistant = false;
    public int givenMoney = 0;
    public int givenDoubleDice = 0;
    public int givenHalfDice = 0;

    // Use this for initialization
    void Start()
	{
        if (hasDouble)
        {
            player.AddEffect(Effect.Double, 1);
        }
        if (hasHalf)
        {
            player.AddEffect(Effect.Half, 1);
        }
        if (hasLuckIncrease)
        {
            player.AddEffect(Effect.LuckIncrease, 1);
        }
        if (hasLuckDecrease)
        {
            player.AddEffect(Effect.LuckDecrease, 1);
        }
        if (hasNoBluff)
        {
            player.AddEffect(Effect.NoBluff, 1);
        }
        if (hasMineResistant)
        {
            player.AddEffect(Effect.MineResistant, 1);
        }
        if (givenMoney != 0)
        {
            player.AddMoney(givenMoney);
        }
        for (int i = 0; i < givenDoubleDice; i++)
        {
            player.AddItem(new DoubleDice());
        }
        for (int i = 0; i < givenHalfDice; i++)
        {
            player.AddItem(new LesserDice());
        }
    }

	// Update is called once per frame
	void Update()
	{

	}
}