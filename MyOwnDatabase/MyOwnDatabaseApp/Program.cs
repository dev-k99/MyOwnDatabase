using MyOwnDatabase;

//TESTING STORAGE
//var db = new Database();
//db.CreateTable("Users", new List<string> { "Id", "Name", "Email" });
//db.InsertRow("Users", new Dictionary<string, object> { { "Id", 1 }, { "Name", "Kwanele" }, { "Email", "test@email.com" } });
//var users = db.Select("Users", new List<string> { "*" });
//Console.WriteLine($"Found {users.Count} users: {users.First()["Name"]}");


//TESTING PARSING
var parser = new Parser();
var query = parser.Parse("SELECT * FROM Users");
Console.WriteLine($"Command: {query.Command}, Table: {query.Table}, Columns: {string.Join(", ", query.Columns)}");

var db = new Database();

// Create Users table
db.CreateTable("Users", new List<string> { "Id", "Name" });
db.InsertRow("Users", new Dictionary<string, object> { { "Id", 1 }, { "Name", "Kwanele" } });
db.InsertRow("Users", new Dictionary<string, object> { { "Id", 2 }, { "Name", "John" } });

// Create Orders table
db.CreateTable("Orders", new List<string> { "OrderId", "UserId", "Amount" });
db.InsertRow("Orders", new Dictionary<string, object> { { "OrderId", 101 }, { "UserId", 1 }, { "Amount", 450 } });
db.InsertRow("Orders", new Dictionary<string, object> { { "OrderId", 102 }, { "UserId", 2 }, { "Amount", 380 } });

// JOIN Users and Orders on Users.Id = Orders.UserId
var joined = db.Join("Users", "Orders", "t1.Id = t2.UserId");

Console.WriteLine($"JOIN returned {joined.Count} rows:");
foreach (var row in joined)
{
    Console.WriteLine($"User: {row["t1_Name"]}, Order: {row["t2_OrderId"]}, Amount: {row["t2_Amount"]}");
}