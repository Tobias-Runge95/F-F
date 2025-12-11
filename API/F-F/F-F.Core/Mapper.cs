using F_F.Core.Responses;
using F_F.Core.Responses.FoodDiary;
using F_F.Core.Responses.FoodItem;
using F_F.Core.Responses.User;
using F_F.Core.Responses.UserData;
using F_F.Database.Models;

namespace F_F.Core;

public static class Mapper
{
    public static UserDTO ToDTO(this User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName
        };
    }

    public static UserDataDTO ToDTO(this UserData userData)
    {
        return new UserDataDTO
        {
            Age = userData.Age,
            BodyType = userData.BodyType,
            NutritionGoal = userData.NutritionGoal,
            Height = userData.Height,
            Weight = userData.Weight,
            TargetWeight = userData.TargetWeight,
            UserId = userData.UserId,
            LatestReport = userData.UserReports[^1]
        };
    }

    public static SmallFoodItemDTO ToSmallDTO(this FoodItem foodItem)
    {
        return new SmallFoodItemDTO
        {
            Id = foodItem.Id,
            Name = foodItem.ProductName,
        };
    }

    public static UserDataReport ToUserDataReport(List<UserReport> userReports)
    {
        return new UserDataReport
        {
            UserReports = userReports
        };
    }

    public static FoodDiaryDTO ToDTO(this FoodDiary foodDiary)
    {
        return new FoodDiaryDTO
        {
            Id = foodDiary.Id,
            DateTime = foodDiary.Date,
            Nutrition = foodDiary.Nutrition,
            Breakfast = foodDiary.Breakfast,
            Lunch = foodDiary.Lunch,
            Dinner = foodDiary.Dinner,
            Snacks = foodDiary.Snacks,
            Water = foodDiary.Water
        };
    }

    public static OpenFoodFactsDTO ToDTO(this FoodItem foodItem)
    {
        return new OpenFoodFactsDTO
        {
            Brand = foodItem.Brand,
            Code = foodItem.Code,
            IngredientsText = foodItem.IngredientsText,
            NutritionDataPer = foodItem.NutritionDataPer,
            NutrotionGrades = foodItem.NutrotionGrades,
            ProductName = foodItem.ProductName,
            Quantity = foodItem.Quantity,
            Nutriments = foodItem.Nutriments,
            Images = foodItem.Images,
            Ingredients = foodItem.Ingredients,
            Id = foodItem.Id
        };
    }

    public static OpenFoodFactsDTO ToDTO(this OpenFoodFacts foodFacts)
    {
        var images = new List<string>();
        if (!string.IsNullOrWhiteSpace(foodFacts.Product.image_front_url)) images.Add(foodFacts.Product.image_front_url);
        if (!string.IsNullOrWhiteSpace(foodFacts.Product.image_url)) images.Add(foodFacts.Product.image_url);
        if (!string.IsNullOrWhiteSpace(foodFacts.Product.image_ingredients_url)) images.Add(foodFacts.Product.image_ingredients_url);
        if (!string.IsNullOrWhiteSpace(foodFacts.Product.image_nutrition_url)) images.Add(foodFacts.Product.image_nutrition_url);

        return new OpenFoodFactsDTO
        {
            Brand = foodFacts.Product.brands,
            Code = foodFacts.Product.code,
            IngredientsText = foodFacts.Product.ingredients_text,
            NutritionDataPer = foodFacts.Product.nutrition_data_per,
            NutrotionGrades = foodFacts.Product.nutrition_grades,
            ProductName = foodFacts.Product.product_name,
            Quantity = foodFacts.Product.quantity,
            Images = images,
            Ingredients = foodFacts.Product.ingredients,
            Nutriments = foodFacts.Product.nutriments
        };
    }
}
