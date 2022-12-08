

namespace Universe
{
    public abstract class USpreadsheetProvider : UniverseScriptableObject
    {
        public abstract string GetSpreadsheet( USpreadsheetLinkData data );
        public SpreadsheetProviderEvent OnCompleteEvent = new SpreadsheetProviderEvent();
    }
}