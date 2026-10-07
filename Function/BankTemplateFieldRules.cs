using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using DocumentFormat.OpenXml.Spreadsheet;
using MIS.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;


namespace MIS.Function
{
    internal static class BankTemplateFieldRules
    {
        private static readonly Dictionary<string, string> SourceToMcc = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "Nature of Request",
                "Request Type"
            },
            {
                "TID Issuance Type",
                "POS Type"
            },
            {
                "Requester's Name",
                "Requestor"
            },
            {
                "Merchant's DBA Name",
                "Merchant Location/DBA Name"
            },
            {
                "Merchant's DBA Address",
                "Address"
            },
            {
                "Merchant's DBA City",
                "City"
            },
            {
                "Merchant's DBA Province",
                "Area 1 (Metro Manila / Provincial)"
            },
            {
                "Store Representative",
                "Contact Person"
            },
            {
                "Contact Number (Outlet / Branch)",
                "Contact Number"
            },
            {
                "Required Date and Time of Installation",
                "Target Installation date"
            },
            {
                "Remarks / Special Instructions " +
                "(Dispatch-related, etc.)",
                "RM Instruction / Remarks"
            }
        };

        private static readonly HashSet<string> JsonOnlyTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
          {
             "Prepared by",
             "Checked & Approved by",
             "Reference Credit TID",
             "Credit Straight MID-VMJACD"
          };

        public static void ApplyDefaults(
            IList<BankTemplateRow> rows)
        {
            ApplyDefaults(rows, false);
        }

        public static void ApplyDefaults(
            IList<BankTemplateRow> rows,
            bool isAdditionalTerminal)
        {
            if (rows == null)
                throw new ArgumentNullException("rows");

            foreach (BankTemplateRow row in rows)
            {
                string tag = CanonicalizeTag(row.Tag);
                string mccColumn;

                row.MccColumn = string.Empty;
                row.MccValue = string.Empty;

                if (TryGetIdentifierMccColumn(
                        tag,
                        isAdditionalTerminal,
                        out mccColumn))
                {
                    row.MccColumn = mccColumn;
                    row.MccValue = NormalizeIdentifierList(row.Value);
                    row.IsSelected = row.IsSelectable;
                    continue;
                }

                if (SourceToMcc.TryGetValue(
                        tag,
                        out mccColumn))
                {
                    row.MccColumn = mccColumn;

                    row.MccValue = ConvertMccValue(
                        tag,
                        row.Value);

                    row.IsSelected = row.IsSelectable;
                    continue;
                }

                row.IsSelected =
                    row.IsSelectable &&
                    (JsonOnlyTags.Contains(tag) ||
                     tag.StartsWith(
                         "Sheet2 - ",
                         StringComparison.OrdinalIgnoreCase));
            }
        }

        private static bool TryGetIdentifierMccColumn(
            string tag,
            bool isAdditionalTerminal,
            out string mccColumn)
        {
            mccColumn = string.Empty;

            if (isAdditionalTerminal)
            {
                if (string.Equals(
                        tag,
                        "Sheet2 - MID",
                        StringComparison.OrdinalIgnoreCase))
                {
                    mccColumn = "Merchant ID (MID)";
                    return true;
                }

                if (string.Equals(
                        tag,
                        "Sheet2 - TID",
                        StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(
                        tag,
                        "Credit TID",
                        StringComparison.OrdinalIgnoreCase))
                {
                    mccColumn = "Terminal ID (TID)";
                    return true;
                }

                if (string.Equals(
                        tag,
                        "Debit TID",
                        StringComparison.OrdinalIgnoreCase))
                {
                    mccColumn = "Bancnet TID";
                    return true;
                }

                return false;
            }

            if (string.Equals(
                    tag,
                    "Debit TID",
                    StringComparison.OrdinalIgnoreCase))
            {
                mccColumn = "Merchant ID (MID)";
                return true;
            }

            if (string.Equals(
                    tag,
                    "Credit TID",
                    StringComparison.OrdinalIgnoreCase))
            {
                mccColumn = "Terminal ID (TID)";
                return true;
            }

            return false;
        }

        public static string NormalizeIdentifierList(string sourceValue)
        {
            string value = (sourceValue ?? string.Empty).Trim();

            Match rangeMatch = Regex.Match(
                value,
                @"^(\d+)\s+to\s+(\d+)$",
                RegexOptions.IgnoreCase);

            if (rangeMatch.Success)
            {
                string startText = rangeMatch.Groups[1].Value;
                string endText = rangeMatch.Groups[2].Value;
                long start;
                long end;

                if (long.TryParse(
                        startText,
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out start) &&
                    long.TryParse(
                        endText,
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out end) &&
                    end >= start &&
                    end - start <= 1000)
                {
                    int width = Math.Max(
                        startText.Length,
                        endText.Length);

                    List<string> expanded = new List<string>();

                    for (long current = start;
                         current <= end;
                         current++)
                    {
                        expanded.Add(
                            current.ToString(
                                new string('0', width),
                                CultureInfo.InvariantCulture));
                    }

                    return string.Join(", ", expanded);
                }
            }

            MatchCollection matches = Regex.Matches(value, @"\d+");

            if (matches.Count <= 1)
                return value;

            List<string> identifiers = new List<string>();
            HashSet<string> seen = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (Match match in matches)
            {
                if (seen.Add(match.Value))
                    identifiers.Add(match.Value);
            }

            return string.Join(", ", identifiers);
        }



        private static string ConvertMccValue(
            string sourceTag, string sourceValue)

        {
            string tag = CanonicalizeTag(sourceTag);
            string value = (sourceValue ?? string.Empty).Trim();

            if (string.Equals(
                    tag,
                    "Nature of Request",
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    value.Replace(" ", string.Empty),
                    "TIDISSUANCE",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "New Installation";
            }

            if (string.Equals(
                tag,
                "TID Issuance Type",
                StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    value.Replace(" ", string.Empty),
                    "MINIPOS",
                    StringComparison.OrdinalIgnoreCase))         
            {
                return "Reprogramming";
            }

            return value;
        }
        public static bool TryGetMccColumn(
            string sourceTag,
            out string mccColumn)
        {
            return SourceToMcc.TryGetValue(
                CanonicalizeTag(sourceTag),
                out mccColumn);
        }

        public static string CanonicalizeTag(string tag)
        {
            string result = (tag ?? string.Empty).Trim();

            result = result.TrimEnd(':', '>');

            return Regex.Replace(result, @"\s+", " ");
        }

        public static string NormalizeJsonKey(string tag)
        {
            string result = CanonicalizeTag(tag);

            result = result.Replace("&", " and ");

            result = Regex.Replace(
                result, @"[^A-Za-z0-9]+", "_");

            return result.Trim('_');
        }
    }

}
