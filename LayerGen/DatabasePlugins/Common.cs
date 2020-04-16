﻿using System;
using System.Linq;
using System.Text;

namespace LayerGen.DatabasePlugins
{
    internal class Common
    {
        internal static void DoComments(ref StringBuilder templateText, string commentDelimiter, bool includeComments)
        {
            if (includeComments)
            {
                templateText.Replace("{$Comment1}", commentDelimiter + " Created using LayerGen 3.5" + Environment.NewLine);
                templateText.Replace("{$Comment}", "");
                templateText.Replace("{/$Comment}", "");
            }
            else
            {
                templateText.Replace("{$Comment1}", "");
                int ndx1 = templateText.IndexOf("{$Comment}");
                int ndx2 = templateText.IndexOf("{/$Comment}");

                while (ndx1 >= 0 && ndx2 >= 0)
                {
                    templateText.Remove(ndx1, ndx2 - ndx1 + 11);

                    ndx1 = templateText.IndexOf("{$Comment}");
                    ndx2 = templateText.IndexOf("{/$Comment}");
                }
            }
        }

        internal static void DoComments(ref StringBuilder templateText, string commentDelimiter, bool includeComments, string customComments)
        {
            if (includeComments)
            {
                if (string.IsNullOrEmpty(customComments))
                {
                    templateText.Replace("{$Comment1}", commentDelimiter + " Created using LayerGen 4.0" + Environment.NewLine);
                }
                else
                {
                    templateText.Replace("{$Comment1}", customComments + Environment.NewLine
                        + commentDelimiter + " Created using LayerGen 4.0" + Environment.NewLine);
                }
                templateText.Replace("{$Comment}", "");
                templateText.Replace("{/$Comment}", "");
            }
            else
            {
                templateText.Replace("{$Comment1}", "");
                int ndx1 = templateText.IndexOf("{$Comment}");
                int ndx2 = templateText.IndexOf("{/$Comment}");

                while (ndx1 >= 0 && ndx2 >= 0)
                {
                    templateText.Remove(ndx1, ndx2 - ndx1 + 11);

                    ndx1 = templateText.IndexOf("{$Comment}");
                    ndx2 = templateText.IndexOf("{/$Comment}");
                }
            }
        }

