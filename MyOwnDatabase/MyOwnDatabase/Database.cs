using System;
using System.Collections.Generic;
using System.Linq;

namespace MyOwnDatabase
{
    public class Row
    {
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }

    public class Table
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new List<string>();
        public List<Row> Rows { get; set; } = new List<Row>();
    }

    public class Database
    {
        public Dictionary<string, Table> Tables { get; set; } = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);

        // Step 1: Create a new table with specified columns
        public void CreateTable(string name, List<string> columns)
        {
            if (Tables.ContainsKey(name))
                throw new InvalidOperationException($"Table '{name}' already exists.");

            Tables[name] = new Table { Name = name, Columns = columns };
        }

        // Step 2: Insert a row into the table
        public void InsertRow(string tableName, Dictionary<string, object> values)
        {
            if (!Tables.TryGetValue(tableName, out var table))
                throw new InvalidOperationException($"Table '{tableName}' does not exist.");

            var row = new Row { Values = values };
            table.Rows.Add(row);
        }

        // Step 3: Select rows from the table (with optional columns and WHERE)
        public List<Dictionary<string, object>> Select(string tableName, List<string> columns, string where = "")
        {
            if (!Tables.TryGetValue(tableName, out var table))
                throw new InvalidOperationException($"Table '{tableName}' does not exist.");

            var results = new List<Dictionary<string, object>>();

            foreach (var row in table.Rows)
            {
                if (string.IsNullOrEmpty(where) || MatchesWhere(row, where))
                {
                    var resultRow = new Dictionary<string, object>();
                    if (columns.Contains("*") || columns.Count == 0)
                    {
                        resultRow = row.Values;
                    }
                    else
                    {
                        foreach (var col in columns)
                        {
                            if (row.Values.TryGetValue(col, out var val))
                                resultRow[col] = val;
                            else
                                resultRow[col] = null!; // Null for missing columns
                        }
                    }
                    results.Add(resultRow);
                }
            }

            return results;
        }

        // Step 3: Update rows in the table (with values and optional WHERE)
        public void Update(string tableName, Dictionary<string, object> values, string where = "")
        {
            if (!Tables.TryGetValue(tableName, out var table))
                throw new InvalidOperationException($"Table '{tableName}' does not exist.");

            foreach (var row in table.Rows)
            {
                if (string.IsNullOrEmpty(where) || MatchesWhere(row, where))
                {
                    foreach (var kv in values)
                    {
                        row.Values[kv.Key] = kv.Value;
                    }
                }
            }
        }

        // Step 3: Delete rows from the table (with optional WHERE)
        public void Delete(string tableName, string where = "")
        {
            if (!Tables.TryGetValue(tableName, out var table))
                throw new InvalidOperationException($"Table '{tableName}' does not exist.");

            table.Rows.RemoveAll(row => string.IsNullOrEmpty(where) || MatchesWhere(row, where));
        }

        // Helper for simple WHERE clauses (e.g., "Id = 1" or "Name = 'Kwanele'")
        private bool MatchesWhere(Row row, string where)
        {
            if (string.IsNullOrEmpty(where)) return true;

            // Basic parsing for "Column = Value" (supports strings with single quotes)
            var parts = where.Split(new[] { '=' }, 2);
            if (parts.Length != 2) throw new InvalidOperationException("Invalid WHERE clause.");

            var column = parts[0].Trim();
            var value = parts[1].Trim().Trim('\''); // Remove single quotes for strings

            return row.Values.TryGetValue(column, out var cellValue) && cellValue?.ToString() == value;
        }
        public List<Dictionary<string, object>> Join(string table1, string table2, string onCondition)
        {
            if (!Tables.TryGetValue(table1, out var t1) || !Tables.TryGetValue(table2, out var t2))
                throw new InvalidOperationException("Table not found.");

            var results = new List<Dictionary<string, object>>();

            // Parse simple onCondition like "t1.Id = t2.UserId"
            var parts = onCondition.Split('=');
            if (parts.Length != 2)
                throw new InvalidOperationException("JOIN condition must be 't1.Column = t2.Column'");

            var left = parts[0].Trim();  // e.g., "t1.Id"
            var right = parts[1].Trim(); // e.g., "t2.UserId"

            var leftCol = left.Replace("t1.", "");
            var rightCol = right.Replace("t2.", "");

            foreach (var r1 in t1.Rows)
                foreach (var r2 in t2.Rows)
                {
                    if (r1.Values.TryGetValue(leftCol, out var v1) &&
                        r2.Values.TryGetValue(rightCol, out var v2) &&
                        Equals(v1, v2))
                    {
                        var joined = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                        // Add t1 columns (prefix to avoid conflicts)
                        foreach (var kv in r1.Values)
                            joined[$"t1_{kv.Key}"] = kv.Value;

                        // Add t2 columns
                        foreach (var kv in r2.Values)
                            joined[$"t2_{kv.Key}"] = kv.Value;

                        results.Add(joined);
                    }
                }

            return results;
        }

        public void BeginTransaction() { } // Placeholder for ACID
        public void CommitTransaction() { } // Placeholder
        public void RollbackTransaction() { } // Placeholder
    }
}