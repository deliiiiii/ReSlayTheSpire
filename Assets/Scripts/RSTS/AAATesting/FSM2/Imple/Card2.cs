using System;
using System.Collections.Generic;

public class CardBattle
{
    public CardRainy Rainy;
    public CardTurn? Turn;
    
    public TStateData GetStateData<TState, TStateData>(out TStateData stateData)
        where TStateData : IStateData<TState>
    {
        object? target;
        if (typeof(TState) == typeof(BothTurn))
            target = Turn;
        else if (typeof(TState) == typeof(WeatherRainy))
            target = Rainy;
        else
            throw new Exception("No such state data");

        return stateData = (TStateData)(IStateData<TState>)target!;
    }
}

public interface IStateData<TState>;
public class CardTurn : IStateData<BothTurn>;
public class CardRainy : IStateData<WeatherRainy>;

public class Battle
{
    Weather weather = new WeatherRainy();
    BothTurn? bothTurn;

    public List<CardBattle> CardList = [];
}

public class BothTurn;

public abstract class Weather;
public class WeatherSunny : Weather;
public class WeatherRainy : Weather;