        /// <summary>
        /// Converts a string containing a name into a name safely
        /// suited for VB.Net. It does this by converting spaces into
        /// underscores and wraps the name in square brackets if its a reserved
        /// VB.Net keyword.
        /// </summary>
        /// <param name="fieldName">The string containing the name</param>
        /// <returns>The safe VB.Net name.</returns>
        internal static string GetSafeVbName(string fieldName)
        {
            string originalName = fieldName;
            fieldName = fieldName.ToLower();

            if (fieldName == "addhandler" || fieldName == "addressof" || fieldName == "alias" || fieldName == "and" ||
                fieldName == "andalso" || fieldName == "as" || fieldName == "boolean" || fieldName == "byref" ||
                fieldName == "byte" || fieldName == "byval" || fieldName == "call" || fieldName == "case" ||
                fieldName == "catch" || fieldName == "cbool" || fieldName == "cbyte" || fieldName == "cchar" ||
                fieldName == "cdate" || fieldName == "cdec" || fieldName == "cdbl" || fieldName == "char" ||
                fieldName == "cint" || fieldName == "class" || fieldName == "clng" || fieldName == "cobj" ||
                fieldName == "const" || fieldName == "continue" || fieldName == "csbyte" || fieldName == "cshort" ||
                fieldName == "csng" || fieldName == "cstr" || fieldName == "ctype" || fieldName == "cuint" ||
                fieldName == "culng" || fieldName == "cushort" || fieldName == "date" || fieldName == "decimal" ||
                fieldName == "declare" || fieldName == "default" || fieldName == "delegate" || fieldName == "dim" ||
                fieldName == "directcast" || fieldName == "do" || fieldName == "double" || fieldName == "each" ||
                fieldName == "else" || fieldName == "elseif" || fieldName == "end" || fieldName == "endif" ||
                fieldName == "enum" || fieldName == "erase" || fieldName == "error" || fieldName == "event" ||
                fieldName == "exit" || fieldName == "false" || fieldName == "finally" || fieldName == "for" ||
                fieldName == "friend" || fieldName == "function" || fieldName == "get" || fieldName == "gettype" ||
                fieldName == "getxmlnamespace" || fieldName == "global" || fieldName == "gosub" || fieldName == "goto" ||
                fieldName == "handles" || fieldName == "if" || fieldName == "implements" || fieldName == "imports" ||
                fieldName == "in" || fieldName == "inherits" || fieldName == "integer" || fieldName == "interface" ||
                fieldName == "is" || fieldName == "isnot" || fieldName == "let" || fieldName == "lib" ||
                fieldName == "like" || fieldName == "long" || fieldName == "loop" || fieldName == "me" ||
                fieldName == "mod" || fieldName == "module" || fieldName == "mustinherit" || fieldName == "mustoverride" ||
                fieldName == "mybase" || fieldName == "myclass" || fieldName == "namespace" || fieldName == "narrowing" ||
                fieldName == "new" || fieldName == "next" || fieldName == "not" || fieldName == "nothing" ||
                fieldName == "notinheritable" || fieldName == "notoverridable" || fieldName == "object" ||
                fieldName == "of" || fieldName == "on" || fieldName == "operator" || fieldName == "option" ||
                fieldName == "optional" || fieldName == "or" || fieldName == "orelse" || fieldName == "overloads" ||
                fieldName == "overridable" || fieldName == "overrides" || fieldName == "paramarray" || fieldName == "partial" ||
                fieldName == "private" || fieldName == "property" || fieldName == "protected" || fieldName == "public" ||
                fieldName == "raiseevent" || fieldName == "readonly" || fieldName == "redim" ||
                fieldName == "rem" || fieldName == "removehandler" || fieldName == "resume" || fieldName == "return" ||
                fieldName == "sbyte" || fieldName == "select" || fieldName == "set" || fieldName == "shadows" ||
                fieldName == "shared" || fieldName == "short" || fieldName == "single" || fieldName == "static" ||
                fieldName == "step" || fieldName == "stop" || fieldName == "string" || fieldName == "structure" ||
                fieldName == "sub" || fieldName == "synclock" || fieldName == "then" || fieldName == "throw" ||
                fieldName == "to" || fieldName == "true" || fieldName == "try" || fieldName == "trycast" ||
                fieldName == "typeof" || fieldName == "variant" || fieldName == "wend" || fieldName == "uinteger" ||
                fieldName == "ulong" || fieldName == "ushort" || fieldName == "using" || fieldName == "when" ||
                fieldName == "while" || fieldName == "widening" || fieldName == "with" || fieldName == "withevents" ||
                fieldName == "writeonly" || fieldName == "xor")
            {
                return "[" + originalName.Replace(" ", "_") + "]";
            }

            return originalName.Replace(" ", "_");
        }

