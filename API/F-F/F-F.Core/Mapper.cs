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
            Name = foodItem.Name,
        };
    }

    public static FoodItemDTO ToDTO(this FoodItem foodItem)
    {
        return new FoodItemDTO
        {
            Id = foodItem.Id,
            Name = foodItem.Name,
            Nutrition = foodItem.Nutrition
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
}