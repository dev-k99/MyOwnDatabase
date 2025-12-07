using System;
using System.Collections.Generic;
using System.Linq;

namespace MyOwnDatabase
{
    public class Token
    {
        public string Value { get; set; } = string.Empty;
        public TokenType Type { get; set; }
    }

    public enum TokenType { Keyword, Identifier, Operator, Literal, Comma, Semicolon, Star }

    public class Query
    {
        public string Command { get; set; } = string.Empty; // SELECT, INSERT, etc.
        public string Table { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new List<string>(); // For SELECT
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>(); // For INSERT
        public string WhereCondition { get; set; } = string.Empty; // Simple WHERE clause
    }

    public class Parser
    {
        public Query Parse(string sql)
        {
            var tokens = Tokenize(sql.ToUpper());
            var query = new Query();

            if (tokens.Count < 3) throw new InvalidOperationException("Invalid SQL");

            query.Command = tokens[0].Value;
            query.Table = tokens[2].Value;

            switch (query.Command)
            {
                case "SELECT":
                    query.Columns = ParseSelectColumns(tokens);
                    break;
                case "INSERT":
                    query.Values = ParseInsertValues(tokens);
                    break;
                // Add UPDATE/DELETE later
                default:
                    throw new NotSupportedException($"Command '{query.Command}' not supported.");
            }

            return query;
        }

        private List<Token> Tokenize(string sql)
        {
            var tokens = new List<Token>();
            var current = "";
            var i = 0;

            while (i < sql.Length)
            {
                var ch = sql[i];

                if (char.IsWhiteSpace(ch))
                {
                    if (!string.IsNullOrEmpty(current))
                        AddToken(tokens, current);
                    current = "";
                }
                else if (ch == '*' || ch == ';' || ch == ',')
                {
                    if (!string.IsNullOrEmpty(current))
                        AddToken(tokens, current);
                    tokens.Add(new Token { Value = ch.ToString(), Type = TokenType.Star });
                    current = "";
                }
                else if (ch == '=')
                {
                    if (!string.IsNullOrEmpty(current))
                        AddToken(tokens, current);
                    tokens.Add(new Token { Value = "=", Type = TokenType.Operator });
                    current = "";
                }
                else
                {
                    current += ch;
                }
                i++;
            }

            if (!string.IsNullOrEmpty(current))
                AddToken(tokens, current);

            return tokens;
        }

        private void AddToken(List<Token> tokens, string value)
        {
            if (value == "*") tokens.Add(new Token { Value = value, Type = TokenType.Star });
            else if (value == "SELECT" || value == "INSERT") tokens.Add(new Token { Value = value, Type = TokenType.Keyword });
            else tokens.Add(new Token { Value = value, Type = TokenType.Identifier });
        }

        private List<string> ParseSelectColumns(List<Token> tokens)
        {
            var columns = new List<string>();
            var i = tokens.FindIndex(t => t.Value == "FROM") - 1;

            if (tokens[i].Value == "*")
                return new List<string> { "*" };

            // Simple: assume comma-separated after SELECT
            var colTokens = tokens.Skip(1).TakeWhile(t => t.Type != TokenType.Keyword).ToList();
            columns = colTokens.Select(t => t.Value).ToList();

            return columns;
        }

        private Dictionary<string, object> ParseInsertValues(List<Token> tokens)
        {
            var values = new Dictionary<string, object>();
            var i = tokens.FindIndex(t => t.Value == "VALUES") + 1;

            // Simple: assume key=value pairs after VALUES
            for (int j = i; j < tokens.Count; j += 2)
            {
                if (j + 1 < tokens.Count && tokens[j].Type == TokenType.Identifier && tokens[j + 1].Type == TokenType.Literal)
                    values[tokens[j].Value] = tokens[j + 1].Value;
            }

            return values;
        }
    }
}