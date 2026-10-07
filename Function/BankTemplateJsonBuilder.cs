using MIS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace MIS.Function
{
    internal static class BankTemplateJsonBuilder
    {
        public static string Build( IList<BankTemplateRow> rows)
        {
            return Build(rows, string.Empty);
        }

        public static string Build(IList<BankTemplateRow> rows, string sourceFileName)
        {
            if (rows == null) throw new ArgumentNullException("rows");


            JObject values = new JObject();
            HashSet<string> usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int selectedCount = 0;

            foreach (BankTemplateRow row in rows)
            {
                if (!row.IsSelectable || !row.IsSelected)
                    continue;

                string baseKey = BankTemplateFieldRules.NormalizeJsonKey(row.Tag);

                if (string.IsNullOrWhiteSpace(baseKey))
                    continue;

                string uniqueKey = GetUniqueKey(baseKey, usedKeys);

                // Explicit string value.
                values.Add(uniqueKey, new JValue(row.Value ?? string.Empty));

                selectedCount++;
            }

            if (selectedCount == 0)
            {
                throw new InvalidOperationException("No fields are selected.");
            }

            JObject root = new JObject()
;
            if (!string.IsNullOrWhiteSpace(sourceFileName))
            {
                root["SourceBank"] = "BDO";
                root["SourceFileName"] = Path.GetFileName(sourceFileName);
            }

            root["Values"] = values;

            return root.ToString(Formatting.Indented);
        }

        private static string GetUniqueKey(string baseKey, ISet<string> usedKeys)
        {
            string key = baseKey;
            int suffix = 2;

            while (!usedKeys.Add(key))
            {
                key = baseKey + "_" + suffix;
                suffix++;
            }

            return key;
        }
    }
}