using AssociationRule;

Console.WriteLine("Hello, World!");

// Create a list of lists to represent the shopping baskets for different persons
List<List<string>> shoppingBaskets = new List<List<string>>();

// Add products to each person's shopping basket
List<string> person1Basket = new List<string> { "Apple", "Milk", "Bread", "Eggs", "Butter" };
List<string> person2Basket = new List<string> { "Banana", "Yogurt", "Bread", "Chicken", "Rice" };
List<string> person3Basket = new List<string> { "Orange", "Cheese", "Milk", "Pasta", "Tomato Sauce" };
List<string> person4Basket = new List<string> { "Apple", "Bread", "Eggs", "Lettuce", "Cucumber" };
List<string> person5Basket = new List<string> { "Steak", "Potatoes", "Carrots", "Peas", "Gravy" };
List<string> person6Basket = new List<string> { "Fish", "Lemon", "Butter", "Garlic", "Herbs" };
List<string> person7Basket = new List<string> { "Fish", "Apple", "Butter", "Garlic", "Herbs" };
List<string> person8Basket = new List<string> { "Fish", "Apple", "Butter", "Cucumber", "Gravy" };
List<string> person9Basket = new List<string> { "Carrots", "Bread", "Eggs", "Cucumber", "Gravy" };
List<string> person10Basket = new List<string> { "Carrots", "Bread", "Eggs", "Cucumber", "Gravy" };


shoppingBaskets.Add(person1Basket);
shoppingBaskets.Add(person2Basket);
shoppingBaskets.Add(person3Basket);
shoppingBaskets.Add(person4Basket);
shoppingBaskets.Add(person5Basket);
shoppingBaskets.Add(person6Basket);
shoppingBaskets.Add(person7Basket);
shoppingBaskets.Add(person8Basket);
shoppingBaskets.Add(person9Basket);
shoppingBaskets.Add(person10Basket);


// Print the shopping baskets for each person
for (int i = 0; i < shoppingBaskets.Count; i++)
{
    Console.WriteLine($"Person {i + 1}'s Basket:");
    foreach (var item in shoppingBaskets[i])
    {
        Console.WriteLine($"- {item}");
    }
    Console.WriteLine();
}

Console.WriteLine("----------------------------------------------------------------------------------");

var service = new Service();
var result = service.MineAssociationRules(shoppingBaskets, 3, 0.5);

foreach (var item in result.OrderByDescending(x => x.Confidence))
{
    Console.WriteLine($"{item.rule} : {item.Confidence}");
}