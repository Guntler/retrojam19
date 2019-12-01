﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Update100Evt : GameEvent
{

}

public class Update200Evt : GameEvent
{

}

public class Update500Evt : GameEvent
{

}

public class Update1000Evt : GameEvent
{

}

public class PickedUpDebrisEvt : GameEvent
{
    public BoardItemBehavior PickedItem;

    public PickedUpDebrisEvt(BoardItemBehavior item) : base()
    {
        PickedItem = item;
    }
}

public class RocketCollidedEvt : GameEvent
{

}

public class TruckCollidedEvt : GameEvent
{

}

public class DeliveredDebrisEvt : GameEvent
{

}

public class DeliverCarEvt : GameEvent
{
    public CarBehavior CarToDeliver;

    public DeliverCarEvt(CarBehavior car)
    {
        CarToDeliver = car;
    }
}

public class StopTickEvt : GameEvent
{

}

public class StartTickEvt : GameEvent
{

}

public class CreateScoreMultiplierTextEvt : GameEvent
{
    public int Multiplier;

    public CreateScoreMultiplierTextEvt(int mult) : base()
    {
        Multiplier = mult;
    }
}

public class UpdateScoreEvt : GameEvent
{
    public int ScoreAmt = 0;

    public UpdateScoreEvt(int score) : base()
    {
        ScoreAmt = score;
    }
}

public class AddScoreEvt : GameEvent
{
    public int ScoreAmt = 0;

    public AddScoreEvt(int score) : base()
    {
        ScoreAmt = score;
    }
}
public class LaunchedRocketEvt : GameEvent
{

}
public class LaunchCountdownEvt : GameEvent
{

}

public class GameEndEvt : GameEvent
{

}

public class GameStartEvt : GameEvent
{

}

public class SpawnRocketEvt : GameEvent
{

}