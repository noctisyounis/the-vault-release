using System.Collections.Generic;

namespace Universe
{
    public class SpreadsheetData : FactBase
    {
        #region Public

        public List<SpreadsheetLineData> m_lines = new List<SpreadsheetLineData>();

        public override string ToString() => $"[SpreadsheetData] {name} has {m_lines.Count} line(s).";

        #endregion
    }
}