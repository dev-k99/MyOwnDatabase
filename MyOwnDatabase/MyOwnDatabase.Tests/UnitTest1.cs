using Xunit;
using MyOwnDatabase;

namespace MyOwnDatabase.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CanCreateTableAndInsertRow()
        {
            var db = new Database();
            db.CreateTable("Test", new List<string> { "Id", "Name" });
            db.InsertRow("Test", new Dictionary<string, object> { { "Id", 1 }, { "Name", "TestUser" } });

            Assert.True(db.Tables.ContainsKey("Test"));
            Assert.Single(db.Tables["Test"].Rows);
        }

        [Fact]
        public void CanSelectWithWhere()
        {
            var db = new Database();
            db.CreateTable("Users", new List<string> { "Id", "Name" });
            db.InsertRow("Users", new Dictionary<string, object> { { "Id", 1 }, { "Name", "Kwanele" } });
            db.InsertRow("Users", new Dictionary<string, object> { { "Id", 2 }, { "Name", "John" } });

            var results = db.Select("Users", new List<string> { "*" }, "Id = 1");
            Assert.Single(results);
            Assert.Equal("Kwanele", results[0]["Name"]);
        }

        [Fact]
        public void CanJoinTables()
        {
            var db = new Database();
            db.CreateTable("Users", new List<string> { "Id", "Name" });
            db.InsertRow("Users", new Dictionary<string, object> { { "Id", 1 }, { "Name", "Kwanele" } });

            db.CreateTable("Orders", new List<string> { "OrderId", "UserId" });
            db.InsertRow("Orders", new Dictionary<string, object> { { "OrderId", 101 }, { "UserId", 1 } });

            var joined = db.Join("Users", "Orders", "t1.Id = t2.UserId");
            Assert.Single(joined);
            Assert.Equal("Kwanele", joined[0]["t1_Name"]);
            Assert.Equal(101, joined[0]["t2_OrderId"]);
        }

        [Fact]
        public void CanUpdateAndDelete()
        {
            var db = new Database();
            db.CreateTable("Test", new List<string> { "Id", "Value" });
            db.InsertRow("Test", new Dictionary<string, object> { { "Id", 1 }, { "Value", "Old" } });

            db.Update("Test", new Dictionary<string, object> { { "Value", "New" } }, "Id = 1");
            var updated = db.Select("Test", new List<string> { "*" });
            Assert.Equal("New", updated[0]["Value"]);

            db.Delete("Test", "Id = 1");
            var deleted = db.Select("Test", new List<string> { "*" });
            Assert.Empty(deleted);
        }
    }
}