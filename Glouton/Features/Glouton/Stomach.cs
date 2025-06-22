using Glouton.EventArgs;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Glouton.Features.Glouton;

internal class Stomach
{
    private const int MAX_ENTRIES = 100;    
    private readonly ConcurrentQueue<Food> _digestedFood;

    public event EventHandler<DigestionEventArgs>? FoodDigested;

    public Stomach()
    {
        _digestedFood = [];
    }

    public void AddFood(Food food)
    {
        _digestedFood.Enqueue(food);

        while (_digestedFood.Count > MAX_ENTRIES && _digestedFood.TryDequeue(out _)) { }

        GloutonAppreciation appreciation = GloutonTaste.FAVORITE_FOOD.Contains(food.Extension) ? GloutonAppreciation.Wonderful
                                         : GloutonTaste.HATED_FOOD.Contains(food.Extension) ? GloutonAppreciation.Awful
                                         : GloutonAppreciation.Ok;
        this.FoodDigested?.Invoke(this, new DigestionEventArgs(appreciation));
    }
}