        /// <summary>
        /// Converts a string containing a name into a name safely
        /// suited for C#. It does this by converting spaces into
        /// underscores and wraps the name in square brackets if its a reserved
        /// C# keyword.
        /// </summary>
        /// <param name="fieldName">The string containing the name</param>
        /// <returns>The safe C# name.</returns>
        internal static string GetSafeCsName(string fieldName)
        {
            string originalName = fieldName;
            originalName = originalName.Replace("-", "_");
            fieldName = fieldName.ToLower();

            if (fieldName == "abstract" || fieldName == "as" || fieldName == "base" || fieldName == "bool" ||
                fieldName == "break" || fieldName == "byte" || fieldName == "case" || fieldName == "catch" ||
                fieldName == "char" || fieldName == "checked" || fieldName == "class" || fieldName == "const" ||
                fieldName == "continue" || fieldName == "decimal" || fieldName == "default" || fieldName == "delegate" ||
                fieldName == "do" || fieldName == "double" || fieldName == "else" || fieldName == "enum" ||
                fieldName == "event" || fieldName == "explicit" || fieldName == "extern" || fieldName == "false" ||
                fieldName == "finally" || fieldName == "fixed" || fieldName == "float" || fieldName == "for" ||
                fieldName == "foreach" || fieldName == "goto" || fieldName == "if" || fieldName == "implicit" ||
                fieldName == "in" || fieldName == "int" || fieldName == "interface" || fieldName == "internal" ||
                fieldName == "is" || fieldName == "lock" || fieldName == "long" || fieldName == "namespace" ||
                fieldName == "new" || fieldName == "null" || fieldName == "object" || fieldName == "operator" ||
                fieldName == "out" || fieldName == "override" || fieldName == "params" || fieldName == "private" ||
                fieldName == "protected" || fieldName == "public" || fieldName == "readonly" || fieldName == "ref" ||
                fieldName == "return" || fieldName == "sbyte" || fieldName == "sealed" || fieldName == "short" ||
                fieldName == "sizeof" || fieldName == "stackalloc" || fieldName == "static" || fieldName == "string" ||
                fieldName == "struct" || fieldName == "switch" || fieldName == "this" || fieldName == "throw" ||
                fieldName == "true" || fieldName == "try" || fieldName == "typeof" || fieldName == "uint" ||
                fieldName == "ulong" || fieldName == "unchecked" || fieldName == "unsafe" || fieldName == "ushort" ||
                fieldName == "using" || fieldName == "yield" || fieldName == "virtual" || fieldName == "void" ||
                fieldName == "volatile" || fieldName == "while")
            {
                
                return "@" + originalName.Replace(" ", "_");
            }

            return originalName.Replace(" ", "_");
        }

        internal static string GetCsPropertyName(string fieldName, string className, string fieldType = null)
        {
            if (GetCsPropertyName(fieldName) == className)
                return GetCsPropertyName(fieldName, fieldType) + "_";
            return GetCsPropertyName(fieldName, fieldType);
        }

        internal static string GetCsPropertyName(string fieldName, string fieldType = null)
        {
            if (fieldName == "uiID")
            {
                return "Id";
            }

            string propertyName = fieldName;
            if (!string.IsNullOrEmpty(fieldType))
            {
                if (fieldType == "Table")
                {
                    if (fieldName.StartsWith("3g", StringComparison.Ordinal))
                    {
                        propertyName = "mobile" + fieldName.Substring(2);
                    }
                    if (char.IsNumber(propertyName[0]))
                    {
                        return "_" + Char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1);
                    }
                    return GetSafeCsName(Char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1));
                }

