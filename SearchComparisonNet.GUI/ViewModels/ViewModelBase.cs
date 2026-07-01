namespace SearchComparisonNet.GUI.ViewModels;

public class ViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    // INotifyDataErrorInfo is required because the views bind with ValidatesOnNotifyDataErrors=True.
    // Input is validated via FluentValidation in the view model rather than per-property error
    // collections, so there are never any errors to surface here.
    public IEnumerable GetErrors(string? propertyName) => Enumerable.Empty<string>();

    public bool HasErrors => false;

#pragma warning disable CS0067 // Required by INotifyDataErrorInfo but intentionally never raised (validation is via FluentValidation).
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
#pragma warning restore CS0067
}
