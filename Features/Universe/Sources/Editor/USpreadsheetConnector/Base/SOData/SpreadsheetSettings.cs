namespace Universe
{
    public struct SpreadsheetSettings
    {
        public bool m_downloadable;
        public USpreadsheetLinkData m_data;

        public SpreadsheetSettings( bool downloadable, USpreadsheetLinkData spreadsheetCredentialData ) : this()
        {
            m_downloadable = downloadable;
            m_data = spreadsheetCredentialData;
        }
    }
}