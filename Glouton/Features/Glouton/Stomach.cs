using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Features.Glouton;

internal class Stomach
{
    private readonly ConcurrentQueue<Food> _digestedFood;

    public event EventHandler<DigestionEventArgs>? FoodDigested;

    public Stomach()
    {
        _digestedFood = [];
    }

    public void AddFood(Food food)
    {
        _digestedFood.Enqueue(food);
        GloutonAppreciation appreciation = GloutonTaste.FAVORITE_FOOD.Contains(food.Extension) ? GloutonAppreciation.Wonderful
                                         : GloutonTaste.HATED_FOOD.Contains(food.Extension) ? GloutonAppreciation.Awful
                                         : GloutonAppreciation.Ok;
        this.FoodDigested?.Invoke(this, new DigestionEventArgs(appreciation));
    }
}