namespace NotepadCore.Functionality
{
    public class FindNextResult
    {
        bool searchStatus;
        int selectionStart;

        public bool SearchStatus
        {
            get { return searchStatus; }
            set { searchStatus = value; }
        }

        public int SelectionStart
        {
            get { return selectionStart; }
            set { selectionStart = value; }
        }
    }
}
