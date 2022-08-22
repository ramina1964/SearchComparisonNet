namespace SearchComparisonNet.GUI.ViewModels;

public class ViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    #region INotifyDataErrorInfo
    public IEnumerable GetErrors(string propertyName)
    {
        if (propertyName == null)
            return null;

        PropErrors.TryGetValue(propertyName, out List<string> errors);
        return errors;
    }

    public bool HasErrors
    {
        get
        {
            try
            {
                var propErrorsCount = PropErrors.Values.FirstOrDefault(r => r.Count > 0);
                return propErrorsCount != null;
            }
            catch { }
            return true;
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    #endregion INotifyDataErrorInfo

    protected void OnPropertyErrorsChanged(string p) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(p));

    protected readonly Dictionary<string, List<string>> PropErrors = new();
}
