using Newtonsoft.Json;

namespace F_F.Core.Responses;

// Minimal, structured models for OpenFoodFacts V2 response
// Only the fields used in mapping/persistence are included.
public class OpenFoodFacts
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("product")]
    public Product Product { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("status_verbose")]
    public string StatusVerbose { get; set; }
}

public class Product
{
    [JsonProperty("code")]
    public string code { get; set; }

    [JsonProperty("brands")]
    public string brands { get; set; }

    [JsonProperty("product_name")]
    public string product_name { get; set; }

    [JsonProperty("quantity")]
    public string quantity { get; set; }

    [JsonProperty("nutrition_grades")]
    public string nutrition_grades { get; set; }

    [JsonProperty("nutrition_data_per")]
    public string nutrition_data_per { get; set; }

    [JsonProperty("ingredients_text")]
    public string ingredients_text { get; set; }

    [JsonProperty("ingredients")]
    public List<Ingredient> ingredients { get; set; }

    [JsonProperty("nutriments")]
    public Nutriments nutriments { get; set; }

    [JsonProperty("image_front_url")]
    public string image_front_url { get; set; }

    [JsonProperty("image_url")]
    public string image_url { get; set; }

    [JsonProperty("image_ingredients_url")]
    public string image_ingredients_url { get; set; }

    [JsonProperty("image_nutrition_url")]
    public string image_nutrition_url { get; set; }
}

public class Ingredient
{
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("text")]
    public string text { get; set; }

    [JsonProperty("percent_estimate")]
    public decimal? percent_estimate { get; set; }

    [JsonProperty("percent_max")]
    public decimal? percent_max { get; set; }

    [JsonProperty("percent_min")]
    public decimal? percent_min { get; set; }

    [JsonProperty("ingredients")]
    public List<Ingredient> ingredients { get; set; }
}

public class Nutriments
{
    [JsonProperty("carbohydrates")]
    public decimal? carbohydrates { get; set; }

    [JsonProperty("carbohydrates_100g")]
    public decimal? carbohydrates_100g { get; set; }

    [JsonProperty("energy")]
    public decimal? energy { get; set; }

    [JsonProperty("energy-kcal")]
    public decimal? energy_kcal { get; set; }

    [JsonProperty("fat")]
    public decimal? fat { get; set; }

    [JsonProperty("fat_100g")]
    public decimal? fat_100g { get; set; }

    [JsonProperty("proteins")]
    public decimal? proteins { get; set; }

    [JsonProperty("proteins_100g")]
    public decimal? proteins_100g { get; set; }

    [JsonProperty("salt")]
    public decimal? salt { get; set; }

    [JsonProperty("salt_100g")]
    public decimal? salt_100g { get; set; }

    [JsonProperty("saturated-fat")]
    public decimal? saturated_fat { get; set; }

    [JsonProperty("saturated-fat_100g")]
    public decimal? saturated_fat_100g { get; set; }

    [JsonProperty("sugars")]
    public decimal? sugars { get; set; }

    [JsonProperty("sugars_100g")]
    public decimal? sugars_100g { get; set; }
}