                switch (fieldType.ToLower())
                {
                    case "bool":
                        if (fieldName.StartsWith("b", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        break;
                    case "byte[]":
                        if (fieldName.StartsWith("by", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(2);
                        }
                        else if (fieldName.StartsWith("str", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(3);
                        }
                        break;
                    case "byte":
                    case "short":
                    case "int":
                    case "integer":
                    case "long":
                        if (fieldName.StartsWith("n", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        else if (fieldName.StartsWith("dw", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(2);
                        }
                        else if (fieldName.StartsWith("ui", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(2);
                        }
                        else if (fieldName.StartsWith("cr", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(2);
                        }
                        else if (fieldName.StartsWith("l", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        if (propertyName.EndsWith("ID", StringComparison.Ordinal))
                        {
                            propertyName = propertyName.Substring(0, propertyName.Length - 1) + "d";
                        }
                        break;
                    case "string":
                        if (fieldName.StartsWith("str", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(3);
                        }
                        else if (fieldName.StartsWith("s", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        break;
                    case "datetime":
                        if (fieldName.StartsWith("dt", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(2);
                        }
                        else if (fieldName.StartsWith("t", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        break;
                    case "decimal":
                    case "double":
                    case "float":
                        if (fieldName.StartsWith("db", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        else if (fieldName.StartsWith("d", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        else if (fieldName.StartsWith("f", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        else if (fieldName.StartsWith("n", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(1);
                        }
                        break;
                    case "timespan":
                        break;
                    case "guid":
                        if (fieldName.StartsWith("guid", StringComparison.Ordinal))
                        {
                            propertyName = fieldName.Substring(4);
                        }
                        break;
                }
            }

            if (char.IsNumber(propertyName[0]))
            {
                return "_" + Char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1);
            }

            return GetSafeCsName(Char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1));
        }

        internal static string GetCsFieldName(string fieldName)
        {
            return "_" + Char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
        }

        internal static string GetSafeJavaName(string fieldName)
        {
            string originalName = fieldName;
            originalName = originalName.Replace("-", "_");
            fieldName = fieldName.ToLower();

            if (fieldName == "abstract" || fieldName == "assert" || fieldName == "boolean" ||
                fieldName == "break" || fieldName == "byte" || fieldName == "case" || fieldName == "catch" ||
                fieldName == "char" || fieldName == "class" || fieldName == "const" ||
                fieldName == "continue" || fieldName == "default" || fieldName == "do" || fieldName == "double" || 
                fieldName == "else" || fieldName == "enum" || fieldName == "extends" || fieldName == "false" ||
                fieldName == "finally" || fieldName == "final" || fieldName == "float" || fieldName == "for" ||
                fieldName == "goto" || fieldName == "if" || fieldName == "implements" ||
                fieldName == "import" || fieldName == "instanceof" || fieldName == "int" || fieldName == "interface" ||
                fieldName == "long" || fieldName == "native" ||
                fieldName == "new" || fieldName == "null" || fieldName == "package" || fieldName == "private" ||
                fieldName == "protected" || fieldName == "public" ||
                fieldName == "return" || fieldName == "short" || fieldName == "static" || fieldName == "strictfp" ||
                fieldName == "super" || fieldName == "switch" || fieldName == "synchronized" ||
                fieldName == "this" || fieldName == "throw" || fieldName == "throws" || fieldName == "transient" ||
                fieldName == "true" || fieldName == "try" || fieldName == "void" ||
                fieldName == "volatile" || fieldName == "while")
            {

                return "_" + originalName.Replace(" ", "_");
            }

            return originalName.Replace(" ", "_");
        }

        internal static string GetJavaPropertyName(string fieldName, string className, string fieldType)
        {
            if (GetJavaPropertyName(fieldName, fieldType) == className)
                return GetJavaPropertyName(fieldName, fieldType) + "_";
            return GetJavaPropertyName(fieldName, fieldType);
        }

        internal static string GetJavaPropertyName(string fieldName, string fieldType)
        {
            if (fieldType == "Table")
            {
                if (char.IsNumber(fieldName[0]))
                {
                    return "_" + Char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);
                }
                return GetSafeJavaName(Char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1));
            }

            if (fieldName == "uiID")
            {
                return "Id";
            }
            

            string propertyName = fieldName;
            switch (fieldType.ToLower())
            {
                case "boolean":
                    if (fieldName.StartsWith("b", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    break;
                case "byte[]":
                    if (fieldName.StartsWith("by", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(2);
                    }
                    else if (fieldName.StartsWith("str", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(3);
                    }
                    break;
                case "byte":
                case "short":
                case "int":
                case "integer":
                case "long":
                    if (fieldName.StartsWith("n", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    else if (fieldName.StartsWith("dw", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(2);
                    }
                    else if (fieldName.StartsWith("ui", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(2);
                    }
                    else if (fieldName.StartsWith("cr", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(2);
                    }
                    else if (fieldName.StartsWith("l", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    if (propertyName.EndsWith("ID", StringComparison.Ordinal))
                    {
                        propertyName = propertyName.Substring(0, propertyName.Length - 1) + "d";
                    }
                    break;
                case "string":
                    if (fieldName.StartsWith("str", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(3);
                    }
                    else if (fieldName.StartsWith("s", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    break;
                case "date":
                    if (fieldName.StartsWith("dt", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(2);
                    }
                    else if (fieldName.StartsWith("t", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    break;
                case "bigdecimal":
                    break;
                case "double":
                case "float":
                    if (fieldName.StartsWith("db", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    else if (fieldName.StartsWith("d", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    else if (fieldName.StartsWith("f", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    else if (fieldName.StartsWith("n", StringComparison.Ordinal))
                    {
                        propertyName = fieldName.Substring(1);
                    }
                    break;
                case "time":
                    break;
            }
            if (char.IsNumber(propertyName[0]))
            {
                return "_" + Char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1);
            }

            return GetSafeJavaName(Char.ToUpperInvariant(propertyName[0]) + propertyName.Substring(1));
        }

        internal static string GetJavaFieldName(string fieldName)
        {
            return Char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
        }

        internal static string GetVbPropertyName(string fieldName)
        {
            if (char.IsNumber(fieldName[0]))
            {
                return "_" + Char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);
            }
            return GetSafeVbName(Char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1));
        }

        internal static string GetVbPropertyName(string fieldName, string className)
        {
            if (GetVbPropertyName(fieldName) == className)
                return GetVbPropertyName(fieldName) + "_";
            return GetVbPropertyName(fieldName);
        }

        internal static string GetVbFieldName(string fieldName)
        {
            return "_" + Char.ToLowerInvariant(fieldName[0]) + fieldName.Substring(1);
        }

        // Contains a list of all reserved SQL Server keywords
        // Returns true if the word passed in is a reserved SQL Server keyword
        internal static bool IsReservedSqlServerKeyword(string word)
        {
            string[] reservedWords =
            {
                "ADD", "EXTERNAL", "PROCEDURE", "ALL", "FETCH", "PUBLIC", "ALTER", "FILE", "RAISERROR", "AND", "FILLFACTOR", "READ",
                "ANY", "FOR", "READTEXT", "AS", "FOREIGN", "RECONFIGURE", "ASC", "FREETEXT", "REFERENCES", "AUTHORIZATION", "FREETEXTTABLE", "REPLICATION",
                "BACKUP", "FROM", "RESTORE", "BEGIN", "FULL", "RESTRICT", "BETWEEN", "FUNCTION", "RETURN", "BREAK", "GOTO", "REVERT",
                "BROWSE", "GRANT", "REVOKE", "BULK", "GROUP", "RIGHT", "BY", "HAVING", "ROLLBACK", "CASCADE", "HOLDLOCK", "ROWCOUNT",
                "CASE", "IDENTITY", "ROWGUIDCOL", "CHECK", "IDENTITY_INSERT", "RULE", "CHECKPOINT", "IDENTITYCOL", "SAVE",
                "CLOSE", "IF", "SCHEMA", "CLUSTERED", "IN", "SECURITYAUDIT", "COALESCE", "INDEX", "SELECT", "COLLATE", "INNER", "SEMANTICKEYPHRASETABLE",
                "COLUMN", "INSERT", "SEMANTICSIMILARITYDETAILSTABLE", "COMMIT", "INTERSECT", "SEMANTICSIMILARITYTABLE", "COMPUTE", "INTO", "SESSION_USER",
                "CONSTRAINT", "IS", "SET", "CONTAINS", "JOIN", "SETUSER", "CONTAINSTABLE", "KEY", "SHUTDOWN", "CONTINUE", "KILL", "SOME",
                "CONVERT", "LEFT", "STATISTICS", "CREATE", "LIKE", "SYSTEM_USER", "CROSS", "LINENO", "TABLE", "CURRENT", "LOAD", "TABLESTAMP",
                "CURRENT_DATE", "MERGE", "TEXTSIZE", "CURRENT_TIME", "NATIONAL", "THEN", "CURRENT_TIMESTAMP", "NOCHECK", "TO",
                "CURRENT_USER", "NONCLUSTERED", "TOP", "CURSOR", "NOT", "TRAN", "DATABASE", "NULL", "TRANSACTION", "DBCC", "NULLIF", "TRIGGER",
                "DEALLOCATE", "OF", "TRUNCATE", "DECLARE", "OFF", "TRY_CONVERT", "DEFAULT", "OFFSETS", "TSEQUAL", "DELETE", "ON", "UNION",
                "DENY", "OPEN", "UNIQUE", "DESC", "OPENDATASOURCE", "UNPIVOT", "DISK", "OPENQUERY", "UPDATE", "DISTINCT", "OPENROWSET", "UPDATETEXT",
                "DISTRIBUTED", "OPENXML", "USE", "DOUBLE", "OPTION", "USER", "DROP", "OR", "VALUES", "DUMP", "ORDER", "VARYING", "ELSE", "OUTER", "VIEW",
                "END", "OVER", "WAITFOR", "ERRLVL", "PERCENT", "WHEN", "ESCAPE", "PIVOT", "WHERE", "EXCEPT", "PLAN", "WHILE",
                "EXEC", "PRECISION", "WITH", "EXECUTE", "PRIMARY", "EXISTS", "PRINT", "WRITETEXT", "EXIT", "PROC", "TRUE", "FALSE"
            };

            return reservedWords.Any(z => word.ToUpper().Equals(z.ToUpper()));
        }

        // Contains a list of all reserved MySql keywords
        // Returns true if the word passed in is a reserved MySql keyword
        internal static bool IsReservedMySqlKeyword(string word)
        {
            string[] reservedWords =
            {
                "ACCESSIBLE", "ADD", "ALL", "ALTER", "ANALYZE", "AND", "AS", "ASC", "ASENSITIVE",
                "BEFORE", "BETWEEN", "BIGINT", "BINARY", "BLOB", "BOTH", "BY", "CALL", "CASCADE",
                "CASE", "CHANGE", "CHAR", "CHARACTER", "CHECK", "COLLATE", "COLUMN", "CONDITION", "CONSTRAINT",
                "CONTINUE", "CONVERT", "CREATE", "CROSS", "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR",
                "DATABASE", "DATABASES", "DAY_HOUR", "DAY_MICROSECOND", "DAY_MINUTE", "DAY_SECOND", "DEC", "DECIMAL", "DECLARE",
                "DEFAULT", "DELAYED", "DELETE", "DESC", "DESCRIBE", "DETERMINISTIC", "DISTINCT", "DISTINCTROW", "DIV",
                "DOUBLE", "DROP", "DUAL", "EACH", "ELSE", "ELSEIF", "ENCLOSED", "ESCAPED", "EXISTS", "EXIT", "EXPLAIN", "FALSE",
                "FETCH", "FLOAT", "FLOAT4", "FLOAT8", "FOR", "FORCE", "FOREIGN", "FROM", "FULLTEXT", "GRANT", "GROUP", "HAVING",
                "HIGH_PRIORITY", "HOUR_MICROSECOND", "HOUR_MINUTE", "HOUR_SECOND", "IF", "IGNORE", "IN", "INDEX", "INFILE",
                "INNER", "INOUT", "INSENSITIVE", "INSERT", "INT", "INT1", "INT2", "INT3", "INT4", "INT8", "INTEGER", "INTERVAL",
                "INTO", "IS", "ITERATE", "JOIN", "KEY", "KEYS", "KILL", "LEADING", "LEAVE", "LEFT", "LIKE", "LIMIT",
                "LINEAR", "LINES", "LOAD", "LOCALTIME", "LOCALTIMESTAMP", "LOCK", "LONG", "LONGBLOB", "LONGTEXT",
                "LOOP", "LOW_PRIORITY", "MASTER_SSL_VERIFY_SERVER_CERT", "MATCH", "MAXVALUE", "MEDIUMBLOB",
                "MEDIUMINT", "MEDIUMTEXT", "MIDDLEINT", "MINUTE_MICROSECOND", "MINUTE_SECOND", "MOD",
                "MODIFIES", "NATURAL", "NOT", "NO_WRITE_TO_BINLOG", "NULL", "NUMERIC", "ON", "OPTIMIZE", "OPTION",
                "OPTIONALLY", "OR", "ORDER", "OUT", "OUTER", "OUTFILE", "PRECISION", "PRIMARY", "PROCEDURE",
                "PURGE", "RANGE", "READ", "READS", "READ_WRITE", "REAL", "REFERENCES", "REGEXP", "RELEASE",
                "RENAME", "REPEAT", "REPLACE", "REQUIRE", "RESIGNAL", "RESTRICT", "RETURN", "REVOKE", "RIGHT",
                "RLIKE", "SCHEMA", "SCHEMAS", "SECOND_MICROSECOND", "SELECT", "SENSITIVE", "SEPARATOR", "SET", "SHOW",
                "SIGNAL", "SMALLINT", "SPATIAL", "SPECIFIC", "SQL", "SQLEXCEPTION", "SQLSTATE", "SQLWARNING", "SQL_BIG_RESULT",
                "SQL_CALC_FOUND_ROWS", "SQL_SMALL_RESULT", "SSL", "STARTING", "STRAIGHT_JOIN", "TABLE", "TERMINATED", "THEN", "TINYBLOB",
                "TINYINT", "TINYTEXT", "TO", "TRAILING", "TRIGGER", "TRUE", "UNDO", "UNION", "UNIQUE", "UNLOCK", "UNSIGNED", "UPDATE",
                "USAGE", "USE", "USING", "UTC_DATE", "UTC_TIME", "UTC_TIMESTAMP", "VALUES", "VARBINARY", "VARCHAR", "VARCHARACTER",
                "VARYING", "WHEN", "WHERE", "WHILE", "WITH", "WRITE", "XOR", "YEAR_MONTH", "ZEROFILL", "GENERAL", "IGNORE_SERVER_IDS",
                "MASTER_HEARTBEAT_PERIOD", "MAXVALUE", "RESIGNAL", "SIGNAL", "SLOW"
            };

            return reservedWords.Any(z => word.ToUpper().Equals(z.ToUpper()));
        }

        // Contains a list of all reserved Sqlite keywords
        // Returns true if the word passed in is a reserved Sqlite keyword
        internal static bool IsReservedSqliteKeyword(string word)
        {
            string[] reservedWords =
            {
                "ABORT", "CREATE", "FROM", "NATURAL", "ROLLBACK", "ACTION", "CROSS", "FULL", "NO", "ROW",
                "ADD", "CURRENT_DATE", "GLOB", "NOT", "SAVEPOINT", "AFTER", "CURRENT_TIME", "GROUP", "NOTNULL", "SELECT",
                "ALL", "CURRENT_TIMESTAMP", "HAVING", "NULL", "SET", "ALTER", "DATABASE", "IF", "OF", "TABLE",
                "ANALYZE", "DEFAULT", "IGNORE", "OFFSET", "TEMP", "AND", "DEFERRABLE", "IMMEDIATE", "ON", "TEMPORARY",
                "AS", "DEFERRED", "IN", "OR", "THEN", "ASC", "DELETE", "INDEX", "ORDER", "TO", "ATTACH", "DESC", "INDEXED", "OUTER", "TRANSACTION",
                "AUTOINCREMENT", "DETACH", "INITIALLY", "PLAN", "TRIGGER", "BEFORE", "DISTINCT", "INNER", "PRAGMA", "UNION",
                "BEGIN", "DROP", "INSERT", "PRIMARY", "UNIQUE", "BETWEEN", "EACH", "INSTEAD", "QUERY", "UPDATE",
                "BY", "ELSE", "INTERSECT", "RAISE", "USING", "CASCADE", "END", "INTO", "RECURSIVE", "VACUUM",
                "CASE", "ESCAPE", "IS", "REFERENCES", "VALUES", "CAST", "EXCEPT", "ISNULL", "REGEXP", "VIEW",
                "CHECK", "EXCLUSIVE", "JOIN", "REINDEX", "VIRTUAL", "COLLATE", "EXISTS", "KEY", "RELEASE", "WHEN",
                "COLUMN", "EXPLAIN", "LEFT", "RENAME", "WHERE", "COMMIT", "FAIL", "LIKE", "REPLACE", "WITH",
                "CONFLICT", "FOR", "LIMIT", "RESTRICT", "WITHOUT", "CONSTRAINT", "FOREIGN", "MATCH", "RIGHT"
            };

            return reservedWords.Any(z => word.ToUpper().Equals(z.ToUpper()));
        }

        internal static bool IsValueType(string csDataType)
        {
            switch (csDataType.ToLower())
            {
                case "bool":
                case "byte":
                case "char":
                case "datetime":
                case "decimal":
                case "double":
                case "float":
                case "guid":
                case "int":
                case "long":
                case "sbyte":
                case "short":
                case "timespan":
                case "uint":
                case "ulong":
                case "ushort":
                    return true;
            }
            return false;
        }
    }
}
