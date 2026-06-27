namespace SearchComparisonNet.GUI.ViewModels;

public class ViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    #region INotifyDataErrorInfo
    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName == null)
            return Enumerable.Empty<string>();

        PropErrors.TryGetValue(propertyName, out List<string>? errors);
        return errors ?? Enumerable.Empty<string>();
    }

    public bool HasErrors => PropErrors.Values.Any(errors => errors.Count > 0);

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    #endregion INotifyDataErrorInfo

    protected void OnPropertyErrorsChanged(string p) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(p));

    protected readonly Dictionary<string, List<string>> PropErrors = new();
}
